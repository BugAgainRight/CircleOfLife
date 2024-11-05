using System;
using CircleOfLife.Battle;
using CircleOfLife.Buff;
using Milutools.Recycle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class TreatmentStation : BuildBase
    {
        public override List<LevelUpDirection> LevelUpDirections => new()
        {
            
        };

        public override int Level { get; protected set; }

        public override void LevelUp(Enum direction = null)
        {
            
        }
        public override void HurtAction(BattleContext context)
        {
            if (context.HitData.Current.Hp <= 0)
            {
                //更新寻路场

                RecyclePool.ReturnToPool(gameObject);
            }
        }

        private void Awake()
        {
            Stats = Attribute.Build(gameObject, HurtAction);
        }

        public LayerMask BattleEntityLayer;

        private void FixedUpdate()
        {
            if (TimerFinish)
            {
                SkillContext skillContext = new(BattleEntityLayer, Stats);
                         
                SkillManagement.GetSkill(BuildSkillType.TreatmentStation)(skillContext);
               
            }
        }

    }
}
