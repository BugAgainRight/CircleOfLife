using Milutools.AI;
using Milutools.AI.Nodes;
using Milutools.Recycle;
using UnityEngine;

namespace Demos
{
    public class EnemyAI : BehaviourTree<EnemyAIContext>
    {
        public override IBehaviourNode Build(EnemyAIContext context)
        {
            return Selector(
                Condition(
                    c => c.PlayerDistance <= c.BattleDistance, 
                    Selector(
                            Condition(c => c.IsSkillReady(), Action(Cast)),
                            Condition(c => !c.IsSkillReady(), Action(Distancing))
                        )
                ),
                Condition(
                    c => c.PlayerDistance <= c.DiscoverDistance, 
                    Sequence(Action(Angry), Wait(context.WaitTime), Action(Chase))
                ),
                Action(Idle)
            );
        }
        
        private BehaviourState Angry(EnemyAIContext context)
        {
            context.EnemyRender.color = Color.red;
            return BehaviourState.Succeed;
        }
        
        private BehaviourState Cast(EnemyAIContext context)
        {
            context.ResetSkillTick();
            context.EnemyRender.color = new Color(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f));
            return BehaviourState.Succeed;
        }
        
        private BehaviourState Chase(EnemyAIContext context)
        {
            if (context.PlayerDistance <= context.BattleDistance)
            {
                return BehaviourState.Succeed;
            }
            
            context.Enemy.position =
                MoveTowards(context.Enemy.position, context.Player.position, 
                    context.MoveSpeed * Time.fixedDeltaTime);
            return BehaviourState.Running;
        }
        
        private BehaviourState Distancing(EnemyAIContext context)
        {
            if (context.IsSkillReady())
            {
                return BehaviourState.Succeed;
            }
            
            context.Enemy.position =
                MoveTowards(context.Enemy.position, context.Player.position, 
                    context.MoveSpeed * -0.2f * Time.fixedDeltaTime);
            return BehaviourState.Running;
        }
        
        private BehaviourState Idle(EnemyAIContext context)
        {
            context.Enemy.localEulerAngles += new Vector3(0f, 0f, 360f * Time.fixedDeltaTime);
            return BehaviourState.Succeed;
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
