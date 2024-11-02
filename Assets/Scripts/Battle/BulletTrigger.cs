using CircleOfLife.Units;
using Milutools.Recycle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class BulletTrigger : MonoBehaviour
    {
        private BattleContext battleContext;
        [SerializeField]
        private BulletTriggerType type;

        private Action<BattleContext> triggerAction;

        private bool hasTrigger = false;
        private void OnEnable()
        {
            hasTrigger = false;
        }

        private void Start()
        {
            triggerAction = BulletManagement.GetBulletTrigger(type);
        }

        public void PassData(BattleContext battleContext)
        {
            this.battleContext = battleContext;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!hasTrigger&&collision.TryGetComponent(out IDamageable_ damage))
            {
                //hasTrigger = true;
                BattleContext battleContext = this.battleContext;
               
                battleContext.HitTran = collision.transform;  
                battleContext.HitData = damage.GetData();
                triggerAction?.Invoke(battleContext);

                RecyclePool.ReturnToPool(gameObject);
            }

        }
    }
}
