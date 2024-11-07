using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CircleOfLife.Battle;
using Milutools.AI;
using UnityEngine;
using UnityEngine.Events;

namespace CircleOfLife
{
    public class AnimalAIContext : BehaviourContext
    {

        public Transform Player;
        public Transform Enemy;
        public Transform Animal;
        public LayerMask EnemyLayer;
        public LayerMask PlayerLayer;
        public BattleStats AnimalBattaleStats;
        public float PlayerDistance { get; private set; }
        public float EnemyDistance { get; private set; }

        ///<summary>
        ///过于远时跑回玩家身边
        /// </summary>
        public float FarFromPlayerDinstance = 20f;
        /// <summary>
        /// 对玩家视野范围
        /// </summary> 
        public float DiscoverPlayerDistance = 3f;
        public float DiscoverEnemyDistance = 6f;
        public float BattleDistance = 1f;
        public bool HasTarget = false;
        public float MoveSpeed = 3f;
        public float RunSpeed = 5f;
        public float SkillCD = 10f;
        private float skillTick = 0f;
        public float SkillTime = 2f;
        public float skillTimeTick = 0f;

        public bool IsAnimalDead = false;
        public float SleepTime = 60f;
        private float SleepTick = 0f;
        [HideInInspector]
        public UnityEvent OnAnimalDead = new UnityEvent();
        [HideInInspector]
        public UnityEvent OnAnimalAwake = new UnityEvent();
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
                return PlayerDistance < DiscoverPlayerDistance;
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
        public bool IsPlayerOutOfDiscoverDistance
        {
            get
            {
                return PlayerDistance > DiscoverPlayerDistance;
            }
        }


        #endregion

        public void UseSkill()
        {
            IsUsingSkill = true;
            skillTick = 0f;
            skillTimeTick = 0f;
        }

        public bool IsSkillReady
        {
            get
            {
                return skillTick >= SkillCD;
            }
        }
        public bool IsUsingSkill = false;
        public override void UpdateContext()
        {
            FindPlayer();
            FindEnemy();
            CheckSkill();
            AnimalDeadHander();
        }

        #region update
        private void FindPlayer()
        {
            if (Player == null)
            {
                Player = GameObject.FindGameObjectWithTag("Player").transform;
            }
            PlayerDistance = Vector2.Distance(Animal.position, Player.position);
        }
        private void FindEnemy()
        {
            if (Enemy == null || Enemy.gameObject.activeInHierarchy == false ||
                Vector2.Distance(Animal.position, Enemy.transform.position) > DiscoverEnemyDistance) HasTarget = false;
            if (!HasTarget)
            {
                List<Collider2D> mid = Physics2D.OverlapCircleAll(Animal.position, DiscoverEnemyDistance, EnemyLayer).ToList();
                /*mid.RemoveAll(x =>
                {
                    var battleEntity = x.GetBattleStats();
                    if (battleEntity == null) return true;

                    if (battleEntity.BattleEntity.FactionType.Equals(FactionType.Enemy)) return true;
                    return false;
                });*/
                if (mid.Count > 0)
                {
                    HasTarget = true;
                    Enemy = mid[0].transform;
                }
                else HasTarget = false;
            }
            if (Enemy != null) EnemyDistance = Vector2.Distance(Animal.position, Enemy.transform.position);
        }


        private void CheckSkill()
        {
            if (IsUsingSkill)
            {
                if (skillTimeTick < SkillTime)
                {
                    skillTimeTick += Time.fixedDeltaTime;
                }
                else
                {
                    //Debug.Log("Skill End");
                    skillTimeTick = 0;
                    IsUsingSkill = false;
                }
            }
            if (skillTick < SkillCD)
            {
                skillTick += Time.fixedDeltaTime;
            }
        }
        private void AnimalDeadHander()
        {
            if (!IsAnimalDead && AnimalBattaleStats.Current.Hp <= 0)
            {
                IsAnimalDead = true;
                OnAnimalDead.Invoke();
            }
            if (IsAnimalDead)
            {
                if (SleepTick < SleepTime)
                {
                    Debug.Log("Animal will awake in " + (SleepTime - SleepTick) + "s:\ncurrentHp:" + AnimalBattaleStats.Current.Hp);
                    SleepTick += Time.fixedDeltaTime;
                }
                else
                {
                    Debug.Log("Animal Awake");
                    IsAnimalDead = false;
                    SleepTick = 0;
                    OnAnimalAwake.Invoke();
                }
            }
        }
        #endregion
    }
}
