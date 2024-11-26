using System;
using CircleOfLife.Audio;
using CircleOfLife.ScriptObject;
using CircleOfLife.Weather;
using Milutools.Audio;
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
            if (SaveManagement.UseSaveData.CurrentDay == 5)
            {
                WeatherSystem.CurrentWeather = WeatherSystem.Weather.Night;
            }
            DayTitle.text = $"第 {SaveManagement.UseSaveData.CurrentDay + 1} 天";
        }

        private void Start()
        {
            if (SaveManagement.UseSaveData.CurrentDay == 5)
            {
                AudioManager.SetBGM(BGMSO.Clips.SpecialVillage);
            }
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
