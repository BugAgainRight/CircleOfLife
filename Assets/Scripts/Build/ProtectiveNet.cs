using System;
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
        public override List<LevelUpDirection> LevelUpDirections => new()
        {
            
        };

        
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
            Level = 1;
            Stats = Attribute[0].Build(gameObject, HurtAction);
        }
        private void OnEnable()
        {
            Level = 1;
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
