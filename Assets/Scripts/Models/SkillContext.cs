using CircleOfLife.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public struct SkillContext 
    {
        public SkillContext(LayerMask layer, BattleStats attackerData, BattleStats hitData=null)
        {
            AttackerData = attackerData;
            HitData = hitData;
            TriggerPos = attackerData.Transform.position;
            TargetPos = hitData?.Transform.position ?? Vector2.zero;
            Direction = (TargetPos - TriggerPos).normalized;

            MoveSpeed = 0f;
            LifeTime = 0f;
            EffectCount = 1;
            SpecialValues = new();

            PhysicsLayer = layer;
        }

        public Vector2 TriggerPos;
        public Vector2 TargetPos;
        public Vector2 Direction;

        public BattleStats AttackerData;
        public BattleStats HitData;

        public int EffectCount;
        public float MoveSpeed;
        /// <summary>
        /// 各种生命时间，buff，子弹等等
        /// </summary>
        public float LifeTime;
        /// <summary>
        /// 特殊值，例如兽医站全屏小动物回复的血量
        /// </summary>
        public List<float> SpecialValues;
    

        /// <summary>
        /// 物理效果作用的layer
        /// </summary>
        public LayerMask PhysicsLayer;
       
    }
}

