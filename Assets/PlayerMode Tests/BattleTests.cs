using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using CircleOfLife.Battle; // 引用 BattleStats 类所在的命名空间
using CircleOfLife.Buff;
/*半成品，已经废弃*/
namespace CircleOfLife.Tests.Battle
{
    public class BattleStatsTests
    {
        static GameObject TestGameObject = new();
        static BattleContext BattleContext = new()
        {
             //等待填入
        };
        static BattleStats TestBattleStats = new(TestGameObject, TestStats, null);
        static BattleStats.Stats TestStats = new()
        {
            Armor = 1,
            Attack = 1,
            AttackInterval = 0.1F,
            CriticalChance = 0.5F,
            CriticalStrikeDamage = 50,
            EffectRange = 1,
            EvasionRate = 0.5f,
            Hp = 100,
            Velocity = 1
        };
        static BuffContext context = new()
        {
            Duration = 10,
            Level = 1,
            MaxLevel = 1,
            BuffHandler = BuffHandler.SlowDownDeBuff,
            DurationPolicy = BuffDurationPolicy.ExtendDuration

        };

       /* [UnityTest]
        public IEnumerator DamageTest()
        {
            float initialHp = TestBattleStats.Current.Hp;
            float damage = 10f;
            TestBattleStats.Damage(damage,context);
            yield return null; // 等待一帧以模拟异步操作
            Assert.AreEqual(initialHp - damage, TestBattleStats.Current.Hp, "The damage should reduce the current HP correctly.");
        }*/

        [UnityTest]
        public IEnumerator ApplyBuffTest()
        {
            TestBattleStats.ApplyBuff(context);
            yield return null;
            Assert.IsTrue(context.BuffHandler == BuffHandler.SlowDownDeBuff, "The buff should be applied correctly.");//uncorrect
        }

        [UnityTest]
        public IEnumerator ReduceBuffTest()
        {
            TestBattleStats.ApplyBuff(context);
            TestBattleStats.ReduceBuff(context);
            yield return null;
            Assert.AreEqual(1, TestBattleStats, "The buff level should be reduced by one.");//uncorrect
        }


    }
}


