using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife.Level
{
    /// <summary>
    /// 关卡所处于的阶段
    /// </summary>
    public enum LevelStateEnum
    {
        WaveBegin,
        WaveBefore
    }
    //关卡控制器用于判断关卡所处于的阶段、并且启动刷怪
    public class LevelController : MonoBehaviour
    {
        public static LevelController Instance;

        //场景中的出生点字典
        private Dictionary<string, GameObject> sceneAppearPointDict = new Dictionary<string, GameObject>();

        private Dictionary<string, GameObject> levelEnemyDict;
        //战斗阶段
        public static LevelStateEnum LevelState = LevelStateEnum.WaveBefore;

        private bool isWin = false;

        void Update()
        {
            if (CheckLoseCondition())
            {
                LevelManager.OnLevelLose.Invoke();
            }
            if (CheckWaveWinCondition() && !isWin)
            {
                OnWaveEnd();
            }
        }
        public static void EnsureInitialized()
        {
            if (Instance)
            {
                return;
            }
            var go = new GameObject("[LevelController]", typeof(LevelController));
            go.SetActive(true);
            Instance = go.GetComponent<LevelController>();
        }
        /// <summary>
        ///下一个波次开始,注册并创建出生点,并将其添加到场景中，然后将LevelWaveAppearPoint数据分配至对应出生点
        /// </summary>
        public void OnWaveBegin()
        {
            isWin = false;
            LevelManager.OnWaveBegin.Invoke();
            LevelState = LevelStateEnum.WaveBegin;
            if (LevelUtils.Level == null)
            {
                Debug.LogWarning("Level is null,请先加载关卡数据");
                return;
            }
            foreach (LevelWaveAppearPoint point in LevelUtils.AppearPointList)
            {
                RegisterAppearPoint(point);
            }
        }

        //波次结束
        private void OnWaveEnd()
        {
            LevelState = LevelStateEnum.WaveBefore;
            if (LevelUtils.CurrentWave == LevelUtils.MaxWave - 1)
            {
                isWin = true;
                Debug.Log("关卡结束，你过关！");
                LevelManager.OnLevelWin.Invoke();
            }
            else
            {
                Debug.Log("波次" + LevelUtils.CurrentWave + "结束");
                LevelUtils.CurrentWave += 1;
                LevelManager.OnWaveEnd.Invoke();
            }
        }

        //检测是否达到胜利条件
        public bool CheckWaveWinCondition()
        {
            return LevelUtils.WinCondition;
        }

        public bool CheckLoseCondition()
        {
            return LevelUtils.LoseCondition;
        }
        //注册并创建出生点,并将其添加到场景中，然后将LevelWaveAppearPoint数据分配至对应出生点
        private void RegisterAppearPoint(LevelWaveAppearPoint point)
        {
            if (!CheckAppearPoint(point.AppearPointName))
            {
                Debug.Log("注册出生点:" + point.AppearPointName);
                GameObject gameObject = new GameObject(point.AppearPointName, typeof(LevelAppearPoint));
                gameObject.transform.position = new Vector3(point.Postition.x, point.Postition.y, point.Postition.z);
                gameObject.GetComponent<LevelAppearPoint>().SetLevelAppearPoint(point);
                sceneAppearPointDict.Add(point.AppearPointName, gameObject);
            }
            else
            {
                Debug.Log("使用出生点:" + point.AppearPointName);
                GameObject appearPoint;
                sceneAppearPointDict.TryGetValue(point.AppearPointName, out appearPoint);
                appearPoint.GetComponent<LevelAppearPoint>().SetLevelAppearPoint(point);
            }
        }

        //检查出生点是否存在
        private bool CheckAppearPoint(string pointName)
        {
            if (sceneAppearPointDict == null)
            {
                sceneAppearPointDict = new Dictionary<string, GameObject>();
            }
            return sceneAppearPointDict.ContainsKey(pointName);
        }

        public void RegisterEnemy(GameObject enemyGo)
        {
            if (levelEnemyDict == null)
            {
                levelEnemyDict = new Dictionary<string, GameObject>();
            }
            string id = Guid.NewGuid().ToString();
            enemyGo.GetComponent<LevelEnemy>().ID = id;
            while (!levelEnemyDict.TryAdd(id, enemyGo)) ;
        }

        public void UnRegisterEnemy(string id)
        {
            if (levelEnemyDict == null)
            {
                levelEnemyDict = new Dictionary<string, GameObject>();
            }
            if (levelEnemyDict.ContainsKey(id))
            {
                LevelUtils.CurrentEnemyCount++;
                levelEnemyDict.Remove(id);
            }
        }
    }

}