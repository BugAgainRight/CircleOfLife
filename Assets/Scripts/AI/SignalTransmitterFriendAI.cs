using CircleOfLife.Battle;
using Milutools.AI;
using Milutools.AI.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class SignalTransmitterFriendAI : BehaviourTree<BuildFriendContext>
    {
        public override IBehaviourNode Build(BuildFriendContext context)
        {
            return Selector(

                Condition(
                    c => c.HasTarget,
                    Selector(
                        Condition(c => c.FireDistance <= c.EnemyDistance, Action(MoveToEnemy)),
                        Condition(c => !context.TimerFinish, Action(MoveToFireEnemyPos)),
                        Condition(c => context.TimerFinish, Action(Fire))
                        )
                    ),
                 Condition(
                    c => !c.HasTarget,
                    Action(ReturnStandPos)
                    ),
                 Action(Idle)

                );


        }

        private BehaviourState ReturnStandPos(BuildFriendContext context)
        {
            if (Vector2.Distance(context.StandPos, gameObject.transform.position) < 0.005f)
                return BehaviourState.Succeed;

            transform.position = Vector2.MoveTowards(
                transform.position, context.StandPos,
                context.BattleStat.Current.Velocity * Time.fixedDeltaTime);

            return BehaviourState.Running;
        }

        private BehaviourState MoveToEnemy(BuildFriendContext context)
        {
            if (!context.HasTarget) return BehaviourState.Succeed;
            if (Vector2.Distance(transform.position, context.Enemy.transform.position)
                <= context.FireDistance - 0.005) return BehaviourState.Succeed;
            transform.position = Vector2.MoveTowards(
                transform.position, context.Enemy.transform.position,
                context.BattleStat.Current.Velocity * Time.fixedDeltaTime);
            return BehaviourState.Running;
        }


        private BehaviourState MoveToFireEnemyPos(BuildFriendContext context)
        {
            if (!context.HasTarget) return BehaviourState.Succeed;
            if (context.TimerFinish) return BehaviourState.Failed;
  
            Vector2 dir = (context.Enemy.transform.position - transform.position).normalized;
            Vector2 targetPos = (Vector2)context.Enemy.transform.position - dir * (context.FireDistance - 0.1f);
            transform.position = Vector2.MoveTowards(
              transform.position, targetPos, context.BattleStat.Current.Velocity * Time.fixedDeltaTime);
            return BehaviourState.Running;
        }


        private BehaviourState Idle(BuildFriendContext context)
        {
            return BehaviourState.Succeed;
        }

        private BehaviourState Fire(BuildFriendContext context)
        {
            Debug.Log("2");
            SkillContext skillContext = new(context.EnemyLayer, context.BattleStat, context.Enemy.GetBattleStats());

            SkillManagement.GetSkill(context.BuildSkill)(skillContext);
            return BehaviourState.Succeed;
        }
    }
}