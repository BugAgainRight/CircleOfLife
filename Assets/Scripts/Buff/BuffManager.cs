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

        private void FixedUpdate()
        {
            Update();
        }

        // 早于所有脚本执行
        private void Update()
        {
            stats.RemoveAll(x => !x.BindingGameObject);
            foreach (var stat in stats)
            {
                if (!stat.BindingGameObject.activeSelf)
                {
                    continue;
                }
                stat.Tick();
            }
        }
    }
}
