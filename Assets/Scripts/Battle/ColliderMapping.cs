using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife.Battle
{
    public static class ColliderMapping
    {
        private static readonly Dictionary<Collider2D, BattleStats> map = new();

        public static void RegisterCollider(Collider2D collider, BattleStats stats)
        {
            map.TryAdd(collider, stats);
        }

        public static void RevokeCollider(Collider2D collider)
        {
            map.Remove(collider);
        }

        /// <summary>
        /// 返回碰撞箱对应的战斗面板，null = 与战斗单位无关
        /// </summary>
        public static BattleStats GetBattleStats(this Collider2D collider)
        {
            return map.GetValueOrDefault(collider);
        }
    }
}
