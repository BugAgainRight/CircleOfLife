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
        public Transform AttackerTran;

        /// <summary>
        /// 受击者Transform
        /// </summary>
        public Transform HitTran;

        /// <summary>
        /// 受击者Collider
        /// </summary>
        public Collider2D HitCollider;

        /// <summary>
        /// 攻击者的数据
        /// </summary>
        public NPCData AttackerData;
        /// <summary>
        /// 受击者的数据
        /// </summary>
        public NPCData HitData;

        public float BoomRadius;

        public int DamageableLayer;
    
    }

}
