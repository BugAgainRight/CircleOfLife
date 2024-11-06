using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife.Level
{
    //出生点获得数据后立刻开始生成敌人
    public class LevelAppearPoint : MonoBehaviour
    {
        private LevelWaveAppearPoint appearPoint;
        private List<LevelWaveUnits> levelWaveUnitList;
        private float time;

        private void Start()
        {
            ResetWaveTime();
        }
        void FixedUpdate()
        {
            time += Time.deltaTime;
            CreateEnemy();
        }

        //重置时间
        public void ResetWaveTime()
        {
            time = 0;
        }

        //获取波次数据
        public void SetLevelAppearPoint(LevelWaveAppearPoint point)
        {
            appearPoint = point;
            levelWaveUnitList = new List<LevelWaveUnits>();
            foreach (LevelWaveUnits units in appearPoint.UnitsList)
            {
                LevelWaveUnits l = new LevelWaveUnits();
                l.AppearTime = 0 + units.AppearTime;
                l.UnitPrefab = units.UnitPrefab;
                l.AppearInterval = 0 + units.AppearInterval;
                l.UnitCount = 0 + units.UnitCount;
                levelWaveUnitList.Add(l);
            }
            ResetWaveTime();
        }

        //生成敌人
        private void CreateEnemy()
        {
            foreach (LevelWaveUnits units in levelWaveUnitList)
            {
                if (units.UnitCount > 0 && units.AppearTime <= time)
                {
                    GameObject go = Instantiate(units.UnitPrefab, transform.position, new Quaternion());
                    go.AddComponent<LevelEnemy>();
                    LevelController.Instance.RegisterEnemy(go);
                    units.UnitCount--;
                    units.AppearTime += units.AppearInterval;
                    LevelController.OnEnemyCreated.Invoke(go);
                }
            }
        }
    }
}
