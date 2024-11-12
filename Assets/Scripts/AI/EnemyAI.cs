using CircleOfLife.AI;
using CircleOfLife.Battle;
using Milutools.AI;
using Milutools.AI.Nodes;
using Milutools.Recycle;
using RuiRuiVectorField;
using UnityEngine;

namespace CircleOfLife.AI
{
    public class EnemyAI : BehaviourTree<EnemyAIContext>
    {
        public override IBehaviourNode Build(EnemyAIContext context)
        {
            return Selector(
                Condition(
                    c => c.Target && c.Distance <= c.BattleDistance, 
                    Selector(
                            Condition(c => c.IsSkillReady(), Action(Cast)),
                            Condition(c => !c.IsSkillReady(), Action(Distancing))
                        )
                ),
                Condition(
                    c => c.Target, 
                    Sequence(Action(Chase))
                ),
                Action(Idle)
            );
        }
        
        private BehaviourState Cast(EnemyAIContext context)
        {
            if (context.TargetStats == null || !context.TargetStats.Transform)
            {
                return BehaviourState.Succeed;
            }
            context.ResetSkillTick();
            var skillContext = new SkillContext(context.LayerMask, context.Stats, context.TargetStats);
            skillContext.FireTransform = context.SkillOffset;
            SkillManagement.GetSkill(context.EnemyType)(skillContext);
            return BehaviourState.Succeed;
        }
        
        private BehaviourState Chase(EnemyAIContext context)
        {
            if (context.Distance <= context.BattleDistance)
            {
                return BehaviourState.Succeed;
            }
            
            context.Enemy.position =
                MoveTowards(context.Enemy.position, context.Target.position, 
                    context.Speed * Time.fixedDeltaTime);
            return BehaviourState.Running;
        }
        
        private BehaviourState Distancing(EnemyAIContext context)
        {
            if (context.IsSkillReady())
            {
                //context.LockDirection = false;
                return BehaviourState.Succeed;
            }

            /**context.LockDirection = true;
            context.Enemy.position =
                MoveTowards(context.Enemy.position, context.Player.position,
                    context.Speed * -0.1f * Time.fixedDeltaTime);**/
            return BehaviourState.Running;
        }
        
        private BehaviourState Idle(EnemyAIContext context)
        {
            if (context.Target)
            {
                return BehaviourState.Succeed;
            }
            //context.EnemyAnimator.state.SetAnimation(0, "idel", true);
            return BehaviourState.Running;
        }
        
        private Vector3 MoveTowards(Vector3 pos, Vector3 target, float speed)
        {
            var arc = Mathf.Atan2(target.y - pos.y, target.x - pos.x);
            pos.x += speed * Mathf.Cos(arc);
            pos.y += speed * Mathf.Sin(arc);
            return pos;
        }
    }
}
