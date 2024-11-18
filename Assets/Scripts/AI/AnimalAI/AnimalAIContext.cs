
using System.Collections.Generic;
using System.Linq;
using CircleOfLife.Battle;
using CircleOfLife.Buff;
using Milutools.AI;
using RuiRuiAstar;
using RuiRuiMathTool;
using RuiRuiVectorField;
using UnityEngine;
using UnityEngine.Events;


namespace CircleOfLife
{
    public class AnimalAIContext : BehaviourContext, IBattleEntity, IAstarMove
    {
        [HideInInspector]
        public Transform Player;
        [HideInInspector]
        public Transform Enemy;
        [HideInInspector]
        public Transform Animal;
        [HideInInspector]
        private Transform _target;
        public Transform Target
        {
            get
            {
                if (_target == null)
                {
                    _target = Player;
                }
                return _target;
            }
            set
            {
                _target = value;
            }
        }
        [HideInInspector]
        public BattleStats TargetStats
        {
            get
            {
                return Enemy.GetComponent<Collider2D>().GetBattleStats();
            }
        }
        public LayerMask EnemyLayer;
        public LayerMask FriendLayer;
        public BattleStats Stats { get; set; }
        public BattleStats.Stats Stat;
        public BehaviourTree<AnimalAIContext> BehaviourTree;
        public FactionType FactionType => FactionType.Friend;
        public AnimalStat AnimalType;
        public AnimalSkillType AnimalSkillType;
        public float PlayerDistance { get; private set; }
        public float EnemyDistance { get; private set; }
        public Transform SkillOffset;
        ///<summary>
        ///过于远时跑回玩家身边
        /// </summary>
        public float FarFromPlayerDinstance = 20f;
        /// <summary>
        /// 对玩家视野范围
        /// </summary> 
        public float DiscoverPlayerDistance = 5f;
        /// <summary>
        /// 移动到玩家的距离
        /// </summary> 
        public float MoveToPlayerDistance = 3f;

        public float DiscoverEnemyDistance = 6f;
        public float BattleDistance = 1f;
        public bool HasTarget
        {
            get
            {
                return Enemy != null;
            }
        }
        public float RunSpeed
        {
            get
            {
                return Stats.Current.Velocity * 2;
            }
        }
        private float skillTick = 0f;
        public bool IsAnimalDead = false;
        public float SleepTime = 60f;
        private float SleepTick = 0f;
        [HideInInspector]
        public UnityEvent OnAnimalDead = new UnityEvent();
        [HideInInspector]
        public UnityEvent OnAnimalAwake = new UnityEvent();
        public Collider2D Collider2D;
        #region 判断条件
        [HideInInspector]
        public bool IsVeryFarFromPlayer
        {
            get
            {
                return PlayerDistance > FarFromPlayerDinstance;
            }
        }
        public bool IsNearPlayer
        {
            get
            {
                return PlayerDistance < MoveToPlayerDistance;
            }
        }
        public bool IsPlayerOutOfDiscoverDistance
        {
            get
            {
                return PlayerDistance > DiscoverPlayerDistance;
            }
        }
        public bool IsFindEnemy
        {
            get
            {
                return EnemyDistance < DiscoverEnemyDistance;
            }
        }
        public bool IsEnemyInBattaleDistance
        {
            get
            {
                return EnemyDistance < BattleDistance;
            }
        }



        #endregion


        #region IAStarMove
        public bool NeedAstarMove { get; set; } = false;
        public bool IsArrival { get; set; }
        public float Timer { get; set; }
        public int UpdateTime { get; set; } = 1;
        public Vector2 TargetPos => Target.position;

        public List<Vector2Int> path { get; set; }

        public Transform Transform => Animal;
        public int Speed { get; set; }

        public void ResumeAStarMove()
        {
            ((IAstarMove)this).OnEnableNew(1, (int)Stats.Current.Velocity);
        }

        public void CloseAStarMove() => ((IAstarMove)this).CloseAstarMove();
        #endregion
        #region update

        private void Awake()
        {
            Animal = transform;
            Stats = Stat.Build(this.gameObject, (context) =>
            {
                if (Stats.Current.Hp <= 0f)
                {
                    Stats.Current.Hp = -0.01f;
                    IsAnimalDead = true;
                    SleepTick = 0;
                    IsArrival = true;
                    Collider2D.enabled = false;
                }
            });
            FindPlayer();
        }

        private void OnEnable()
        {
            Stats.ReplaceBaseStat(Stat);
            if (Collider2D == null)
            {
                Collider2D = GetComponent<Collider2D>();
            }
            //((IAstarMove)this).OnEnableNew(1, (int)Stats.Current.Velocity);
            BehaviourTree.Start();
        }
        public override void UpdateContext()
        {
            FindPlayer();
            Enemy = FindNearestEnemy();
            if (Enemy != null)
            {
                EnemyDistance = Vector2.Distance(Animal.position, Enemy.position);
            }
            else
            {
                EnemyDistance = float.MaxValue;
            }
            CheckSkill();
            AnimalDeadHander();
            ((IAstarMove)this).FixedUpdateNew();
        }
        public void UseSkill()
        {
            skillTick = 0f;
        }

        public bool IsSkillReady
        {
            get
            {
                return skillTick >= Stats.Current.AttackInterval;
            }
        }

        private void FindPlayer()
        {
            if (Player == null)
            {
                Player = GameObject.FindGameObjectWithTag("Player").transform;
            }
            PlayerDistance = Vector2.Distance(Animal.position, Player.position);
        }


        private Transform FindNearestEnemy()
        {
            if (Enemy != null && Enemy.gameObject.activeSelf && EnemyDistance < BattleDistance)
            {
                return Enemy;
            }
            List<Collider2D> mid = Physics2D.OverlapCircleAll(Animal.position, DiscoverEnemyDistance, EnemyLayer).ToList();
            mid.RemoveAll(x =>
            {
                var battleEntity = x.GetBattleStats();
                if (battleEntity == null) return true;

                if (battleEntity.BattleEntity.FactionType.Equals(FactionType.Friend)) return true;
                return false;
            });
            Transform enemy = null;
            if (mid.Count > 0)
            {
                float minDictance = Vector2.Distance(Animal.position, mid[0].transform.position);
                enemy = mid[0].transform;
                foreach (var enemyCol in mid)
                {
                    if (Vector2.Distance(Animal.position, enemy.transform.position) < minDictance)
                    {
                        minDictance = Vector2.Distance(Animal.position, enemy.transform.position);
                        enemy = enemyCol.transform;
                    }
                }
                return mid[0].transform;
            }
            return enemy;
        }
        private void CheckSkill()
        {
            if (skillTick < Stats.Current.AttackInterval)
            {
                skillTick += Time.fixedDeltaTime;
            }
        }
        private void AnimalDeadHander()
        {
            if (IsAnimalDead)
            {
                if (SleepTick < SleepTime)
                {
                    //Debug.Log("Animal will awake in " + (SleepTime - SleepTick) + "s:\ncurrentHp:" + Stats.Current.Hp);
                    SleepTick += Time.fixedDeltaTime;
                }
                else
                {
                    Debug.Log("Animal Awake");
                    IsAnimalDead = false;
                    IsArrival = false;
                    OnEnable();
                    Collider2D.enabled = true;
                    OnAnimalAwake.Invoke();
                }
            }
        }
        #endregion
        #region 其他方法
        public void ChangeMoveTarget(Transform target)
        {
            Target = target;
        }
        #endregion
    }
}
