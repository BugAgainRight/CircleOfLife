using System.Collections;
using System.Collections.Generic;
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
                Condition(c => c.IsPlayerOutOfDiscoverDistance && !c.IsEnemyInBattaleDistance, Action(RunToPlayer)),
                Action(Idle));
        }


        private BehaviourState UseSkill(AnimalAIContext context)
        {
            if (context.IsAnimalDead)
            {
                Stop();
            }
            if (!context.IsUsingSkill && context.IsSkillReady)
            {
                context.UseSkill();
            }
            if (context.IsUsingSkill)
            {
                //Debug.Log("Skill has been used " + context.skillTimeTick + " s");
                context.Animal.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f));
                return BehaviourState.Running;
            }
            else
            {
                context.Animal.GetComponent<SpriteRenderer>().color = Color.white;
                return BehaviourState.Succeed;
            }
        }

        private BehaviourState Idle(AnimalAIContext context)
        {
            context.Animal.localEulerAngles += new Vector3(0f, 0f, 360f * Time.fixedDeltaTime);
            return BehaviourState.Succeed;
        }

        private BehaviourState Sleep(AnimalAIContext context)
        {
            context.Animal.GetComponent<SpriteRenderer>().color = Color.black;
            if (context.IsAnimalDead)
            {
                return BehaviourState.Running;
            }
            context.Animal.GetComponent<SpriteRenderer>().color = Color.white;
            return BehaviourState.Succeed;
        }

        //行动:当玩家离自身过于远时,向玩家移动,在靠近玩家后取消
        private BehaviourState RunToPlayer(AnimalAIContext context)
        {
            if (context.IsAnimalDead)
            {
                Stop();
            }
            if (context.IsNearPlayer)
            {
                return BehaviourState.Succeed;
            }
            context.Animal.position = MoveTowards(context.Animal.position, context.Player.position, context.RunSpeed);
            return BehaviourState.Running;
        }
        //行动:当自身发现敌人后，追击敌人,敌人进入攻击范围或者离开视野范围后取消
        private BehaviourState MoveToEnemy(AnimalAIContext context)
        {
            if (context.IsAnimalDead)
            {
                Stop();
            }
            if (!context.IsFindEnemy
            || context.IsEnemyInBattaleDistance
            || context.IsVeryFarFromPlayer)
            {
                return BehaviourState.Succeed;
            }
            context.Animal.position = MoveTowards(context.Animal.position, context.Enemy.position, context.MoveSpeed);
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
