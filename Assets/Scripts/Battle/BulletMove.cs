using Milutools.Recycle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class BulletMove : MonoBehaviour
    {
        private BulletMoveContext moveContext=new();
        public float Speed;
        [SerializeField]
        private BulletMoveType type;

        private Action<BulletMoveContext> moveAction;
        private void Awake()
        {
            moveContext.Transform = transform;
        }
        private void Start()
        {
            moveAction = BulletManagement.GetBulletMove(type);
        }
        private void OnEnable()
        {
            timer = Time.time;
        }
        private float timer;
        public void PassData(BulletMoveContext moveContext)
        {
            this.moveContext = moveContext;
        }
        public void SetTarget(Transform target)
        {
            moveContext.Transform = transform;
            moveContext.TargetTransform = target;
            moveContext.Speed = Speed;
            moveContext.SpeedVector= (target.position - transform.position).normalized;
        }

        private void FixedUpdate()
        {
            moveAction?.Invoke(moveContext);
            if(timer+ moveContext.LifeTime < Time.time)
            {
                RecyclePool.ReturnToPool(gameObject);
            }
        }
    }
}
