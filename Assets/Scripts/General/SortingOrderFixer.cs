using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
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
            order = render.sortingOrder;
        }

        private void Update()
        {
            var o = (int)(-trans.position.y * 10f);
            if (order != o)
            {
                order = o;
                render.sortingOrder = o;
            }
        }
    }
}
