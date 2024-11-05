using System;
using CircleOfLife.Buff;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CircleOfLife.Battle;
using Milutools.Recycle;

namespace CircleOfLife
{
    public class BladeFencing : BuildBase
    {
        public override List<LevelUpDirection> LevelUpDirections => new()
        {
            
        };

        public override void HurtAction(BattleContext context)
        {
            if (context.AttackerData != null)
            {
                DamageManagement.BuffDamage(context.AttackerData, Stats.Current.Attack);
            }
            if (Stats.Current.Hp <= 0) RecyclePool.ReturnToPool(gameObject);

        }
        private void Awake()
        {
            Level = 1;
            NowType = BuildSkillType.BladeFencing;
            Stats = Attribute[0].Build(gameObject, HurtAction);
        }
        private void OnEnable()
        {
            Level = 1;
            NowType = BuildSkillType.BladeFencing;
            ReplaceStats(Attribute[0], true);

        }

        private void FixedUpdate()
        {
            RecoveryHP();
        }

        protected override void LevelUpFunc()
        {
            
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Skill(collision);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (!TimerFinish) return;
            Skill(collision);

        }

        private void Skill(Collision2D collision)
        {
            BattleStats stats = collision.collider.GetBattleStats();
            if (stats == null) return;
            if (stats.BattleEntity.FactionType == FactionType.Enemy)
            {
                DamageManagement.Damage(new BattleContext(PhysicsLayer, Stats, stats));
                stats.BattleEntity.Stats.ApplyBuff(BuffUtils.ToBuff(UniversalBuff.Blood, 5f));

            }
        }
    }
}
