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

        public override int Level { get; protected set; }

        public override void LevelUp(Enum direction = null)
        {
            
        }
        
        public override void HurtAction(BattleContext battleStats)
        {
            if (battleStats.AttackerData!=null)
            {
                DamageManagement.BuffDamage(battleStats.AttackerData, Stats.Current.Attack);
            }
           
        }
        
        private void Awake()
        {
            Stats = Attribute.Build(gameObject, HurtAction);
        }
    }
}
