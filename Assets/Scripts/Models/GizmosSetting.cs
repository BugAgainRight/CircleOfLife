using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CircleOfLife
{
    public enum DrawGizmosType { rectangle = 0, circle }
    [Serializable]
    public class GizmosSetting
    {

        public bool editorMode;
        public bool openGizmos;
        public DrawGizmosType GizmosType;
        public Transform point;
        public float radius;
        public Vector2 size, offSet;
        public Color gizmosColor;
        public Vector2 gizmosCenter { get { return (Vector2)point.position + offSet; } }

        public void AutoRange(Vector2Int xRange, Vector2Int yRange)
        {
            point.position = Vector3.zero;
            int sizeX = xRange.y - xRange.x;
            int sizeY = yRange.y - yRange.x;
            size = new Vector2(sizeX, sizeY);
            offSet = new Vector2((xRange.y + xRange.x) / 2f, (yRange.y + yRange.x) / 2f);
        }
        /// <summary>
        /// 默认-1为所有layer
        /// </summary>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        public List<Collider2D> GetCollInRange(int layerMask = -1)
        {

            if (GizmosType == DrawGizmosType.rectangle)
                if (layerMask == -1) return Physics2D.OverlapBoxAll(gizmosCenter, size, 0).ToList();
                else return Physics2D.OverlapBoxAll(gizmosCenter, size, 0, layerMask).ToList();
            else if (GizmosType == DrawGizmosType.circle)
                if (layerMask == -1) return Physics2D.OverlapCircleAll(gizmosCenter, radius).ToList();
                else return Physics2D.OverlapCircleAll(gizmosCenter, radius, layerMask).ToList();
            return null;
        }

    }

}
