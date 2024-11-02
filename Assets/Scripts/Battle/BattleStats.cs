using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CircleOfLife.Buff;
using UnityEngine;

namespace CircleOfLife.Battle
{
    public partial class BattleStats
    {
        [HideInInspector]
        public Stats Max, Current;

        private List<BuffContext> buffContexts = new();
        
        private Stats lasting;
        private Action<BattleStats> hurtAction;

        public void Damage(float damage)
        {
            lasting.Hp -= damage;
            Current.Hp -= damage;
            hurtAction?.Invoke(this);
        }
        
        /// <summary>
        /// 移除一层 Buff
        /// </summary>
        /// <param name="context">Buff 上下文</param>
        /// <param name="completely">是否完全移除 Buff，而不是只移除一层</param>
        public void ReduceBuff(BuffContext context, bool completely = false)
        {
            var buff = buffContexts.FirstOrDefault(x => x.BuffHandler == context.BuffHandler);
            if (buff != null)
            {
                if (completely)
                {
                    buffContexts.Remove(buff);
                    return;
                }
                buff.Level--;
                if (buff.Level <= 0)
                {
                    buffContexts.Remove(buff);
                }
            }
        }

        /// <summary>
        /// 只在不包含此 Buff 时施加
        /// </summary>
        /// <param name="context">Buff 上下文</param>
        public void ApplyBuffIfNotExist(BuffContext context)
        {
            if (buffContexts.All(x => x.BuffHandler != context.BuffHandler))
            {
                buffContexts.Add(context);
            }
        }
        
        /// <summary>
        /// 施加一层 Buff
        /// </summary>
        /// <param name="context">Buff 上下文</param>
        public void ApplyBuff(BuffContext context)
        {
            var buff = buffContexts.FirstOrDefault(x => x.BuffHandler == context.BuffHandler);
            if (buff != null)
            {
                if (context.DurationPolicy != BuffDurationPolicy.None)
                {
                    if (context.DurationPolicy == BuffDurationPolicy.ExtendDuration && context.Duration >= 0f)
                    {
                        buff.Duration += context.Duration;
                    }
                    else
                    {
                        buff.Duration = context.Duration;
                    }
                }
                buff.Level = Math.Min(context.MaxLevel, buff.Level + 1);
            }
            else
            {
                buffContexts.Add(context);
            }
        }
        
        public void Tick()
        {
            Current = lasting;
            foreach (var buff in buffContexts.Where(_ => true))
            {
                buff.Tick();
                buff.BuffHandler.Invoke(this, buff);
                if (buff.Duration > 0f)
                {
                    buff.Duration = Mathf.Max(0f, buff.Duration - Time.deltaTime);
                }
            }

            buffContexts.RemoveAll(x => x.Duration == 0f);
        }
        
        public BattleStats(Stats baseStats, Action<BattleStats> hurtAction)
        {
            Max = baseStats;
            this.hurtAction = hurtAction;
        }

        public void Reset()
        {
            lasting = Max;
            Current = Max;
        }
    }
}
