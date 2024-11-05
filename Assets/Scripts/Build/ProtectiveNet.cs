using CircleOfLife.Battle;
using CircleOfLife.Buff;
using Milutools.Recycle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class ProtectiveNet : BuildBase
    {
        public override void HurtAction(BattleContext context)
        {
            if (context.HitData.Current.Hp <= 0)
            {
                //更新寻路场

                RecyclePool.ReturnToPool(gameObject);
                return;
            }


        }

        private void Awake()
        {
            Stats = Attribute.Build(gameObject, HurtAction);
        }

    }
}
