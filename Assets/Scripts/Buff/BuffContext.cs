using System;
using System.Collections.Generic;
using CircleOfLife.Battle;
using UnityEngine;

namespace CircleOfLife.Buff
{
    public enum BuffDurationPolicy
    {
        /// <summary>
        /// 叠层时重置持续时间
        /// </summary>
        ResetDuration, 
        /// <summary>
        /// 叠层时延长持续时间
        /// </summary>
        ExtendDuration,
        /// <summary>
        /// 叠层时不对持续时间操作
        /// </summary>
        None
    }
    
    public class BuffContext
    {
        public delegate T Modifier<T>(T data);
        
        public int MaxLevel = 1;
        /// <summary>
        /// 当前 Buff 层数
        /// </summary>
        public int Level = 1;
        public BuffHandleFunction BuffHandler;
        /// <summary>
        /// 当前 Buff 剩余时间
        /// </summary>
        public float Duration;
        /// <summary>
        /// 叠层时对持续时间的行为
        /// </summary>
        public BuffDurationPolicy DurationPolicy = BuffDurationPolicy.ResetDuration;
        /// <summary>
        /// 独立计时器，用来实现一些特殊效果，比如：每0.5秒移除一层
        /// </summary>
        public float TickedTime { get; private set;}

        private readonly Dictionary<string, object> dataDict = new();

        public void Set<T>(string key, Modifier<T> modifier, T defaultValue = default)
        {
            dataDict.TryAdd(key, defaultValue);
            dataDict[key] = modifier((T)dataDict[key]);
        }
        
        public T Get<T>(string key, T defaultValue = default)
        {
            dataDict.TryAdd(key, defaultValue);
            return (T)dataDict[key];
        }
        
        public bool IsMeet<T>(string key, Predicate<T> predicate, T defaultValue = default)
        {
            dataDict.TryAdd(key, defaultValue);
            return predicate((T)dataDict[key]);
        }
        
        public void Tick()
        {
            TickedTime += Time.deltaTime;
        }

        public void ResetTickedTime()
        {
            TickedTime = 0f;
        }

        /// <summary>
        /// 设置为 Buff 已存在时，延长其持续时间（默认是重置时间）
        /// </summary>
        /// <returns></returns>
        public BuffContext ExtendDurationIfExists()
        {
            DurationPolicy = BuffDurationPolicy.ExtendDuration;
            return this;
        }
        
        /// <summary>
        /// 设置 Buff 可以叠加的最高等级
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public BuffContext SetMaxLevel(int level)
        {
            MaxLevel = level;
            return this;
        }
    }
}
