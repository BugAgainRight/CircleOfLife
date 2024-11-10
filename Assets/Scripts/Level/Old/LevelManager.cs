using System.Collections;
using System.Collections.Generic;
using CircleOfLife.Build;
using System.Data.Common;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace CircleOfLife.Level.Old
{
    public static class LevelManager
    {
        public static string LevelInfoPath = "LevelSO/LevelSOTest";
        public static string EnemyAppearPointPath = "LevelSO/EnemyAppearPointSOTest";
        public static LevelEnum CurrentLevelID = LevelEnum.Level1;
        public static LevelSO LevelInfo;
        public static EnemyAppearPointSO EnemyAppearPointInfo;
        #region AddUnityEvent
        /// <summary>
        /// 添加波次开始的监听器(永远调用，不删)
        /// </summary>
        /// <param name="action">参数为当前波次的方法</param>
        public static void OnWaveStartAlways(UnityAction<int> action) => LevelController.OnWaveStartAlways.AddListener(action);
        /// <summary>
        /// 添加波次开始的监听器(关卡结束时会清理)
        /// </summary>
        /// <param name="action">参数为当前波次的方法</param>
        public static void OnWaveStart(UnityAction<int> action) => LevelController.OnWaveStartOnce.AddListener(action);
        /// <summary>
        /// 添加波次结束的监听器(永远调用，不删)
        /// </summary>
        /// <param name="action">参数为当前波次的方法</param>
        public static void OnWaveEndAlways(UnityAction<int> action) => LevelController.OnWaveEndAlways.AddListener(action);
        /// <summary>
        /// 添加波次结束的监听器(关卡结束时会清理)
        /// </summary>
        /// <param name="action">参数为当前波次的方法</param>
        public static void OnWaveEnd(UnityAction<int> action) => LevelController.OnWaveEndOnce.AddListener(action);
        /// <summary>
        /// 添加波次进行时执行的监听器(置于FixedUpdate中)(永远调用，不删)
        /// </summary>
        /// <param name="action">参数为当前波次的方法</param>
        public static void OnWaveFixedUpdateAlways(UnityAction<int> action) => LevelController.OnWaveFixedUpdateAlways.AddListener(action);
        /// <summary>
        /// 添加波次进行时执行的监听器(置于FixedUpdate中)(关卡结束时会清理)
        /// </summary>
        /// <param name="action">参数为当前波次的方法</param>
        public static void OnWaveFixedUpdate(UnityAction<int> action) => LevelController.OnWaveFixedUpdateOnce.AddListener(action);
        /// <summary>
        /// 添加关卡胜利的监听器(永远调用，不删)
        /// </summary>
        /// <param name="action">参数为当前关卡ID的方法</param> 
        public static void OnLevelWinAlways(UnityAction<LevelEnum> action) => LevelController.OnLevelWinAlways.AddListener(action);
        /// <summary>
        /// 添加关卡胜利的监听器(关卡结束时会清理)
        /// </summary>
        /// <param name="action">参数为当前关卡ID的方法</param> 
        public static void OnLevelWin(UnityAction<LevelEnum> action) => LevelController.OnLevelWinOnce.AddListener(action);
        /// <summary>
        /// 添加关卡失败的监听器(永远调用，不删)
        /// </summary>
        /// <param name="action">参数为当前关卡ID的方法</param> 
        public static void OnLevelLoseAlways(UnityAction<LevelEnum> action) => LevelController.OnLevelLoseAlways.AddListener(action);
        /// <summary>
        /// 添加关卡失败的监听器(关卡结束时会清理)
        /// </summary>
        /// <param name="action">参数为当前关卡ID的方法</param> 
        public static void OnLevelLose(UnityAction<LevelEnum> action) => LevelController.OnLevelLoseOnce.AddListener(action);
        /// <summary>
        /// 添加敌人生成时的监听器(关卡结束时会清理)
        /// </summary>
        /// <param name="action">参数为 第一时间生成的敌人 的方法</param> 
        public static void OnEnemyCreated(UnityAction<GameObject> action) => LevelController.OnEnemyCreated.AddListener(action);

        #endregion
        /// <summary>
        /// 下载关卡,并且初始化关卡数据
        /// </summary>
        /// <param name="levelID">关卡编号</param>
        public static void LoadLevel(LevelEnum levelID)
        {
            if (LevelInfo == null)
            {
                LevelInfo = Resources.Load<LevelSO>(LevelInfoPath);
            }
            if (EnemyAppearPointInfo == null)
            {
                EnemyAppearPointInfo = Resources.Load<EnemyAppearPointSO>(EnemyAppearPointPath);
            }
            Level level = GetCurrentLevel(levelID);
            AppearPointsInLevels appearPointsInLevels = GetCurrentLevelPoints(levelID);
            CurrentLevelID = level.LevelEnum;
            LevelContext.SetCurrentLevel(level, appearPointsInLevels);
            LevelController.EnsureInitialized();
            LevelController.Instance.Reset();
        }
        public static void LoadLevel()
        {
            if (LevelInfo == null)
            {
                LevelInfo = Resources.Load<LevelSO>(LevelInfoPath);
            }
            if (EnemyAppearPointInfo == null)
            {
                EnemyAppearPointInfo = Resources.Load<EnemyAppearPointSO>(EnemyAppearPointPath);
            }
            Level level = GetCurrentLevel(CurrentLevelID);
            AppearPointsInLevels appearPointsInLevels = GetCurrentLevelPoints(CurrentLevelID);
            CurrentLevelID = level.LevelEnum;
            LevelContext.SetCurrentLevel(level, appearPointsInLevels);
            LevelController.EnsureInitialized();
            LevelController.Instance.Reset();
        }

        /// <summary>
        /// 卸载关卡(好像没用)
        /// </summary>
        public static void UnLoadLevel()
        {
            LevelContext.UnLoadCurrentLevel();
        }
        /// <summary>
        /// 重置关卡(仅针对局内数据)
        /// </summary>
        public static void ResetLevel()
        {
            LevelController.EnsureInitialized();
            LevelController.Instance.Reset();
        }

        //找到指定的关卡数据
        private static Level GetCurrentLevel(LevelEnum LevelID)
        {
            foreach (Level l in LevelInfo.LevelList)
            {
                if (l.LevelEnum == LevelID) return l;
            }
            Debug.LogWarning("Countn't find LevelID");
            return null;
        }

        private static AppearPointsInLevels GetCurrentLevelPoints(LevelEnum LevelID)
        {
            foreach (AppearPointsInLevels l in EnemyAppearPointInfo.AppearPointsInEveryLevel)
            {
                if (l.LevelEnum == LevelID) return l;
            }
            Debug.LogWarning("Countn't find LevelID");
            return null;
        }

        /// <summary>
        /// 开始一个波次
        /// </summary>
        public static void StartWave()
        {
            LevelController.EnsureInitialized();
            LevelController.Instance.OnWaveStart();
            BuildUtils.EnableAllBuilding();
        }

        ///<summary>
        /// 强制结束当前波次(测试用 不会遏制敌人继续出生)
        /// </summary>
        public static void EndWave()
        {
            LevelController.EnsureInitialized();
            LevelController.Instance.OnWaveEnd();
            BuildUtils.DisableAllBuilding();
        }
        /// <summary>
        /// 直接判定游戏失败
        /// </summary>
        public static void LevelLose()
        {
            LevelController.EnsureInitialized();
            LevelController.Instance.OnLevelLose();
        }

        /// <summary>
        /// 直接判定游戏胜利
        /// </summary>
        public static void LevelWin()
        {
            LevelController.EnsureInitialized();
            LevelController.Instance.OnLevelWin();
        }

        public static int GetCost() => LevelContext.Cost;
        public static void ChangeCost(int cost) => LevelContext.Cost += cost;
    }
}
