using System.Collections;
using System.Collections.Generic;
using CircleOfLife.Weather;
using UnityEngine;

namespace CircleOfLife
{
    public class WeatherSystemTest : MonoBehaviour
    {
        public void DayTest() => WeatherSystem.CurrentWeather = WeatherSystem.Weather.Day;
        public void NightTest() => WeatherSystem.CurrentWeather = WeatherSystem.Weather.Night;
        public void RainyTest() => WeatherSystem.CurrentWeather = WeatherSystem.Weather.Rainy;
    }
}
