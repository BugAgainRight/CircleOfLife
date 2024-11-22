using Milutools.AI;
using Milutools.AI.Nodes;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class BossAI : BehaviourTree<BossAIContext>
    {
        public override IBehaviourNode Build(BossAIContext context)
        {
            return Selector(
                Condition(
                    c=>c.TargetIsPlayer,
                    Selector(
                        Condition(c=> c.NearPlayer, Action(Fire)),
                        Condition(c=>!c.NearPlayer,Action(Move))
                        )

                    ),
                Condition(
                    c=> !c.TargetIsPlayer&&c.Target!=null&&c.Target.gameObject.activeInHierarchy,
                    Action(Fire)
                    ),
                Action(Move)
                
                );
        }

        private BehaviourState Fire(BossAIContext context)
        {
            ChangeAnim(context.SkeletonAnimation, false);
            context.AttackTarget();
            return BehaviourState.Succeed;
        }
        private Vector2 prePos;
        private BehaviourState Move(BossAIContext context)
        {
            ChangeAnim(context.SkeletonAnimation, true);
            context.MoveToPlayer();
            transform.localScale = new Vector3(-Mathf.Sign(transform.position.x - prePos.x) * 0.4f, 0.4f, 1);
            prePos = transform.position;
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
