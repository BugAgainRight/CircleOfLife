using CircleOfLife.Battle;
using System;
using UnityEngine;

namespace CircleOfLife.Buff
{
    public static class BuffUtils
    {
        private static GameObject hpBarPrefab;
        public static BuffContext ToBuff(BuffHandleFunction handler, float duration = -1f)
        {
            return new BuffContext()
            {
                BuffHandler = handler,
                Duration = duration
            };
        }

        public static BattleStats Build(this BattleStats.Stats stats, GameObject go, Action<BattleContext> hurtAction,
            bool hideHPBar = false)
        {
            if (!hpBarPrefab)
            {
                hpBarPrefab = Resources.Load<GameObject>("Prefabs/HPBar");
            }
            var stat = new BattleStats(go, stats, hurtAction);
            BuffManager.RegisterBattleStat(stat);

            if (!hideHPBar)
            {
                var hpBar = GameObject.Instantiate(hpBarPrefab);
                hpBar.GetComponent<HPBar>().Initialize(stat);
                hpBar.SetActive(true);
            }
            
            return stat;
        }

        public static BattleContext WrapBuffBattleContext(this BattleStats stats)
        {
            return new BattleContext(0, null, stats);
        }
    }
}
