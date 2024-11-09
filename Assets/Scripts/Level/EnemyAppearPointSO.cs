using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife.Level
{
    [CreateAssetMenu(fileName = "EnemyAppearPointSO", menuName = "ScriptableObject/EnemyAppearPointSO")]
    public class EnemyAppearPointSO : ScriptableObject
    {
        public List<AppearPointsInLevels> AppearPointsInEveryLevel;
    }
    [Serializable]
    public class AppearPointsInLevels
    {
        public LevelEnum LevelEnum;
        public List<EnemyAppearPoint> EnemyAppearPointList;
    }
    [Serializable]
    public class EnemyAppearPoint
    {
        public AppearPointEnum AppearPointEnum;
        public Vector3 Postition;
        public GameObject AppearPointObj;
    }
}
