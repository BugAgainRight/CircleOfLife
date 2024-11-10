using System;
using UnityEngine;

namespace CircleOfLife.Level
{
    public class LevelPointConfigure : MonoBehaviour
    {
        public AppearPoint Point;
        private void Awake()
        {
            var size = GetComponent<Renderer>().bounds.size;
            var pos = transform.position;
            LevelManager.Instance.RegisterPoint(Point, new Rect(pos.x, pos.y, size.x, size.y));
            gameObject.SetActive(false);
        }
    }
}
