using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CircleOfLife.Level
{
    /// <summary>
    /// 局内数据: 波次Wave、敌人数量EnemyCount、胜利条件WinCondition、资源数量Cost、
    /// </summary>
    public static class LevelUtils
    {
        private static Level level;

        public static List<LevelWave> LevelWavelist
        {
            get
            {
                if (level == null)
                {
                    return null;
                }
                return level.LevelWaveList;
            }
        }

        public static List<LevelWaveAppearPoint> AppearPointList
        {
            get
            {
                if (CurrentWave >= LevelWavelist.Count)
                {
                    return null;
                }
                return LevelWavelist[CurrentWave].AppearPoints;
            }
        }
        /// <summary>
        /// <最大波次数量> 
        ///</summary>
        public static int MaxWave
        {
            get
            {
                return LevelWavelist.Count;
            }
        }
        /// <summary>
        /// 该波次的最大敌人数量
        /// </summary>
        public static int MaxEnemyCount
        {
            get
            {
                int count = 0;
                foreach (LevelWaveAppearPoint point in AppearPointList)
                {
                    foreach (LevelWaveUnits unit in point.UnitsList)
                    {
                        count += unit.UnitCount;
                    }
                }
                return count;
            }
        }
        /// <summary>
        /// 判断所有的胜利条件是否都已满足
        /// </summary>
        public static bool WinCondition
        {
            get { return WinCondition1 && WinConditionExample || WinCondition2; }
        }
        /// <summary>
        /// 胜利条件1: 波次达到最大值并且敌人数量达到最大值
        /// </summary>
        public static bool WinCondition1
        {
            get { return CurrentWave >= MaxWave && CurrentEnemyCount >= MaxEnemyCount; }
        }
        /// <summary>
        /// 胜利条件2: 手动获取胜利
        /// </summary>
        public static bool WinCondition2 = false;
        //这里继续注册胜利条件?
        public static bool WinConditionExample
        {
            get { return true; }
        }
        /// <summary>
        /// 失败条件是否成立
        /// </summary>
        /// <value></value>
        public static bool LoseCondition
        {
            get { return LoseCondition1; }
        }
        public static bool LoseCondition1 = false;
        private static int currentWave;
        public static int CurrentWave
        {
            get { return currentWave; }
            set
            {
                OnWaveChange();
                currentWave = value;
            }
        }
        /// <summary>
        /// 当前消灭的敌人数量
        /// </summary>
        public static int CurrentEnemyCount;
        /// <summary>
        /// 当前资源数量
        /// </summary>
        public static int Cost;
        /// <summary>
        /// 当前波次的结算资源
        /// </summary>
        public static int CurrentWaveCost
        {
            get
            {
                return LevelWavelist[CurrentWave].WaveCost;
            }
        }

        /// <summary>
        /// 初始化关卡
        /// </summary>
        /// <param name="level">关卡数据</param>
        public static void SetCurrentLevel(Level level)
        {
            LevelUtils.level = level;
            CurrentWave = 0;
            CurrentEnemyCount = 0;
            Cost = level.LevelCost;
        }
        /// <summary>
        /// 将关卡数据置空（似乎没用）
        /// </summary>
        public static void UnLoadCurrentLevel()
        {
            level = null;
        }
        /// <summary>
        /// 重置关卡
        /// </summary>
        public static void ResetCurrentLevel()
        {
            CurrentWave = 0;
            CurrentEnemyCount = 0;
            Cost = level.LevelCost;
        }

        private static void OnWaveChange()
        {
            //波次改变时对局内数据进行一些改变
            CurrentEnemyCount = 0;
        }
    }
}
