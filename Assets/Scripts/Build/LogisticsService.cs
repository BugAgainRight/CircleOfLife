using CircleOfLife.Battle;
using CircleOfLife.Buff;
using Milutools.Recycle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class LogisticsService : BuildBase
    {
        public override void HurtAction(BattleStats battleStats)
        {
            if (battleStats.Current.Hp <= 0)
            {
                RecyclePool.ReturnToPool(gameObject);
            }
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
