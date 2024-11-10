using System.Collections;
using System.Collections.Generic;
using CircleOfLife.ScriptObject;
using Milutools.Recycle;
using UnityEngine;

namespace CircleOfLife.Level.Old
{
    //出生点数据，只等一声令下，就会生成敌人
    public class LevelAppearPoint : MonoBehaviour
    {
        private LevelWaveAppearPoint appearPoint;
        public List<LevelWaveUnits> levelWaveUnitList;
        private Vector3 maxBounds;
        private Vector3 minBounds;
        private SpriteRenderer spriteRenderer;
        private float time;

        private bool isWaveStart;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            maxBounds = spriteRenderer.bounds.max;
            minBounds = spriteRenderer.bounds.min;
            spriteRenderer.color = new Color(1, 1, 1, 0f);
        }
        void FixedUpdate()
        {
            time += Time.deltaTime;
            CreateEnemy();
        }
        //开始波次
        public void StartWave()
        {
            isWaveStart = true;
        }
        public void EndWave()
        {
            levelWaveUnitList = new List<LevelWaveUnits>();
            isWaveStart = false;
        }
        //获取波次数据
        public void SetLevelAppearPoint(LevelWaveAppearPoint point)
        {
            appearPoint = point;
            if (levelWaveUnitList == null)
            {
                levelWaveUnitList = new List<LevelWaveUnits>();
            }
            foreach (LevelWaveUnits units in appearPoint.UnitsList)
            {
                LevelWaveUnits l = new LevelWaveUnits();
                l.AppearTime = 0 + units.AppearTime;
                l.TheEnemyStat = units.TheEnemyStat;
                l.AppearInterval = 0 + units.AppearInterval;
                l.UnitCount = 0 + units.UnitCount;
                levelWaveUnitList.Add(l);
            }
        }

        //生成敌人
        private void CreateEnemy()
        {

            if (isWaveStart == false)
            {
                return;
            }

            bool isAllEnemyOut = true;

            foreach (LevelWaveUnits units in levelWaveUnitList)
            {
                if (units.UnitCount > 0)
                {
                    if (units.AppearTime <= time)
                    {
                        Vector3 pos = new Vector3(Random.Range(minBounds.x, maxBounds.x), Random.Range(minBounds.y, maxBounds.y), 0);
                        //Debug.Log(this.gameObject.name + ":CreateEnemy:" + units.TheEnemyStat.ToString() + " pos:" + pos);

                        GameObject go = null;
                        RecyclePool.Request(units.TheEnemyStat, c =>
                        {
                            c.Transform.position = pos;
                            c.GameObject.SetActive(true);
                            go = c.GameObject;
                        });
                        go.AddComponent<LevelEnemy>();
                        LevelController.Instance.RegisterEnemy(go);

                        units.UnitCount--;
                        units.AppearTime += units.AppearInterval;
                        LevelController.OnEnemyCreated.Invoke(go);
                    }
                    isAllEnemyOut = false;
                }
            }
            if (isAllEnemyOut)
            {
                EndWave();
            }
        }
    }
}
