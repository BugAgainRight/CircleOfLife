using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        private Dictionary<AppearPointEnum, GameObject> sceneAppearPointDict;
        //出生点使用次数字典
        private Dictionary<AppearPointEnum, int> appearPointCountDict;
        //场景中的敌人字典
        private Dictionary<string, GameObject> levelEnemyDict;
        //战斗阶段
        public static LevelStateEnum LevelState;
        private bool isWin;
        private bool isLose;
        #region 事件
        /// <summary>
        /// 关卡进入后执行(不会进行清理)
        /// </summary>
        public static UnityEvent<LevelEnum> OnLevelStartAlways = new UnityEvent<LevelEnum>();
        /// <summary>
        /// 关卡进入后执行(关卡结束后清空)
        /// </summary>
        public static UnityEvent<LevelEnum> OnLevelStart = new UnityEvent<LevelEnum>();
        private bool isLevelStart = false;
        /// <summary>
        /// 波次开始后执行(不会进行清理)
        /// </summary>
        public static UnityEvent<int> OnWaveStartAlways = new UnityEvent<int>();
        /// <summary>
        /// 波次开始后执行(关卡结束后清空)
        /// </summary>
        public static UnityEvent<int> OnWaveStartOnce = new UnityEvent<int>();
        /// <summary>
        /// 波次中执行(置于FixedUpdate中、不会进行清理)
        /// </summary>
        public static UnityEvent<int> OnWaveFixedUpdateAlways = new UnityEvent<int>();
        /// <summary>
        /// 波次中执行(置于FixedUpdate中、关卡结束后清空)
        /// </summary>
        public static UnityEvent<int> OnWaveFixedUpdateOnce = new UnityEvent<int>();
        /// <summary>
        /// 波次结束后执行(不会进行清理)
        /// </summary> 
        public static UnityEvent<int> OnWaveEndAlways = new UnityEvent<int>();
        /// <summary>
        /// 波次结束后执行(关卡结束后清空)
        /// </summary> 
        public static UnityEvent<int> OnWaveEndOnce = new UnityEvent<int>();
        /// <summary>
        /// 关卡胜利后执行(不会进行清理)
        /// </summary>
        public static UnityEvent<LevelEnum> OnLevelWinAlways = new UnityEvent<LevelEnum>();
        /// <summary>
        /// 关卡胜利后执行(关卡结束后清空)
        /// </summary>
        public static UnityEvent<LevelEnum> OnLevelWinOnce = new UnityEvent<LevelEnum>();
        /// <summary>
        /// 关卡失败后执行(不会进行清理)
        /// </summary> 
        public static UnityEvent<LevelEnum> OnLevelLoseAlways = new UnityEvent<LevelEnum>();
        /// <summary>
        /// 关卡失败后执行(关卡结束后清空)
        /// </summary> 
        public static UnityEvent<LevelEnum> OnLevelLoseOnce = new UnityEvent<LevelEnum>();
        /// <summary>
        /// 敌人生成时执行(关卡结束后清空)
        /// </summary> 
        public static UnityEvent<GameObject> OnEnemyCreated = new UnityEvent<GameObject>();

        #endregion
        void Update()
        {

            if (!isLose && !isWin)
            {
                if (!isLevelStart)
                {
                    OnLevelStartAlways.Invoke(LevelContext.Level.LevelEnum);
                    RegisterAllAppearPoints();
                    AssignAppearPoint();
                    OnLevelStart.Invoke(LevelContext.Level.LevelEnum);
                    isLevelStart = true;
                }
                if (CheckLoseCondition())
                {
                    OnLevelLose();
                }
                else if (CheckWaveWinCondition())
                {
                    OnWaveEnd();
                }
            }
        }
        void FixedUpdate()
        {
            if (LevelState == LevelStateEnum.WaveBegin)
            {
                OnWaveFixedUpdateOnce.Invoke(LevelContext.CurrentWave);
                OnWaveFixedUpdateAlways.Invoke(LevelContext.CurrentWave);
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

        public void Reset()
        {
            isWin = false;
            isLose = false;
            LevelState = LevelStateEnum.WaveBefore;
            LevelContext.ResetCurrentLevel();
            UnRegisterAllAppearPoint();
            UnRegisterAllEnemy();
        }
        #region 波次相关
        /// <summary>
        ///下一个波次开始，然后将LevelWaveAppearPoint数据分配至对应出生点
        /// </summary>
        public void OnWaveStart()
        {
            if (LevelContext.Level == null)
            {
                Debug.LogWarning("Level is null,请先加载关卡数据");
                return;
            }
            isWin = false;
            LevelState = LevelStateEnum.WaveBegin;
            OnWaveStartAlways.Invoke(LevelContext.CurrentWave);
            OnWaveStartOnce.Invoke(LevelContext.CurrentWave);
            ResetAppearPointCountDict();

            foreach (var item in sceneAppearPointDict)
            {
                item.Value.GetComponent<LevelAppearPoint>().StartWave();
            }
        }

        //波次结束
        public void OnWaveEnd()
        {

            ChangeCost(LevelContext.CurrentWaveCost);
            LevelState = LevelStateEnum.WaveBefore;
            UnRegisterAllEnemy();
            if (LevelContext.CurrentWave + 1 == LevelContext.MaxWave)
            {
                OnLevelWin();
            }
            else
            {
                Debug.Log("波次" + LevelContext.CurrentWave + "结束");
                LevelContext.CurrentWave += 1;
                OnWaveEndAlways.Invoke(LevelContext.CurrentWave);
                OnWaveEndOnce.Invoke(LevelContext.CurrentWave);
            }

        }
        /// <summary>
        /// 游戏胜利
        /// </summary>
        public void OnLevelWin()
        {
            isWin = true;
            Debug.Log("关卡结束，你过关！");
            OnLevelWinAlways.Invoke(LevelContext.Level.LevelEnum);
            OnLevelWinOnce.Invoke(LevelContext.Level.LevelEnum);
            OnLevelEnd();
        }
        /// <summary>
        /// 游戏失败
        /// </summary>
        public void OnLevelLose()
        {
            isLose = true;
            Debug.Log("游戏失败");
            OnLevelLoseAlways.Invoke(LevelContext.Level.LevelEnum);
            OnLevelLoseOnce.Invoke(LevelContext.Level.LevelEnum);
            OnLevelEnd();
        }
        //游戏结束后清理事件、出生点
        private void OnLevelEnd()
        {
            OnLevelLoseOnce.RemoveAllListeners();
            OnLevelWinOnce.RemoveAllListeners();
            OnWaveStartOnce.RemoveAllListeners();
            OnWaveEndOnce.RemoveAllListeners();
            OnWaveFixedUpdateOnce.RemoveAllListeners();
            OnEnemyCreated.RemoveAllListeners();
            UnRegisterAllAppearPoint();
            isLevelStart = false;
        }
        //检测是否达到胜利条件
        private bool CheckWaveWinCondition()
        {
            return LevelContext.WinCondition;
        }
        //检测是否达到失败条件
        private bool CheckLoseCondition()
        {
            return LevelContext.LoseCondition;
        }
        #endregion
        #region 出生点相关
        //注册并创建出生点,并将其添加到场景中，然后将LevelWaveAppearPoint数据分配至对应出生点
        private void RegisterAppearPoint(EnemyAppearPoint point)
        {
            if (sceneAppearPointDict == null)
            {
                sceneAppearPointDict = new Dictionary<AppearPointEnum, GameObject>();
            }
            if (!CheckAppearPoint(point.AppearPointEnum))
            {
                //Debug.Log("注册出生点:" + point.AppearPointName);
                GameObject gameObject = Instantiate(point.AppearPointObj);
                gameObject.name = point.AppearPointEnum.ToString();
                gameObject.transform.position = new Vector3(point.Postition.x, point.Postition.y, 0);
                gameObject.AddComponent<LevelAppearPoint>();
                sceneAppearPointDict.Add(point.AppearPointEnum, gameObject);
            }
        }
        //分配出生点数据
        private void AssignAppearPoint()
        {
            foreach (LevelWaveAppearPoint item in LevelContext.LevelWaveAppearPointList)
            {
                if (item.AppearPointEnum == AppearPointEnum.Roll)
                {
                    RollAppearPoint(item);
                }
                if (item.AppearPointEnum == AppearPointEnum.RollByMinCount)
                {
                    RollAppearPointByMinCount(item);
                }
                LevelWaveAppearPoint point = item;
                UseAppearPoint(point, point.AppearPointEnum);
            }
        }

        //roll一个出生点,完全随机
        private void RollAppearPoint(LevelWaveAppearPoint item)
        {
            int index = UnityEngine.Random.Range(0, sceneAppearPointDict.Count);
            foreach (var point in sceneAppearPointDict)
            {
                if (index == 0)
                {
                    UseAppearPoint(item, point.Key);
                    //Debug.Log($"Roll 到了出生点: {point.Key}");
                    break;
                }
                index--;

            }
        }

        //roll一个出生点,优先使用本回合使用次数较少的出生点
        private void RollAppearPointByMinCount(LevelWaveAppearPoint item)
        {
            int minCount = 100;
            AppearPointEnum appearPointEnum = AppearPointEnum.Roll;
            foreach (var point in appearPointCountDict)
            {
                if (point.Value < minCount)
                {
                    minCount = point.Value;
                    appearPointEnum = point.Key;
                }
            }
            //Debug.Log($"Roll 到了出生点: {appearPointEnum}");
            UseAppearPoint(item, appearPointEnum);
        }

        //使用出生点
        private void UseAppearPoint(LevelWaveAppearPoint point, AppearPointEnum pointEnum)
        {
            if (sceneAppearPointDict == null)
            {
                sceneAppearPointDict = new Dictionary<AppearPointEnum, GameObject>();
            }
            if (CheckAppearPoint(pointEnum))
            {
                GameObject appearPoint;
                sceneAppearPointDict.TryGetValue(pointEnum, out appearPoint);
                appearPointCountDict[pointEnum] += 1;
                appearPoint.GetComponent<LevelAppearPoint>().SetLevelAppearPoint(point);
            }
        }
        //注册当前关卡的所有出生点
        private void RegisterAllAppearPoints()
        {
            appearPointCountDict = new Dictionary<AppearPointEnum, int>();
            foreach (EnemyAppearPoint point in LevelContext.EnemyAppearPointList)
            {
                RegisterAppearPoint(point);
                appearPointCountDict.Add(point.AppearPointEnum, 0);
            }
            Debug.Log("注册所有出生点:" + sceneAppearPointDict.Count);
        }
        private void ResetAppearPointCountDict()
        {
            foreach (var item in sceneAppearPointDict.Keys)
            {
                appearPointCountDict[item] = 0;
            }
        }
        //检查出生点是否存在
        private bool CheckAppearPoint(AppearPointEnum pointName)
        {
            if (sceneAppearPointDict == null)
            {
                sceneAppearPointDict = new Dictionary<AppearPointEnum, GameObject>();
            }
            return sceneAppearPointDict.ContainsKey(pointName);
        }

        //注销所有出生点
        private void UnRegisterAllAppearPoint()
        {
            if (sceneAppearPointDict == null)
            {
                sceneAppearPointDict = new Dictionary<AppearPointEnum, GameObject>();
            }
            if (appearPointCountDict == null)
            {
                appearPointCountDict = new Dictionary<AppearPointEnum, int>();
            }
            foreach (var item in sceneAppearPointDict)
            {
                Destroy(item.Value);
            }
            Debug.Log("注销所有出生点:" + sceneAppearPointDict.Count);
            appearPointCountDict.Clear();
            sceneAppearPointDict.Clear();
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
                LevelContext.CurrentEnemyCount++;
                levelEnemyDict.Remove(id);
            }
        }

        public void UnRegisterAllEnemy()
        {
            if (levelEnemyDict == null)
            {
                levelEnemyDict = new Dictionary<string, GameObject>();
            }
            foreach (var item in levelEnemyDict)
            {
                Destroy(item.Value);
            }
            levelEnemyDict.Clear();
        }
        #endregion
        #region Cost相关
        /// <summary>
        /// 修改Cost,输入正数为增加Cost,输入负数为减少Cost
        /// </summary>
        /// <param name="cost">cost</param>
        public void ChangeCost(int cost)
        {
            LevelContext.Cost += cost;
        }
        #endregion
    }
    public class WavePointCount
    {
        public AppearPointEnum AppearPointEnum;
        public int Count;
    }
}
