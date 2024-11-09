using System;
using CircleOfLife.Battle;
using CircleOfLife.Buff;
using CircleOfLife.Units;
using Milutools.AI;
using Milutools.Recycle;
using RuiRuiSTL;
using RuiRuiVectorField;
using Spine.Unity;
using UnityEngine;

namespace CircleOfLife.AI
{
    public class EnemyAIContext : BehaviourContext, IVectorFieldMove, IBattleEntity
    {
        private const float FIND_TARGET_INTERVAL = 0.2f;
        public bool NeedVectorFieldMove { get; set; }
        public bool IsArrival { get; set; }
        public Transform Transform => Enemy;
        public int Speed
        {
            get => Mathf.RoundToInt(Stats.Current.Velocity);
            set {}
        }

        public BattleStats Stats { get; set; }
        public FactionType FactionType => FactionType.Enemy;
        public LayerMask LayerMask;

        public SkeletonAnimation EnemyAnimator;
        public EnemyStat EnemyType;
        public bool FocusBuildingOnly = false;

        public BattleStats.Stats Stat;
        
        [HideInInspector]
        public Transform Enemy, Player;
        [HideInInspector]
        public Transform Target;
        [HideInInspector]
        public BattleStats TargetStats;

        public float StopBattleDistance = 8f;
        public float DiscoverDistance = 4f;
        public float BattleDistance = 1f;
        public float Distance { get; private set; }
        
        private float skillTick = 0f;
        private float findTargetTick = 0f;

        private Vector3 lastPos;
        private bool lastRun = false;
        private bool lastFaceLeft;
        
        private void Awake()
        {
            Enemy = transform;
            Stats = Stat.Build(gameObject, (context) =>
            {
                if (context.AttackerData != null)
                {
                    Target = context.AttackerData.Transform;
                }
                if (Stats.Current.Hp <= 0f)
                {
                    RecyclePool.ReturnToPool(gameObject);
                }
            });
        }

        private void Start()
        {
            Player = PlayerController.Instance.transform;
        }

        private void OnEnable()
        {
            ((IVectorFieldMove)this).OnEnableNew(Speed);
        }

        public void ResetSkillTick()
        {
            skillTick = 0f;
        }
        
        public bool IsSkillReady()
        {
            return skillTick >= Stats.Current.AttackInterval;
        }

        private void UpdateTarget()
        {
            if (!Target || !Target.gameObject.activeSelf || Distance > StopBattleDistance)
            {
                Distance = 0f;
                Target = null;
            }

            if (findTargetTick < FIND_TARGET_INTERVAL)
            {
                findTargetTick += Time.fixedDeltaTime;
                return;
            }

            if (Target)
            {
                return;
            }
            
            findTargetTick -= FIND_TARGET_INTERVAL;

            if (!FocusBuildingOnly && Vector2.Distance(Player.position, Enemy.position) <= DiscoverDistance)
            {
                Target = Player;
                TargetStats = PlayerController.Instance.Stats;
                return;
            }
            
            var colliders =
                Physics2D.OverlapCircleAll(Transform.position, BattleDistance, LayerMask);
            foreach (var col in colliders)
            {
                var stat = col.GetBattleStats();
                if (stat != null && stat.BattleEntity.FactionType != FactionType)
                {
                    if (FocusBuildingOnly && (stat.BattleEntity is not BuildBase))
                    {
                        continue;
                    }
                    TargetStats = stat;
                    Target = stat.Transform;
                    break;
                }
            }
        }
        
        public override void UpdateContext()
        {
            if (skillTick < Stats.Current.AttackInterval)
            {
                skillTick += Time.fixedDeltaTime;
            }
            UpdateTarget();
            Distance = !Target ? 0f : Vector2.Distance(Target.position, Enemy.position);
            NeedVectorFieldMove = !Target;

            var running = lastPos != Transform.position;

            if (running)
            {
                if (Mathf.Approximately(Transform.position.x, lastPos.x))
                {
                    return;
                }
                var faceLeft = Transform.position.x < lastPos.x;
                if (lastFaceLeft != faceLeft)
                {
                    lastFaceLeft = faceLeft;
                    var scale = Transform.localScale;
                    scale.x = Mathf.Abs(scale.x) * (lastFaceLeft ? 1f : -1f);
                    Transform.localScale = scale;
                }
            }
            lastPos = Transform.position;
            
            if (lastRun != running)
            {
                lastRun = running;
                EnemyAnimator.state.SetAnimation(0, running ? "run" : "idel", true);
            }
            
            ((IVectorFieldMove)this).FixedUpdateNew();
        }
    }
}
