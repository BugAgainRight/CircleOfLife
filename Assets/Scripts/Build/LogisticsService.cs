using CircleOfLife.Battle;
using CircleOfLife.Buff;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class LogisticsService : BuildBase
    {
        public override void HurtAction(BattleStats battleStats)
        {
            
        }

        private void Awake()
        {
            Stats = Attribute.Build(gameObject, HurtAction);
        }

        private void FixedUpdate()
        {
            if (TimerFinish)
            {
                Debug.Log($"获得资源:{Stats.Current.Attack}");
            }
        }

    }
}
