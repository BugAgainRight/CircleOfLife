using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class BulletMoveContext 
    {
        public Vector2 StartPos;
        public Vector2 TargetPos;

        public Vector2 Direction;

        public Transform Transform;
        public Transform TargetTransform;

        public float Speed;

        /// <summary>
        /// 上一帧中速度的向量，制作曲线时可以使用到
        /// </summary>
        public Vector2 SpeedVector;

        public float Gravity;

        /// <summary>
        /// 新的X轴方向
        /// </summary>
        public Vector2 XDirection;

        /// <summary>
        /// 新的Y轴方向
        /// </summary>
        public Vector2 YDirection;

        /// <summary>
        /// 曲线最高高度
        /// </summary>
        public float MaxHeight;

        /// <summary>
        /// 发射时间
        /// </summary>
        public float LaunchTime;


        public float LifeTime = 10;
      
    }
}
