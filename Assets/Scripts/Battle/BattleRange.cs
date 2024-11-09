using System;
using CircleOfLife.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class BattleRange : MonoBehaviour
    {
        public GizmosSetting Range;

        private void Awake()
        {
            Range.point = transform;
        }

        public List<Collider2D> GetAllEnemyInRange(LayerMask layer,FactionType attackerFaction)
        {
            var list = Range.GetCollInRange(layer, 0);
            
            list.RemoveAll(item =>
            {
                BattleStats mid = item.GetBattleStats();
                if (mid == null || attackerFaction.Equals(mid.BattleEntity.FactionType)|| item.gameObject == gameObject) return true;
                return false;
            });

            return list;
        }

        public List<Collider2D> GetAllFriendInRange(LayerMask layer, FactionType attackerFaction)
        {
            var list = Range.GetCollInRange(layer, 0);
            list.RemoveAll(item =>
            {
                BattleStats mid = item.GetBattleStats();
                if (mid == null || !attackerFaction.Equals(mid.BattleEntity.FactionType) || item.gameObject==gameObject) return true;
                return false;
            });
            return list;
        }
    }
}
