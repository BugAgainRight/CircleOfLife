using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    [CreateAssetMenu(fileName = "LevelSO", menuName = "ScriptableObject/LevelSO")]
    public class LevelSO : ScriptableObject
    {
        public List<LevelContext> LevelContexts;
    }
}
