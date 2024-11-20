using CircleOfLife.Battle;
using CircleOfLife.Buff;
using Milutools.AI;
using Milutools.AI.Nodes;
using UnityEngine;

namespace CircleOfLife.AI
{
    public class FalcoCherrug : AnimalAI
    {
        bool IsBuffApply = false;
        public override IBehaviourNode Build(AnimalAIContext context)
        {
            return Selector(
                Condition(c => c.IsAnimalDead, Action(Sleep)),
                Condition(c => !IsBuffApply, Action(ApplyBuffToPlayer)),
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
            stats.ApplyBuff(BuffUtils.ToBuff(FalcoCherrug_PlayerRangeUpBuff, 3000));

            //Debug.Log("应用buff");
            return BehaviourState.Succeed;
        }


        private static void FalcoCherrug_PlayerRangeUpBuff(BattleStats stats, BuffContext buff)
        {
            stats.Current.EffectRange += BuffConsts.EFFECT_RANGE_UNIT;
            //Debug.Log("Player Range Up to:" + stats.Current.EffectRange);
        }
    }
}
