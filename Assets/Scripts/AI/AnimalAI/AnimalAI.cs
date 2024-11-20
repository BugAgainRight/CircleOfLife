using CircleOfLife.Battle;
using Milutools.AI;
using Milutools.AI.Nodes;
using UnityEngine;

namespace CircleOfLife
{
    public class AnimalAI : BehaviourTree<AnimalAIContext>
    {
        //状态1:跟随玩家(此状态下，比如每相距玩家10米，移动至相距玩家5米处)
        //状态2:攻击敌人
        //状态3:休眠
        //行动3:当玩家离自身过于远时,向玩家移动,在靠近玩家后取消
        //行动4:当玩家距离自身过于远时，
        public override IBehaviourNode Build(AnimalAIContext context)
        {
            return Selector(
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


        private BehaviourState UseSkill(AnimalAIContext context)
        {
            //Debug.Log("UseSkill");
            if (context.Enemy == null || context.TargetStats == null || !context.TargetStats.Transform)
            {
                return BehaviourState.Succeed;
            }
            context.UseSkill();
            var skillContext = new SkillContext(context.EnemyLayer, context.Stats, context.TargetStats);
            skillContext.FireTransform = context.SkillOffset;
            skillContext.SpecialValues.Add(context.SkillBuffProbability);
            skillContext.SpecialValues.Add(context.SkillBuffDuration);
            SkillManagement.GetSkill(context.AnimalSkillType)(skillContext);
            return BehaviourState.Succeed;
        }

        private BehaviourState Idle(AnimalAIContext context)
        {
            //context.Animal.localEulerAngles += new Vector3(0f, 0f, 360f * Time.fixedDeltaTime);
            return BehaviourState.Succeed;
        }

        private BehaviourState Sleep(AnimalAIContext context)
        {
            //Debug.Log("Sleep");
            if (context.IsAnimalDead)
            {
                return BehaviourState.Running;
            }
            return BehaviourState.Succeed;
        }

        //行动:当玩家离自身过于远时,向玩家移动,在靠近玩家后取消
        private BehaviourState RunToPlayer(AnimalAIContext context)
        {
            //启用寻路
            if (!context.NeedAstarMove || context.IsArrival)
            {
                context.ChangeMoveTarget(context.Player);
                context.ResumeAStarMove();
            }
            //禁用寻路
            if (context.IsNearPlayer)
            {
                context.IsArrival = true;
                context.CloseAStarMove();
                return BehaviourState.Succeed;
            }
            //context.Animal.position = MoveTowards(context.Animal.position, context.Player.position, context.RunSpeed);
            return BehaviourState.Running;
        }
        //行动:当自身发现敌人后，追击敌人,敌人进入攻击范围或者离开视野范围后取消
        private BehaviourState MoveToEnemy(AnimalAIContext context)
        {
            //启用寻路
            if (!context.NeedAstarMove || context.IsArrival)
            {
                context.ChangeMoveTarget(context.Enemy);
                context.ResumeAStarMove();
            }
            //禁用寻路
            if (!context.IsFindEnemy
            || context.IsEnemyInBattaleDistance
            || context.IsVeryFarFromPlayer)
            {
                context.IsArrival = true;
                context.CloseAStarMove();
                return BehaviourState.Succeed;
            }
            //context.Animal.position = MoveTowards(context.Animal.position, context.Enemy.position, context.Stats.Current.Velocity);
            return BehaviourState.Running;
        }

        private Vector3 MoveTowards(Vector3 pos, Vector3 target, float speed)
        {
            var arc = Mathf.Atan2(target.y - pos.y, target.x - pos.x);
            pos.x += speed * Mathf.Cos(arc) * Time.fixedDeltaTime;
            pos.y += speed * Mathf.Sin(arc) * Time.fixedDeltaTime;
            return pos;
        }

    }
}
