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
            Day = 1, Night = 2, Rainy = 4, Snowy = 8
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

        public GameObject VolumeContainer, Snow;
        public RainScript2D Rain;
        public Light2D[] CloudLights;
        public Light2D GlobalLight;

        private Volume dayVolume, nightVolume, rainyVolume, snowyVolume;
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
                        snowyVolume.MileaseTo("weight", weatherType == Weather.Snowy ? 1f : 0f, 
                            1f, 0f, EaseFunction.Quad, EaseType.Out),
                        GlobalLight.MileaseTo(UMN.Color, globalLight, 1f),
                        GlobalLight.MileaseTo("intensity", globalIntensity, 1f)
                    );

            foreach (var cloud in CloudLights)
            {
                animator.While(cloud.MileaseTo("intensity", weatherType == Weather.Day ? 1f : 0f, 0.5f));
            }

            animator.While(new Action(() =>
            {
                Snow.SetActive(weatherType == Weather.Snowy);
            }).AsMileaseKeyEvent());
            
            return animator;
        }
        
        private void Awake()
        {
            Instance = this;
            
            var volumes = VolumeContainer.GetComponents<Volume>();
            dayVolume = volumes.First(x => Mathf.Approximately(x.priority, 0f));
            nightVolume = volumes.First(x => Mathf.Approximately(x.priority, 1f));
            rainyVolume = volumes.First(x => Mathf.Approximately(x.priority, 2f));
            snowyVolume = volumes.First(x => Mathf.Approximately(x.priority, 3f));
            
            weatherAnimators = new Dictionary<Weather, MilInstantAnimator>
            {
                [Weather.Day] = WrapAnimator(Weather.Day, Color.white, 0.4f),
                [Weather.Night] = WrapAnimator(Weather.Night, ColorUtils.RGB(14, 8, 255), 0.3f),
                [Weather.Rainy] = WrapAnimator(Weather.Rainy, Color.white, 0.4f),
                [Weather.Snowy] = WrapAnimator(Weather.Snowy, Color.white, 1.2f),
            };
            weatherAnimators[Weather.Day].Play();
        }

        private void Update()
        {
            if (Snow.activeSelf)
            {
                var cam = Camera.main;
                if (cam)
                {
                    Snow.transform.position = cam.transform.position + new Vector3(0f, 5.7f, 0f);
                }
            }
            
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
