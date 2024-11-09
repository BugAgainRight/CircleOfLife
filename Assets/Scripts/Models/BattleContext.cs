using CircleOfLife.Battle;
using CircleOfLife.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    /// <summary>
    /// 阵营类型
    /// </summary>
    public enum FactionType
    {
        Friend, Enemy
    }
    public struct BattleContext
    {

        public BattleContext(LayerMask damageableLayer, BattleStats attackerData, BattleStats hitData)
        {
            AttackerData = attackerData;
            HitData = hitData;
            BoomRadius = 0;
            DamageableLayer = damageableLayer;
            Prefab = null;
            SkillRate = 1;
            BulletTransform = null;
        }


        /// <summary>
        /// 攻击者的数据
        /// </summary>
        public BattleStats AttackerData;
        /// <summary>
        /// 受击者的数据
        /// </summary>
        public BattleStats HitData;

        public float BoomRadius;
        public float SkillRate;

        public LayerMask DamageableLayer;

        public GameObject Prefab;

        public Transform BulletTransform;

    }

}
