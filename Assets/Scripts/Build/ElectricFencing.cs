using CircleOfLife.Battle;
using CircleOfLife.Buff;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class ElectricFencing : BuildBase
    {
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
