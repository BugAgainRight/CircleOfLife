using System;
using CircleOfLife.Buff;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class BladeFencing : BuildBase
    {
        public override List<LevelUpDirection> LevelUpDirections => new()
        {
            
        };

        public override void HurtAction(BattleContext context)
        {
           
        }
        private void Awake()
        {
            Level = 1;
            NowType = BuildSkillType.BladeFencing;
            Stats = Attribute[0].Build(gameObject, HurtAction);
        }
        private void OnEnable()
        {
            Level = 1;
            NowType = BuildSkillType.BladeFencing;
            ReplaceStats(Attribute[0], true);

        }

        private void FixedUpdate()
        {
            RecoveryHP();
        }

        protected override void LevelUpFunc()
        {
            
        }
    }
}
