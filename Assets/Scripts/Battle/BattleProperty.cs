using System;
using UnityEngine;

namespace CircleOfLife.Battle
{
    public partial class BattleStats
    {
        /// <summary>
        /// 战斗实体属性
        /// </summary>
        [Serializable]
        public struct Stats
        {
            /// <summary>
            /// 移速
            /// </summary>
            public float Velocity;

            /// <summary>
            /// 生命值
            /// </summary>
            public float Hp;

            /// <summary>
            /// 攻击力
            /// </summary>
            public float Attack;

            /// <summary>
            /// 护甲
            /// </summary>
            public float Armor;

            /// <summary>
            /// 技能影响范围加成
            /// </summary>
            public float EffectRange;

            /// <summary>
            /// 普攻间隔
            /// </summary>
            public float AttackInterval;

            /// <summary>
            /// 暴击率
            /// </summary>
            public float CriticalChance;

            /// <summary>
            /// 暴击伤害
            /// </summary>
            public float CriticalStrikeDamage;

            /// <summary>
            /// 闪避率
            /// </summary>
            public float EvasionRate;

            /// <summary>
            /// 减伤比例
            /// </summary>
            public float ReduceDamageRate { get; set; }

            /// <summary>
            /// 生命偷取率
            /// </summary>
            public float LifeStealRate { get; set; }

            /// <summary>
            /// 反弹伤害率
            /// </summary>
            public float ReboundDamageRate { get; set; }
        }
    }
}
