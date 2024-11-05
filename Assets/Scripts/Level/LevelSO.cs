using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife.Level
{
    [CreateAssetMenu(fileName = "LevelSO", menuName = "ScriptableObject/LevelSO")]
    public class LevelSO : ScriptableObject
    {
        public List<Level> LevelList;
    }
}
