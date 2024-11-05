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

        public override int Level { get; protected set; }

        public override void LevelUp(Enum direction = null)
        {
            
        }

        public override void HurtAction(BattleContext context)
        {
           
        }
        private void Awake()
        {
            Stats = Attribute.Build(gameObject,HurtAction);
        }
    }
}
