using CircleOfLife.Battle;
using CircleOfLife.Buff;
using Milutools.AI;
using Milutools.AI.Nodes;
using UnityEngine;

namespace CircleOfLife.AI
{
    public class WildYak : AnimalAI
    {
        bool IsBuffApply = false;
        public override IBehaviourNode Build(AnimalAIContext context)
        {
            return Selector(
                Condition(c => !IsBuffApply, Action(ApplyBuffToSelf)),
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

        protected BehaviourState ApplyBuffToSelf(AnimalAIContext context)
        {

            IsBuffApply = true;
            BattleStats stats = context.Stats;
            if (stats == null)
            {
                return BehaviourState.Succeed;
            }
            stats.ApplyBuff(BuffUtils.ToBuff(WildYakAttackBuff, 3000));
            //Debug.Log("应用buff");
            return BehaviourState.Succeed;
        }
        private static void WildYakAttackBuff(BattleStats stats, BuffContext buff)
        {
            stats.Current.Attack *= 1 - stats.Current.Hp / stats.Max.Hp;
            //Debug.Log("WildYak Attack Up to:" + stats.Current.Attack);
        }
    }
}
