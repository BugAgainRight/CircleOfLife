using System.Linq;
using CircleOfLife.Battle;
using CircleOfLife.Key;
using CircleOfLife.Utils;
using UnityEngine;

namespace CircleOfLife.Buff
{
    public class UniversalBuff
    {
        /// <summary>
        /// 1. 亢奋：移速+，攻速+
        /// </summary>
        public static void Excited(BattleStats stats, BuffContext buff)
        {
            stats.Current.Velocity *= (1f + buff.Level * 0.25f);
            stats.Current.AttackInterval *= (1f - buff.Level * 0.25f);
        }
        
        /// <summary>
        /// 2. 流血：移动时每秒受到x伤害
        /// </summary>
        public static void Blood(BattleStats stats, BuffContext buff)
        {
            var position = stats.Transform.position;
            if (buff.IsMeet<Vector3>("position", x => x.Equals(position)))
            {
                return;
            }

            buff.Set<Vector3>("position", _ => position);

            if (buff.TickedTime >= 1f)
            {
                buff.ResetTickedTime();
                stats.Damage(11f);
            }
        }
        
        /// <summary>
        /// 3. 束缚：无法移动x秒，对玩家：可以挣脱
        /// </summary>
        public static void Restrict(BattleStats stats, BuffContext buff)
        {
            stats.Current.Velocity = 0f;
            
            if (stats.GameObject.IsPlayer() && KeyEnum.Struggle.IsKeyUp())
            {
                buff.Set<int>("struggle", x => ++x);
                if (buff.IsMeet<int>("struggle", x => x >= 10))
                {
                    stats.ReduceBuff(BuffUtils.ToBuff(Restrict));
                }
            }
        }
        
        /// <summary>
        /// 4. 恐慌：移速+，护甲--，对玩家：无法攻击和移动x秒，对其他：失去目标（乱跑），不攻击x秒
        /// </summary>
        public static void Panic(BattleStats stats, BuffContext buff)
        {
            stats.Current.Armor *= 0.8f;
            stats.Current.AttackInterval = float.PositiveInfinity;

            if (stats.GameObject.IsPlayer())
            {
                stats.Current.Velocity = 0f;
            }
            else
            {
                stats.Current.Velocity *= 1.2f;
                // 乱跑等待后续接入
            }
        }
        
        /// <summary>
        /// 5. 隐蔽：攻击力+，暴击++，闪避++（当敌对单位进入到一定范围内后失效）
        /// </summary>
        public static void Hidden(BattleStats stats, BuffContext buff)
        {
            stats.Current.Attack *= 1.2f;
            stats.Current.CriticalChance += 0.5f;
            stats.Current.EvasionRate += 0.5f;
            
            var entities = Physics2D.OverlapCircleAll(stats.Transform.position, 5f);
            if (entities.Any(x => x.GetBattleStats()?.BattleEntity.FactionType != stats.BattleEntity.FactionType))
            {
                stats.ReduceBuff(BuffUtils.ToBuff(Hidden));
            }
        }
        
        /// <summary>
        /// 6. 缓慢：移动速度-
        /// </summary>
        public static void SlowDown(BattleStats stats, BuffContext buff)
        {
            stats.Current.Velocity *= 0.75f;
        }
        
        /// <summary>
        /// 7. 窒息：每秒受到x伤害
        /// </summary>
        public static void Choke(BattleStats stats, BuffContext buff)
        {
            if (buff.TickedTime >= 1f)
            {
                buff.ResetTickedTime();
                stats.Damage(11f);
            }
        }
        
        /// <summary>
        /// 8. 幸运：闪避+++
        /// </summary>
        public static void Lucky(BattleStats stats, BuffContext buff)
        {
            stats.Current.EvasionRate += 0.75f;
        }
        
        /// <summary>
        /// 9. 愤怒：攻击+，攻速+，移速+，护甲--
        /// </summary>
        public static void Angry(BattleStats stats, BuffContext buff)
        {
            stats.Current.Attack *= 1.2f;
            stats.Current.AttackInterval *= 0.8f;
            stats.Current.Velocity *= 1.2f;
            stats.Current.Armor *= 0.5f;
        }
        
        /// <summary>
        /// 10. 眩晕：无法攻击和移动x秒
        /// </summary>
        public static void Dizzy(BattleStats stats, BuffContext buff)
        {
            stats.Current.AttackInterval = float.PositiveInfinity;
            stats.Current.Velocity = 0f;
        }
        
        /// <summary>
        /// 11. 武装：速度-，护甲++
        /// </summary>
        public static void Armed(BattleStats stats, BuffContext buff)
        {
            stats.Current.Velocity *= 0.8f;
            stats.Current.Armor *= 1.5f;
        }
        
        /// <summary>
        /// 12. 强壮：血量max+，攻击力+
        /// </summary>
        public static void Strong(BattleStats stats, BuffContext buff)
        {
            stats.Max.Hp *= 1.2f;
            stats.Current.Attack *= 1.2f;
        }
        
        /// <summary>
        /// 13. 瘦弱：血量max-，攻击力-
        /// </summary>
        public static void Sick(BattleStats stats, BuffContext buff)
        {
            stats.Max.Hp *= 0.8f;
            stats.Current.Attack *= 0.8f;
        }
        
        /// <summary>
        /// 14. 目光锐利：攻击范围+，暴击率+
        /// </summary>
        public static void Sharp(BattleStats stats, BuffContext buff)
        {
            stats.Current.EffectRange *= 1.2f;
            stats.Current.CriticalChance += 0.25f;
        }
        
        /// <summary>
        /// 15. 无敌：免受任何伤害x秒
        /// </summary>
        public static void Invincible(BattleStats stats, BuffContext buff)
        {
            // 等待减伤比例加入
        }
    }
}
