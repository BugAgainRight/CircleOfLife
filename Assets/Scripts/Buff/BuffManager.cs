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

        /// <summary>
        /// 注册到 Buff 管理器中，建议 Awake 中操作
        /// </summary>
        /// <param name="stats"></param>
        public static void RegisterBattleStat(BattleStats stats)
        {
            EnsureInitialized();
            Instance.stats.Add(stats);
        }
        
        /// <summary>
        /// 从 Buff 管理器中删除注册
        /// </summary>
        /// <param name="stats"></param>
        public static void RevokeBattleStat(BattleStats stats)
        {
            EnsureInitialized();
            Instance.stats.Remove(stats);
        }
        
        // 早于所有脚本执行
        private void Update()
        {
            foreach (var stat in stats)
            {
                stat.Tick();
            }
        }
    }
}
