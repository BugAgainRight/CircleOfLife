using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife.Lighting
{
    public class DynamicCloud : MonoBehaviour
    {
        private const float subSize = 40.41f;
        
        public Transform FollowCamera;
        public Vector2 Contribution;
        public float MoveSpeed;
        
        private Vector2 moved;
        private Transform trans;

        private void Awake()
        {
            trans = transform;
        }

        private void Update()
        {
            moved += Contribution * (MoveSpeed * Time.deltaTime);
            var camPos = (Vector2)FollowCamera.position;
            var tar = moved - camPos;
            tar.x = Mathf.Repeat(tar.x, subSize);
            tar.y = Mathf.Repeat(tar.y, subSize);
            trans.position = tar + camPos;
        }
    }
}
