using System;
using System.Collections;
using System.Collections.Generic;
using CircleOfLife.Battle;
using CircleOfLife.Buff;
using UnityEngine;

namespace CircleOfLife
{
    public class SlowDownGround : MonoBehaviour
    {
        //todo:List<Unit基类>
        private List<string> s;

        private GameObject player;

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.collider.tag == "Player")
            {
                GiveBuff(other.gameObject);
            }
        }

        public static void SlowDownDeBuff(BattleStats stats, BuffContext context)
        {
            stats.Current.Velocity *= (1f - context.Level * 0.25f);
        }

        private void GiveBuff(GameObject target)
        {
            // 依赖注入伤害回调
            var stat = new BattleStats(
                target, // 这里应该传具体绑定的游戏物体
                new BattleStats.Stats()
                {
                    Hp = 1000,
                    Attack = 10,
                    Velocity = 1
                },
                (stats) =>
                {
                    if (stats.Current.Hp <= 0f)
                    {
                        // 死亡处理
                    }
                }
            );
            stat.ApplyBuff(BuffUtils.ToBuff(SlowDownDeBuff, 0.5f)
                                            .SetMaxLevel(1)
                                            .ExtendDurationIfExists());
        }

    }
}
