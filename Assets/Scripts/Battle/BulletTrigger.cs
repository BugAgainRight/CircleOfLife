using CircleOfLife.Units;
using Milutools.Recycle;
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

        public void PassData(BattleContext battleContext)
        {
            this.battleContext = battleContext;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamageable_ damage))
            {
                BattleContext battleContext = this.battleContext;

               
                battleContext.HitTran = collision.transform;  
                battleContext.HitData = damage.GetData();
                BulletManagement.GetBulletTrigger(type)(battleContext);

                RecyclePool.ReturnToPool(gameObject);
            }

        }
    }
}
