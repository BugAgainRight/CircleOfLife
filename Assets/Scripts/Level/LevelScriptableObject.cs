using System;
using System.Collections.Generic;
using CircleOfLife.Weather;
using UnityEngine;

namespace CircleOfLife.Level
{
    [CreateAssetMenu(fileName = "LevelSO(New)")]
    public class LevelScriptableObject : ScriptableObject
    {
        public enum WeatherType
        {
            Day = 1, Night = 2, Rainy = 4, Snowy = 8
        }
        
        public int InitialMaterial;
        public WeatherType Weather = WeatherType.Day;
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
