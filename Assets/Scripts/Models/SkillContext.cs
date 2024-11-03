using CircleOfLife.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public struct SkillContext 
    {
        /// <summary>
        /// 调用技能者的gameobject
        /// </summary>
        public GameObject TriggerObj;

        /// <summary>
        /// 目标的gameobject
        /// </summary>
        public GameObject TargetObj;

        /// <summary>
        /// 调用技能者的阵营
        /// </summary>
        public FactionType FactionType;

        public Vector2 TriggerPos;
        public Vector2 TargetPos;
        public Vector2 Direction;
        public GameObject BodyPrefab;
        public BattleStats AttackerData;
        public BattleStats HitData;
        public float MoveSpeed;

        public List<GameObject> OtherPrefabs;
        public GizmosSetting RangeSetting;
        /// <summary>
        /// 物理效果作用的layer
        /// </summary>
        public LayerMask PhysicsLayer;
       
    }
}

