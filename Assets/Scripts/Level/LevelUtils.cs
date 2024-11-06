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
        #region SO
        private static Level level;
        public static Level Level
        {
            get
            {
                if (level == null)
                {
                    Debug.LogWarning("你没有加载关卡数据,默认加载测试关卡");
                    Debug.Log("你没有加载关卡数据,默认加载测试关卡，如果要加载其他关卡，请使用LevelManager.LoadLevel方法");
                    level = Resources.Load<LevelSO>("LevelSO/LevelSOTest").LevelList[0];
                }
                return level;
            }
            set
            {
                level = value;
            }
        }

        public static List<LevelWave> LevelWavelist
        {
            get
            {
                return Level.LevelWaveList;
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
        #endregion
        #region 静态数据
        /// <summary>
        /// <最大波次数量> 
        ///</summary>
        public static int MaxWave
        {
            get
            {
                if (Level == null)
                {
                    return 0;
                }
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
        #endregion
        #region 胜败条件
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
            get { return CurrentEnemyCount >= MaxEnemyCount; }
        }
        /// <summary>
        /// 胜利条件2: 手动获取胜利?
        /// </summary>
        public static bool WinCondition2 = false;
        //这里继续注册胜利条件?
        public static bool WinConditionExample = true;
        /// <summary>
        /// 失败条件是否成立，我也不知道是什么，写这里感觉也不太方便，还是出去直接调用LevelManager.OnGameLose方法的好
        /// </summary>
        public static bool LoseCondition
        {
            get { return LoseCondition1; }
        }
        /// <summary>
        /// 失败条件1:我也不知道是什么，写这里不太方便，还是出去直接调用LevelManager.OnGameLose方法的好
        /// </summary>
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
        #endregion
        #region  动态数据
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
        #endregion
        #region 方法
        /// <summary>
        /// 初始化关卡
        /// </summary>
        /// <param name="level">关卡数据</param>
        public static void SetCurrentLevel(Level level)
        {
            Level = level;
            CurrentWave = 0;
            CurrentEnemyCount = 0;
            Cost = level.LevelCost;
        }
        /// <summary>
        /// 将关卡数据置空（似乎没用）
        /// </summary>
        public static void UnLoadCurrentLevel()
        {
            Level = null;
        }
        /// <summary>
        /// 重置关卡(波次归零、敌人数量归零、资源数量设置为初始值)
        /// </summary>
        public static void ResetCurrentLevel()
        {
            CurrentWave = 0;
            CurrentEnemyCount = 0;
            Cost = Level.LevelCost;
        }

        private static void OnWaveChange()
        {
            //波次改变时对局内数据进行一些改变
            CurrentEnemyCount = 0;
        }
        #endregion
    }
}
