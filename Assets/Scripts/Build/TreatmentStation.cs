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
