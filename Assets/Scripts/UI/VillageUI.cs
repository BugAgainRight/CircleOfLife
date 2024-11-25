using System;
using CircleOfLife.ScriptObject;
using CircleOfLife.Weather;
using TMPro;
using UnityEngine;

namespace CircleOfLife
{
    public class VillageUI : MonoBehaviour
    {
        public TMP_Text DayTitle;
        
        private void Awake()
        {
            WeatherSystem.CurrentWeather = (WeatherSystem.Weather)LevelFlowManager.Levels[SaveManagement.UseSaveData.CurrentDay].Weather;
            DayTitle.text = $"第 {SaveManagement.UseSaveData.CurrentDay + 1} 天";
        }

        public void SaveDialog()
        {
            SaveUI.Open(true);
        }

        public void OpenGameMenu()
        {
            GameMenu.Open(true);
        }
    }
}
