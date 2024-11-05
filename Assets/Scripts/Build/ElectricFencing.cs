using System;
using CircleOfLife.Battle;
using CircleOfLife.Buff;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class ElectricFencing : BuildBase
    {
        public override List<LevelUpDirection> LevelUpDirections => new()
        {
            
        };
        
        public override void HurtAction(BattleContext context)
        {
            if (context.AttackerData!=null)
            {
                DamageManagement.BuffDamage(context.AttackerData, Stats.Current.Attack);
            }
           
        }

        private void Awake()
        {
            Level = 1;
            NowType = BuildSkillType.ElectricFencing;
            Stats = Attribute[0].Build(gameObject, HurtAction);
        }
        private void OnEnable()
        {
            Level = 1;
            NowType = BuildSkillType.ElectricFencing;
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
