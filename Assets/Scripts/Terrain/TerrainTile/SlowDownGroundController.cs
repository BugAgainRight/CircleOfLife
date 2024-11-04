using System;
using System.Collections;
using System.Collections.Generic;
using CircleOfLife.Battle;
using CircleOfLife.Buff;
using CircleOfLife.Utils;
using UnityEngine;

namespace CircleOfLife
{
    public class SlowDownGround : MonoBehaviour
    {
        //todo:List<Unit基类>
        private List<string> s;

        private GameObject player;

        private void OnCollisionEnter2D(Collision2D other)
        {
            var stat = other.collider.GetBattleStats();
            stat?.ApplyBuff(BuffUtils.ToBuff(SlowDownDeBuff).SetMaxLevel(2));
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            var stat = other.collider.GetBattleStats();
            stat?.ReduceBuff(BuffUtils.ToBuff(SlowDownDeBuff));
        }

        public static void SlowDownDeBuff(BattleStats stats, BuffContext context)
        {
            stats.Current.Velocity *= 0.75f;
        }
    }
}
