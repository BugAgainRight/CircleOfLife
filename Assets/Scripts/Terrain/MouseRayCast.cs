using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class MouseRayCast : MonoBehaviour
    {
        private Vector2 mousePosition;
        //从鼠标处发射射线，获取射线触碰的所有碰撞体
        void Start()
        {

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            SetRayCast();
        }

        public void SetRayCast()
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log(Input.mousePosition.ToString() + mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);
            foreach (RaycastHit2D hit in hits)
            {
                //Debug.Log(hit.collider.name);
            }
        }
    }
}
