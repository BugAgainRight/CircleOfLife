using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Milutools.AI;
using Milutools.AI.Nodes;
using CircleOfLife.Battle;

namespace CircleOfLife
{
    public class TestBuildFriend : BehaviourTree<BuildFriendContext>
    {
        public override IBehaviourNode Build(BuildFriendContext context)
        {
            return Selector(

                Condition(
                    c => c.HasTarget,
                    Selector(
                        Condition(c => c.FireDistance <= c.EnemyDistance, Action(MoveToEnemy)),
                        Condition(c => context.TimerFinish, Action(Fire))
                        )


                    ),
                 Condition(
                    c => !c.HasTarget,
                    Action(ReturnStandPos)


                    ),
                 Action(Idle)

                ) ;


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
            if (Vector2.Distance(transform.position, context.Enemy.transform.position) 
                <=context.FireDistance+0.005)return BehaviourState.Succeed;

            transform.position = Vector2.MoveTowards(
                transform.position, context.Enemy.transform.position,
                context.BattleStat.Current.Velocity * Time.fixedDeltaTime);

            return BehaviourState.Running;
        }

        private BehaviourState Idle(BuildFriendContext context)
        {
            return BehaviourState.Succeed;
        }

        private BehaviourState Fire(BuildFriendContext context)
        {
          
            SkillContext skillContext = new()
            {
                HitData=context.Enemy.GetComponent<IBattleEntity>().Stats,
                AttackerData = context.BattleStat,
                TargetObj = context.Enemy,
                TriggerObj = gameObject,
                FactionType = FactionType.Friend,
                TriggerPos=transform.position
            };
            SkillManagement.GetSkill(BuildSkillType.TestBuildFriendFire)(skillContext);
            return BehaviourState.Succeed;
        }
    }
}
