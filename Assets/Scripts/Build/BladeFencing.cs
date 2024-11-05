using CircleOfLife.Buff;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class BladeFencing : BuildBase
    {
        public override void HurtAction(BattleContext context)
        {
           
        }
        private void Awake()
        {
            Stats = Attribute.Build(gameObject,HurtAction);
        }
    }
}
