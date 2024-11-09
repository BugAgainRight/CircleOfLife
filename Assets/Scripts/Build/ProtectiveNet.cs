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

        protected override void LevelUpFunc()
        {
            
        }

        public override void FixedUpdateFunc()
        {
            
        }

        public override void OnColliderEnterFunc(Collision2D collision)
        {
            
        }

        public override void OnColliderTriggerFunc(Collision2D collision)
        {
            
        }

        public override void OnEnableFunc()
        {
            Level = 1;
            ReplaceStats(Attribute[0], true);
        }

        public override void OnDisableFunc()
        {
        }
    }
}
