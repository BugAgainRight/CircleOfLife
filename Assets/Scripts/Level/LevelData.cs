using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace CircleOfLife.Level
{
    //关卡->波次列表->出生点列表->出生单位列表
    //LevelContext->LevelWaveContext->LevelWaveAppearPoint->LevelWaveUnits
    [Serializable]
    public class Level
    {
        [Header("关卡的名称")]
        public string LevelName;
        [Header("关卡编号")]
        public string ID;
        [Header("关卡的描述")]
        public string LevelDescription;

        [Header("关卡的初始费用")]
        public int LevelCost;

        [Header("关卡的波次")]
        /// <summary>
        /// 关卡波次列表
        /// </summary>
        public List<LevelWave> LevelWaveList;

    }

    [Serializable]
    public class LevelWave
    {

        [Header("波次的顺序")]
        public int waveNumber;
        [Header("结算金额")]
        public int WaveCost;
        [Header("单位出生点")]
        /// <summary>
        /// 出生点列表
        /// </summary>
        public List<LevelWaveAppearPoint> AppearPoints;
    }

    [Serializable]
    public class LevelWaveAppearPoint
    {
        [Header("出生点名称")]
        public string AppearPointName;
        [Header("出生点位置")]
        public Vector3 Postition;

        [Header("单位群体列表")]
        /// <summary>
        /// 单位群体列表
        /// </summary>
        public List<LevelWaveUnits> UnitsList;
    }

    [Serializable]
    public class LevelWaveUnits
    {
        [Header("单位的预制体")]
        public GameObject UnitPrefab;

        [Header("该单位的数量")]
        public int UnitCount;

        [Header("距离波次开始的时间")]
        public float AppearTime;

        [Header("出现的时间间隔")]
        public float AppearInterval;
    }
}
