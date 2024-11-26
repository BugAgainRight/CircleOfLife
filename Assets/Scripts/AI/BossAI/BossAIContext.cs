using CircleOfLife.Battle;
using CircleOfLife.Buff;
using Milutools.AI;
using Milutools.Recycle;
using RuiRuiAstar;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using CircleOfLife.Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CircleOfLife
{
    public class BossAIContext : BehaviourContext, IBattleEntity, IAstarMove
    {

        public Image Front, Back;
        public TextMeshProUGUI NameShow;

        [HideInInspector]
        public bool IsDizzAttack;
        public SkeletonAnimation SkeletonAnimation;
        public Transform SkillOffset;
        public BattleRange ViewRadius;
        public LayerMask FriendLayer;
        public BattleStats.Stats FirstState, SecondState;
        public BattleStats Stats { get; set; }

        public FactionType FactionType => FactionType.Enemy;

        public bool NeedAstarMove { get; set; }
        public bool IsArrival { get; set; }
        public float Timer { get; set; }
        public int UpdateTime { get; set; }

        public Vector2 TargetPos
        {
            get
            {
                if (playerColl == null) return Vector2.zero;
                return playerColl.transform.position;
            }
        }

        public List<Vector2Int> path { get; set; }

        public Transform Transform => transform;

        public int Speed { get; set; }

        private Collider2D playerColl;
        [HideInInspector]
        public bool NearPlayer;
        [HideInInspector]
        public bool TargetIsPlayer;
        [HideInInspector]
        public Collider2D Target;

        public IAstarMove ThisAstarMove;
        [HideInInspector]
        public bool isSecondState;


        private float needResetTime = 0;
        private bool needReset = false;
        public bool TimerFinish
        {
            get
            {
                if (needReset && needResetTime < Time.time) timer = Time.time;
                if (timer + Stats.Current.AttackInterval <= Time.time)
                {
                    needResetTime = Time.time;
                    needReset = true;
                    return true;
                }
                needReset = false;
                return false;
            }
        }
        private float timer = 0;

        private void OnEnable()
        {
            IsDizzAttack = false;
            isSecondState = false;
            playerColl = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
            ThisAstarMove = this;
            Stats = FirstState.Build(gameObject,HurtAction, true);
            ThisAstarMove.OnEnableNew(1, Mathf.RoundToInt(FirstState.Velocity));
            Front.fillAmount = 1;
            Back.fillAmount = 1;
            NameShow.text = "BOSS";

        }

        private void HurtAction(BattleContext battleContext)
        {
            if (Stats.Current.Hp <= 0)
            {
                if (LevelManager.Instance)
                {
                    LevelManager.Instance.NotifyEnemyDeath(gameObject);
                }
                RecyclePool.ReturnToPool(gameObject);
            }else if (!isSecondState && Stats.Current.Hp<=SecondState.Hp)
            {
                isSecondState = true;

                NameShow.text = "BOSS第二阶段";

                Stats.ReplaceBaseStat(SecondState);
                ThisAstarMove.OnEnableNew(1, Mathf.RoundToInt(SecondState.Velocity));
                Debug.Log("进入第二阶段");

            }
        }
        private float dizzTimer;
        private float finishTime;
        public void ChangeAttackToDizz(float continueTime)
        {
            IsDizzAttack = true;
            finishTime = Time.time + continueTime;
        }
        public override void UpdateContext()
        {
            if (finishTime <= Time.time) IsDizzAttack = false;

            Front.fillAmount = Stats.Current.Hp / Stats.Max.Hp;
            Back.fillAmount = Mathf.MoveTowards(Back.fillAmount, Stats.Current.Hp / Stats.Max.Hp,
                (1+ Mathf.Abs(Stats.Current.Hp- Back.fillAmount)/ Stats.Max.Hp) *0.2f);




            if (playerColl == null) return;

            ThisAstarMove.UpdateSpeed(Mathf.RoundToInt(Stats.Current.Velocity));

            NearPlayer = Vector2.Distance(transform.position, playerColl.transform.position) < ViewRadius.Range.radius;

            if (!NearPlayer)
            {


                if (Target == null || Target == playerColl || !Target.gameObject.activeInHierarchy)
                {
                    List<Collider2D> mid = ViewRadius.GetAllEnemyInRange(FriendLayer, FactionType.Enemy);
                    if (mid.Count > 0)
                    {
                        Target = mid[0];
                        TargetIsPlayer = false;
                    }
                    else
                    {
                        Target = playerColl;
                        TargetIsPlayer = true;
                    }
                }
                else
                {
                    if(Vector2.Distance(transform.position, Target.transform.position) > ViewRadius.Range.radius+2)
                    {
                        TargetIsPlayer = true;
                        Target = playerColl;
                    }
                }
               
            }
            else
            {
                TargetIsPlayer = true;
                Target = playerColl;
            }

        }

        public void MoveToPlayer()
        {
            ThisAstarMove.FixedUpdateNew();
        }
        public int EnergyBar;
        private int nowEnergy;

        List<EnemySkillType> midSelect = new List<EnemySkillType>() {
                            EnemySkillType.BossSkill2,
                            EnemySkillType.BossSkill3,
                            EnemySkillType.BossSkill4,
                            EnemySkillType.BossSkill2,
                            EnemySkillType.BossSkill3,
                            EnemySkillType.BossSkill4,
                            EnemySkillType.BossSkill2,
                            EnemySkillType.BossSkill3,
                            EnemySkillType.BossSkill4,

                        };
        public void AttackTarget()
        {
            if (TimerFinish)
            {

                if (nowEnergy >= EnergyBar)
                {
                    nowEnergy = 0;

                    if (Stats.Current.Hp < Stats.Max.Hp / 2)
                        SkillManagement.GetSkill(EnemySkillType.BossSkill1)(
                            new SkillContext(FriendLayer, Stats)
                            {
                                FireTransform = SkillOffset
                            }
                        );
                    else
                    {
                        
                        int select = Random.Range(0, midSelect.Count);
                        SkillManagement.GetSkill(midSelect[select])(new SkillContext(FriendLayer, Stats)
                        {
                            FireTransform = SkillOffset
                        });
                    }
                }
                else
                {
                    Stats.ApplyBuff(BuffUtils.ToBuff(ChangeAttacker));
                    SkillManagement.GetSkill(EnemyStat.Boss)(new SkillContext(FriendLayer, Stats,Target.GetBattleStats())
                    {
                        FireTransform = SkillOffset
                    });
                    nowEnergy++;
                }

            }

        }

        public void ChangeAttacker(BattleStats stats, BuffContext buff)
        {
            stats.Current.Attack = 20 * (2 - stats.Current.Hp / stats.Max.Hp);
        }

       
    }
}
