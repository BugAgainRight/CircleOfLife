using CircleOfLife.Battle;
using Milutools.AI;
using Milutools.AI.Nodes;
using Spine.Unity;
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
                        //Condition(c => !context.TimerFinish, Action(MoveToFireEnemyPos)),
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
                return BehaviourState.Failed;
            ChangeAnim(context.SkeletonAnimation, true);
            transform.position = Vector2.MoveTowards(
                transform.position, context.StandPos,
                context.BattleStat.Current.Velocity * Time.fixedDeltaTime);
            transform.localScale = new Vector3(Mathf.Sign(context.StandPos.x - transform.position.x) * 0.4f, 0.4f, 1);
            return BehaviourState.Running;
        }

        private BehaviourState MoveToEnemy(BuildFriendContext context)
        {
            if (!context.HasTarget) return BehaviourState.Succeed;
            if (Vector2.Distance(transform.position, context.Enemy.transform.position)
                <= context.FireDistance - 0.005) return BehaviourState.Succeed;
            ChangeAnim(context.SkeletonAnimation, true);
            transform.position = Vector2.MoveTowards(
                transform.position, context.Enemy.transform.position,
                context.BattleStat.Current.Velocity * Time.fixedDeltaTime);
            transform.localScale = new Vector3(Mathf.Sign(context.Enemy.transform.position.x - transform.position.x) * 0.4f, 0.4f, 1);
            return BehaviourState.Running;
        }


        private BehaviourState MoveToFireEnemyPos(BuildFriendContext context)
        {
            if (!context.HasTarget) return BehaviourState.Succeed;
            if (context.TimerFinish) return BehaviourState.Failed;

            ChangeAnim(context.SkeletonAnimation, true);
            Vector2 dir = (context.Enemy.transform.position - transform.position).normalized;
            Vector2 targetPos = (Vector2)context.Enemy.transform.position - dir * (context.FireDistance - 0.1f);
            transform.position = Vector2.MoveTowards(
              transform.position, targetPos, context.BattleStat.Current.Velocity * Time.fixedDeltaTime);
            transform.localScale = new Vector3(Mathf.Sign(targetPos.x - transform.position.x) * 0.4f, 0.4f, 1);

            return BehaviourState.Running;
        }


        private BehaviourState Idle(BuildFriendContext context)
        {
            
            ChangeAnim(context.SkeletonAnimation, false);
            return BehaviourState.Succeed;
        }

        private BehaviourState Fire(BuildFriendContext context)
        {
            ChangeAnim(context.SkeletonAnimation, false);
            SkillContext skillContext = new(context.EnemyLayer, context.BattleStat, context.Enemy.GetBattleStats());
            skillContext.FireTransform = context.SkillOffset;
            SkillManagement.GetSkill(context.BuildSkill)(skillContext);
            return BehaviourState.Succeed;
        }


        private bool isRun;
        private void ChangeAnim(SkeletonAnimation skeletonAnimation, bool isRun)
        {
            if (isRun && this.isRun) ;
            else if (isRun) skeletonAnimation.state.SetAnimation(0, "run", true);
            else if (!this.isRun) ;
            else skeletonAnimation.state.SetAnimation(0, "idel", true);
            this.isRun = isRun;
        }

    }
}
