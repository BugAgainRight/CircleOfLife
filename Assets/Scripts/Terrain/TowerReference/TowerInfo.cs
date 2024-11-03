using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    /// <summary>
    ///用于检测鼠标点击的碰撞体tag是否属于防御塔(装置)
    /// </summary> 
    public enum TowerTag
    {
        Tower,
        test,
    }
    public class TowerInfo
    {
        public TowerInfo(GameObject gameObject, Vector2 mousePosition)
        {
            this.GameObject = gameObject;
            this.MousePosition = mousePosition;
            TargetPos = gameObject.transform.position;
        }
        public TowerInfo(GameObject gameObject, Vector2 mousePosition, Vector3 targetPos)
        {
            this.GameObject = gameObject;
            this.MousePosition = mousePosition;
            this.TargetPos = targetPos;
        }
        public TowerInfo(GameObject gameObject, Vector3 targetPos)
        {
            this.GameObject = gameObject;
            MousePosition = Input.mousePosition;
            this.TargetPos = targetPos;
        }
        /// <summary>
        /// 指定的装置
        /// </summary> 

        public GameObject GameObject;
        /// <summary>
        /// 鼠标的位置 
        /// </summary>
        public Vector2 MousePosition;
        /// <summary>
        /// 被指定的装置对应的位置
        /// </summary>
        public Vector3 TargetPos;
    }
}
