using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CircleOfLife.Level
{
    public class LevelManager
    {
        public static string LevelInfoPath = "LevelSO/LevelSOTest";
        public static string CurrentLevelID;
        public static LevelSO LevelInfo;
        public static UnityEvent OnWaveBegin = new UnityEvent();
        public static UnityEvent OnWaveEnd = new UnityEvent();
        public static UnityEvent OnLevelBegin = new UnityEvent();
        public static UnityEvent OnLevelWin = new UnityEvent();
        public static UnityEvent OnLevelLose = new UnityEvent();
        /// <summary>
        /// 下载关卡,并且初始化关卡数据
        /// </summary>
        /// <param name="levelID">关卡编号</param>
        public static void LoadLevel(string levelID)
        {
            LevelInfo = Resources.Load<LevelSO>(LevelInfoPath);
            if (LevelInfo == null)
            {
                Debug.LogWarning("LevelInfo is null");
                return;
            }
            Level level = GetCurrentLevel(levelID);
            CurrentLevelID = level.ID;
            LevelUtils.SetCurrentLevel(level);
        }
        public static void LoadLevel()
        {
            if (CurrentLevelID == null)
            {
                Debug.LogWarning("CurrentLevelID is null");
                return;
            }
            LevelInfo = Resources.Load<LevelSO>(LevelInfoPath);
            if (LevelInfo == null)
            {
                Debug.LogWarning("LevelInfo is null");
                return;
            }
            Level level = GetCurrentLevel(CurrentLevelID);
            LevelUtils.SetCurrentLevel(level);
        }

        /// <summary>
        /// 卸载关卡(好像没用)
        /// </summary>
        public void UnLoadLevel()
        {
            LevelUtils.UnLoadCurrentLevel();
        }
        /// <summary>
        /// 重置关卡(仅针对局内数据)
        /// </summary>
        public void ResetLevel()
        {
            LevelUtils.ResetCurrentLevel();
        }

        //找到指定的关卡数据
        private static Level GetCurrentLevel(string LevelID)
        {
            foreach (Level l in LevelInfo.LevelList)
            {
                if (l.ID == LevelID) return l;
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
            LevelController.Instance.OnWaveBegin();
        }
    }
}
