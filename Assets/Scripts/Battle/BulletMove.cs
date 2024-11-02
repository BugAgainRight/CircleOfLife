using Milutools.Recycle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class BulletMove : MonoBehaviour
    {
        private BulletMoveContext moveContext;
        [SerializeField]
        private BulletMoveType type;

        private Action<BulletMoveContext> moveAction;

        private void Start()
        {
            moveAction = BulletManagement.GetBulletMove(type);
        }

        public void PassData(BulletMoveContext moveContext)
        {
            this.moveContext = moveContext;
        }

        private void FixedUpdate()
        {
            moveAction?.Invoke(moveContext);
        }
    }
}
