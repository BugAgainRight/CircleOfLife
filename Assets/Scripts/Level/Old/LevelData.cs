using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace CircleOfLife.Level.Old
{
    //关卡->波次列表->出生点列表->出生单位列表
    //LevelContext->LevelWaveContext->LevelWaveAppearPoint->LevelWaveUnits
    [Serializable]
    public class Level
    {
        [Header("关卡的名称")]
        public string LevelName;
        [Header("关卡枚举")]
        public LevelEnum LevelEnum;

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
        [Header("出生点枚举")]
        public AppearPointEnum AppearPointEnum;

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
        public EnemyStat TheEnemyStat;

        [Header("该单位的数量")]
        public int UnitCount;

        [Header("距离波次开始的时间")]
        public float AppearTime;

        [Header("出现的时间间隔")]
        public float AppearInterval;
    }
}
