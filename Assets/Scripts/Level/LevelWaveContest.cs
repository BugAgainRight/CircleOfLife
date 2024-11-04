using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    [Serializable]
    public class LevelContext
    {
        [Header("关卡的名称")]
        public string LevelName;
        [Header("关卡编号")]
        public string ID;
        [Header("关卡的描述")]
        public string LevelDescription;
        [Header("关卡的波次")]
        public List<LevelWaveContext> LevelWaves;
    }

    [Serializable]
    public class LevelWaveContext
    {

        [Header("波次的顺序")]
        public int waveNumber;
        [Header("单位出生点")]
        public List<LevelWaveAppearPoint> AppearPoints;
    }

    [Serializable]
    public class LevelWaveAppearPoint
    {
        [Header("出生点名称")]
        public string AppearPointName;
        [Header("出生点位置")]
        public Vector3 Postition;

        [Header("单位群体")]
        public List<LevelWaveUnits> Units;
    }

    [Serializable]
    public class LevelWaveUnits
    {
        [Header("单位的预制体")]
        public GameObject UnitPrefabPath;

        [Header("该单位的数量")]
        public int UnitCount;

        [Header("距离波次开始的时间")]
        public float AppearTime;

        [Header("出现的时间间隔")]
        public float AppearInterval;
    }
}
