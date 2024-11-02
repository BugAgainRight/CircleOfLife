using System;
using CircleOfLife.Battle;

namespace CircleOfLife.Buff
{
    public delegate void BuffHandleFunction(BattleStats stats, BuffContext context);
    
    public partial class BuffHandler
    {
        // 假设有这样的 Buff：
        // 至多叠加 3 层，每层使其速度降低 25%、50%、75%
        // 当生命值低于上限的 50% 时，每 2 秒叠加一层
        // 当生命值高于上限的 50% 时，每 1 秒移除一层，并提升 50% 的攻击力
        public static void SlowDownDeBuff(BattleStats stats, BuffContext context)
        {
            stats.Current.Velocity *= (1f - context.Level * 0.25f);
            if (stats.Current.Hp < stats.Max.Hp * 0.5f)
            {
                if (context.TickedTime >= 2f)
                {
                    stats.ApplyBuff(context);
                    context.ResetTickedTime();
                }
            }
            else
            {
                stats.Current.Attack *= 1.5f;
                if (context.TickedTime >= 1f)
                {
                    stats.ReduceBuff(context);
                    context.ResetTickedTime();
                }
            }
        }

        public static void Test()
        {
            // 依赖注入伤害回调
            var stat = new BattleStats(
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
            
            // 施加 Buff 示例（默认无限时间）
            stat.ApplyBuffIfNotExist(BuffUtils.ToBuff(SlowDownDeBuff).SetMaxLevel(3));
            
            // 指定时长后移除，默认叠层时重置 Buff 持续时间
            stat.ApplyBuffIfNotExist(BuffUtils.ToBuff(SlowDownDeBuff, 10f).SetMaxLevel(3));
            
            // 指定时长后移除，叠层时延长 Buff 持续时间
            stat.ApplyBuffIfNotExist(
                BuffUtils.ToBuff(SlowDownDeBuff, 10f)
                                .SetMaxLevel(3)
                                .ExtendDurationIfExists()
                );
        }
    }
}
