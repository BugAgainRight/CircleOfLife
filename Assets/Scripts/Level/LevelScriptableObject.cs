using System;
using System.Collections.Generic;
using CircleOfLife.Audio;
using CircleOfLife.Weather;
using Spine;
using Spine.Unity;
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

        [Header("地图预制体")]
        public GameObject MapPrefab;
        [Header("初始材料")]
        public int InitialMaterial;
        [Header("关卡天气")]
        public WeatherType Weather = WeatherType.Day;
        [Header("保护的小动物")]
        public SkeletonDataAsset ProtectAnimal;
        [Header("是否是最后一关")]
        public bool IsFinal;
        [Header("背景音乐")]
        public BGMSO.Clips BGM = BGMSO.Clips.Battle;
        public List<LevelRound> Rounds;
        [Header("胜利后的剧情")]
        public TextAsset WinPlot;
        [Header("本关解锁的小动物")]
        public AnimalStat UnlockAnimal;
        [Header("本关解锁的玩家技能列表")]
        public List<PlayerSkillType> UnlockSkills;
        
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
