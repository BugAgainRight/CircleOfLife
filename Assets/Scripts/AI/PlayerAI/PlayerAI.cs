using System.Collections;
using System.Collections.Generic;
using Milutools.AI;
using Milutools.AI.Nodes;
using UnityEngine;

namespace CircleOfLife
{
    public class PlayerAI : BehaviourTree<PlayerAIContext>
    {
        public override IBehaviourNode Build(PlayerAIContext context)
        {
            return Condition(c => true, Action(Idle));
        }
        private BehaviourState Idle(PlayerAIContext context)
        {
            return BehaviourState.Succeed;
        }
    }
}
