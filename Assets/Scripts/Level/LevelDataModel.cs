using System;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife.Level
{
    /// <summary>
    /// 关卡回合
    /// </summary>
    [Serializable]
    public class LevelRound
    {
        public List<LevelWave> Waves;
    }

    /// <summary>
    /// 关卡波次
    /// </summary>
    [Serializable]
    public class LevelWave
    {
        [HideInInspector]
        public string Name;
        public float AfterTime;
        public List<LevelEnemy> Enemies;
    }
    
    /// <summary>
    /// 关卡敌人
    /// </summary>
    [Serializable]
    public class LevelEnemy
    {
        public EnemyStat Enemy;
        
        /// <summary>
        /// 生成数量
        /// </summary>
        public int SummonCount = 1;

        /// <summary>
        /// 出现方位
        /// </summary>
        [Tooltip("空列表 = 四个方位随机，否则从列表中随机选择")]
        public List<AppearPoint> AppearPoints;
    }

    /// <summary>
    /// 出现点位
    /// </summary>
    public enum AppearPoint
    {
        North, South, East, West
    }
}
