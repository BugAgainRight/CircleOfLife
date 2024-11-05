using CircleOfLife.Battle;
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
            if (!hasTrigger&&collision.TryGetComponent(out IBattleEntity damage))
            {
                //hasTrigger = true;
                BattleContext battleContext = this.battleContext;
                battleContext.HitData = damage.Stats;
                triggerAction?.Invoke(battleContext);

                RecyclePool.ReturnToPool(gameObject);
            }

        }
    }
}
