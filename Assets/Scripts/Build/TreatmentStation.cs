using CircleOfLife.Battle;
using CircleOfLife.Buff;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class TreatmentStation : BuildBase
    {
        public override void HurtAction(BattleStats battleStats)
        {
            if (battleStats.Current.Hp <= 0)
            {
                //更新寻路场
            }
        }

        private void Awake()
        {
            Stats = Attribute.Build(gameObject, HurtAction);
        }


        public GizmosSetting RecoveryRange;
        public LayerMask BattleEntityLayer;

        private void FixedUpdate()
        {
            if (TimerFinish)
            {
                SkillContext skillContext = new()
                {
                    TriggerObj=gameObject,
                    RangeSetting=RecoveryRange,
                    PhysicsLayer=BattleEntityLayer,
                    FactionType=FactionType,
                    AttackerData=Stats
                };               
                SkillManagement.GetSkill(BuildSkillType.TreatmentStation)(skillContext);
               
            }
        }

    }
}
