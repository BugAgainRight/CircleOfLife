using System.Collections;
using System.Collections.Generic;
using CircleOfLife.Battle;
using CircleOfLife.Buff;
using Milutools.AI;
using Milutools.AI.Nodes;
using UnityEngine;

namespace CircleOfLife.AI
{
    public class TibetanAntelope : AnimalAI
    {
        bool IsBuffApply = false;
        public override IBehaviourNode Build(AnimalAIContext context)
        {
            return Selector(
                Condition(c => !IsBuffApply, Action(ApplyBuffToPlayer)),
                Condition(c => c.IsAnimalDead, Action(Sleep)),
                Condition(c => c.IsVeryFarFromPlayer, Action(RunToPlayer)),
                Condition(c => c.HasTarget && c.IsFindEnemy && !c.IsEnemyInBattaleDistance, Action(MoveToEnemy)),
                Condition(
                    c => c.HasTarget && c.IsEnemyInBattaleDistance,
                Selector(
                    Condition(c => c.IsSkillReady, Action(UseSkill))
                    )),
                Condition(c => c.IsPlayerOutOfDiscoverDistance && !c.IsFindEnemy, Action(RunToPlayer)),
                Action(Idle));
        }

        protected BehaviourState ApplyBuffToPlayer(AnimalAIContext context)
        {

            IsBuffApply = true;
            BattleStats stats = context.Player.GetComponent<Collider2D>().GetBattleStats();
            if (stats == null)
            {
                return BehaviourState.Succeed;
            }
            stats.ApplyBuff(BuffUtils.ToBuff(TibetanAntelope_PlayerSpeedUpBuff, 3000));
            //Debug.Log("应用buff");
            return BehaviourState.Succeed;
        }
        private static void TibetanAntelope_PlayerSpeedUpBuff(BattleStats stats, BuffContext buff)
        {
            stats.Current.Velocity += BuffConsts.SPEED_UNIT;
            //Debug.Log("Player Speed Up to:" + stats.Current.Velocity);
        }
    }
}
