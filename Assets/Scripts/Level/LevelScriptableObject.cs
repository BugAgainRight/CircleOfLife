using System;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife.Level
{
    [CreateAssetMenu(fileName = "LevelSO(New)")]
    public class LevelScriptableObject : ScriptableObject
    {
        public int InitialMaterial;
        public List<LevelRound> Rounds;

        private void OnValidate()
        {
            foreach (var round in Rounds)
            {
                var time = 0f;
                foreach (var wave in round.Waves)
                {
                    time += wave.AfterTime;
                    wave.Name = $"{time:F2} 秒后：";
                }
            }
        }
    }
}
