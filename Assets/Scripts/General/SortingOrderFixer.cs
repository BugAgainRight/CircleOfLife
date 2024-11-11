using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife.General
{
    public class SortingOrderFixer : MonoBehaviour
    {
        private Renderer render;
        private Transform trans;
        private int order;

        private void Awake()
        {
            trans = transform;
            render = GetComponent<Renderer>();
        }

        private void Update()
        {
            var o = Mathf.FloorToInt(trans.position.y / 0.33f) * -1;
            if (order != o)
            {
                order = o;
                render.sortingOrder = o;
            }
        }
    }
}
