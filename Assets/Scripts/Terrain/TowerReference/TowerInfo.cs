using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    //用于检测鼠标点击的碰撞体是否属于防御塔(装置)
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
        public GameObject GameObject;
        public Vector2 MousePosition;
        public Vector3 TargetPos;
    }
}
