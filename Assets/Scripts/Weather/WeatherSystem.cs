using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DigitalRuby.RainMaker;
using Milease.Configuration;
using Milease.Core;
using Milease.Core.Animator;
using Milease.Enums;
using Milease.Utils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ColorUtils = Milease.Utils.ColorUtils;

namespace CircleOfLife.Weather
{
    public class WeatherSystem : MonoBehaviour
    {
        [Flags]
        public enum Weather
        {
            Day = 1, Night = 2, Rainy = 4
        }

        public static WeatherSystem Instance;

        private Weather weather = Weather.Day;

        public static Weather CurrentWeather
        {
            get => Instance.weather;
            set
            {
                Instance.weather = value;
                Instance.weatherAnimators[value].Play();
            }
        }

        public GameObject VolumeContainer;
        public RainScript2D Rain;
        public Light2D[] CloudLights;
        public Light2D GlobalLight;

        private Volume dayVolume, nightVolume, rainyVolume;
        private Dictionary<Weather, MilInstantAnimator> weatherAnimators;

        private MilInstantAnimator WrapAnimator(Weather weatherType, Color globalLight, float globalIntensity)
        {
            var animator =
                Rain.MileaseTo(nameof(Rain.RainIntensity), weatherType == Weather.Rainy ? 1f : 0f, 1f)
                    .While(
                        dayVolume.MileaseTo("weight", weatherType == Weather.Day ? 1f : 0f, 
                            1f, 0f, EaseFunction.Quad, EaseType.Out),
                        nightVolume.MileaseTo("weight", weatherType == Weather.Night ? 1f : 0f, 
                            1f, 0f, EaseFunction.Quad, EaseType.Out),
                        rainyVolume.MileaseTo("weight", weatherType == Weather.Rainy ? 1f : 0f, 
                            1f, 0f, EaseFunction.Quad, EaseType.Out),
                        GlobalLight.MileaseTo(UMN.Color, globalLight, 1f),
                        GlobalLight.MileaseTo("intensity", globalIntensity, 1f)
                    );

            foreach (var cloud in CloudLights)
            {
                animator.While(cloud.MileaseTo("intensity", weatherType == Weather.Day ? 0.5f : 0f, 0.5f));
            }

            return animator;
        }
        
        private void Awake()
        {
            MileaseConfiguration.Configuration.DefaultColorTransformationType = ColorTransformationType.RGB;
            
            Instance = this;
            
            var volumes = VolumeContainer.GetComponents<Volume>();
            dayVolume = volumes.First(x => Mathf.Approximately(x.priority, 0f));
            nightVolume = volumes.First(x => Mathf.Approximately(x.priority, 1f));
            rainyVolume = volumes.First(x => Mathf.Approximately(x.priority, 2f));
            
            weatherAnimators = new Dictionary<Weather, MilInstantAnimator>
            {
                [Weather.Day] = WrapAnimator(Weather.Day, Color.white, 0.4f),
                [Weather.Night] = WrapAnimator(Weather.Night, ColorUtils.RGB(14, 8, 255), 0.3f),
                [Weather.Rainy] = WrapAnimator(Weather.Rainy, Color.white, 0.4f)
            };
            weatherAnimators[Weather.Day].Play();
        }

        private void Update()
        {
            foreach (var light in WeatherManagedLight.ManagedLights)
            {
                var enable = light.IsMeetCondition();
                if (enable != light.Active)
                {
                    light.Active = enable;
                    if (enable)
                    {
                        light.GameObject.SetActive(true);
                    }
                    else
                    {
                        light.FadeOut();
                    }
                }
            }
        }
    }
}
