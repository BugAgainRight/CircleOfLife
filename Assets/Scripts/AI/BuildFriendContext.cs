using CircleOfLife.Battle;
using Milutools.AI;
using Milutools.Recycle;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CircleOfLife
{
    public class BuildFriendContext : BehaviourContext
    {
        [HideInInspector]
        /// <summary>
        /// 站桩地点
        /// </summary>
        public Vector2 StandPos;

        /// <summary>
        /// 视野范围
        /// </summary>
        public GizmosSetting ViewRadius;

        public LayerMask EnemyLayer;

        public BattleStats BattleStat;

        public float FireDistance;

        public int MaxCount;

        

        [HideInInspector]
        public Collider2D Enemy;

        [HideInInspector]
        public bool HasTarget=false;
        
        [HideInInspector]
        public float EnemyDistance;

       
        

        
        

        public bool TimerFinish
        {
            get
            {
                if (timer + BattleStat.Current.AttackInterval< Time.time)
                {
                    timer = Time.time;
                    return true;
                }
                return false;

            }
        }
        private float timer;


        public override void UpdateContext()
        {

            if (Enemy == null || Enemy.gameObject.activeInHierarchy == false ||
                Vector2.Distance(StandPos, Enemy.transform.position) > ViewRadius.radius) HasTarget = false;

            if (!HasTarget)
            {
                List<Collider2D> mid = Physics2D.OverlapCircleAll(StandPos, ViewRadius.radius, EnemyLayer).ToList();
                mid.RemoveAll(x =>
                {  
                    IBattleEntity battleEntity = x.GetBattleStats().BattleEntity;
                    if (battleEntity == null) return true;
                    if (battleEntity.FactionType.Equals(FactionType.Friend)) return true;
                    return false;
                });
                if (mid.Count>0)
                {
                    HasTarget = true;
                    Enemy = mid[0];
                }
                else HasTarget = false;
            }

            if (Enemy != null) EnemyDistance = Vector2.Distance(transform.position, Enemy.transform.position);






        }
    }
}
