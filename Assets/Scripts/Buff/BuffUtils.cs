using CircleOfLife.Battle;
using System;
using UnityEngine;

namespace CircleOfLife.Buff
{
    public static class BuffUtils
    {
        public static BuffContext ToBuff(BuffHandleFunction handler, float duration = -1f)
        {
            return new BuffContext()
            {
                BuffHandler = handler,
                Duration = duration
            };
        }

        public static BattleStats Build(this BattleStats.Stats stats, GameObject go, Action<BattleContext> hurtAction)
        {
            var stat = new BattleStats(go, stats, hurtAction);
            BuffManager.RegisterBattleStat(stat);
            return stat;
        }

        public static BattleContext WrapBuffBattleContext(this BattleStats stats)
        {
            return new BattleContext(0, null, stats);
        }
    }
}
