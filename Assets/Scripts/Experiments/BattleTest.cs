using System;
using CircleOfLife.Build;
using CircleOfLife.Weather;
using UnityEngine;

namespace CircleOfLife.Experiments
{
    public class BattleTest : MonoBehaviour
    {
        public WeatherSystem.Weather Weather;
        private void Start()
        {
            WeatherSystem.CurrentWeather = Weather;
            BuildUtils.EnableAllBuilding();
        }
    }
}
