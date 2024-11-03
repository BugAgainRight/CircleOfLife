using System;
using System.Collections.Generic;
using CircleOfLife.Battle;
using UnityEngine;

namespace CircleOfLife.Buff
{
    public class BuffManager : MonoBehaviour
    {
        public static BuffManager Instance;

        private readonly List<BattleStats> stats = new();

        private static void EnsureInitialized()
        {
            if (Instance)
            {
                return;
            }

            var go = new GameObject("[BuffManager]", typeof(BuffManager));
            go.SetActive(true);
            Instance = go.GetComponent<BuffManager>();
        }
        
        public static void RegisterBattleStat(BattleStats stats)
        {
            EnsureInitialized();
            Instance.stats.Add(stats);
        }
        
        // 早于所有脚本执行
        private void FixedUpdate()
        {
            foreach (var stat in stats)
            {
                if (!stat.GameObject)
                {
                    foreach (var col in stat.Colliders)
                    {
                        ColliderMapping.RevokeCollider(col);
                    }
                }
            }
            stats.RemoveAll(x => !x.GameObject);
            foreach (var stat in stats)
            {
                if (!stat.GameObject.activeSelf)
                {
                    continue;
                }
                stat.Tick();
            }
        }
    }
}
