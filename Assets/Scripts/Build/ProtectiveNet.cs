using CircleOfLife.Battle;
using CircleOfLife.Buff;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class ProtectiveNet : BuildBase
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

    }
}
