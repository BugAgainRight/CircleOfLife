using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CircleOfLife.Buff;
using CircleOfLife.Weather;
using Milutools.Recycle;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CircleOfLife.Battle
{
    public partial class BattleStats
    {
        [HideInInspector]
        public Stats Max, Current;
        
        public readonly GameObject GameObject;
        public readonly Transform Transform;
        public readonly IBattleEntity BattleEntity;

        private readonly List<BuffContext> buffContexts = new();
        
        private Stats lasting, initial;
        private readonly Action<BattleContext> hurtAction;

        public readonly IEnumerable<Collider2D> Colliders;

        public void Damage(float damage, BattleContext context)
        {
            Current.Hp = Mathf.Min(Current.Hp - damage, Max.Hp);
            lasting.Hp = Current.Hp;
            if (!GameObject)
            {
                return;
            }
            hurtAction?.Invoke(context);
        }

        public void SetHP(float hp)
        {
            Current.Hp = hp;
            lasting.Hp = hp;
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
                    buff.CleanUpAnimation();
                    buffContexts.Remove(buff);
                    return;
                }
                buff.Level--;
                if (buff.Level <= 0)
                {
                    buff.CleanUpAnimation();
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
                ApplyBuffInner(context);
            }
        }

        private void ApplyBuffInner(BuffContext context)
        {
            if (context.HasAnimation)
            {
                RecyclePool.Request(context.AnimationPrefab, (c) =>
                {
                    context.GeneratedAnimation = c.GameObject;
                    c.Transform.localPosition = Vector3.zero;
                    c.GameObject.SetActive(true);
                }, Transform);
            }
            buffContexts.Add(context);
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
                ApplyBuffInner(context);
            }
        }
        
        public void Tick()
        {
            Max = initial;
            Current = lasting;
            
            WeatherSystem.WeatherBuff(this, null);
            foreach (var buff in buffContexts.Where(_ => true))
            {
                buff.Tick();
                buff.BuffHandler.Invoke(this, buff);
                if (buff.Duration > 0f)
                {
                    buff.Duration = Mathf.Max(0f, buff.Duration - Time.fixedDeltaTime);
                    if (buff.Duration <= 0f)
                    {
                        buff.CleanUpAnimation();
                    }
                }
            }

            buffContexts.RemoveAll(x => x.Duration == 0f);
            Current.Hp = Mathf.Min(Max.Hp, Current.Hp);
        }
        
        public BattleStats(GameObject go, Stats baseStats, Action<BattleContext> hurtAction)
        {
            GameObject = go;
            Max = baseStats;
            lasting = baseStats;
            initial = baseStats;
            this.hurtAction = hurtAction;
            BattleEntity = go.GetComponent<IBattleEntity>();
            Transform = go.transform;
            if (go.TryGetComponent<SkeletonAnimation>(out var skeletonAnimation))
            {
                skeletonAnimation.zSpacing = Random.Range(-0.1f, 0f);
            }
            Colliders = go.GetComponentsInChildren<Collider2D>()
                                  .Concat(go.GetComponents<Collider2D>());
            foreach (var collider in Colliders)
            {
                ColliderMapping.RegisterCollider(collider, this);
            }
        }

        public void Reset()
        {
            Max = initial;
            lasting = Max;
            Current = Max;
        }

        public void ReplaceBaseStat(Stats stat)
        {
            initial = stat;
            Reset();
        }
    }
}
