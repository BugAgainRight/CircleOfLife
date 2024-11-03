using Milutools.AI;
using UnityEngine;

namespace Demos
{
    public class EnemyAIContext : BehaviourContext
    {
        public SpriteRenderer EnemyRender;
        
        public Transform Enemy;
        public Transform Player;
        
        public float DiscoverDistance = 6f;
        public float BattleDistance = 1f;
        public float SkillCD = 2f;
        public float WaitTime = 1f;

        public float MoveSpeed = 3f;
        
        public float PlayerDistance { get; private set; }
        private float skillTick = 0f;

        public void ResetSkillTick()
        {
            skillTick = 0f;
        }
        
        public bool IsSkillReady()
        {
            return skillTick >= SkillCD;
        }
        
        public override void UpdateContext()
        {
            if (skillTick < SkillCD)
            {
                skillTick += Time.fixedDeltaTime;
            }
            PlayerDistance = Vector2.Distance(Player.position, Enemy.position);
        }
    }
}
