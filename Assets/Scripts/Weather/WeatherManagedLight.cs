using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Milease.Core;
using Milease.Core.Animator;
using Milease.Utils;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace CircleOfLife.Weather
{
    public class WeatherManagedLight : MonoBehaviour
    {
        public static readonly List<WeatherManagedLight> ManagedLights = new();
        
        public WeatherSystem.Weather[] WeatherCondition;
        
        [HideInInspector]
        public GameObject GameObject;

        private WeatherSystem.Weather cachedCondition;
        private MilInstantAnimator showAnimator, hideAnimator;
        
        [HideInInspector]
        public bool Active = true;

        public bool IsMeetCondition()
        {
            return (WeatherSystem.CurrentWeather ^ cachedCondition) == 0;
        }

        private void OnEnable()
        {
            hideAnimator.Pause();
            showAnimator.PlayImmediately();
        }

        public void FadeOut()
        {
            showAnimator.Pause();
            hideAnimator.PlayImmediately();
        }
        
        private void Awake()
        {
            cachedCondition = 0;
            foreach (var condition in WeatherCondition)
            {
                cachedCondition |= condition;
            }
            
            var lightList = transform.GetComponentsInChildren<Light2D>();
            foreach (var l in lightList)
            {
                var part = l.Milease("intensity", 0f, l.intensity, 0.5f, 0.5f);
                if (showAnimator == null)
                {
                    showAnimator = part;
                }
                else
                {
                    showAnimator.While(part);
                }
                part = l.Milease("intensity", l.intensity, 0f, 0.5f);
                if (hideAnimator == null)
                {
                    hideAnimator = part;
                }
                else
                {
                    hideAnimator.While(part);
                }
            }

            hideAnimator.Then(new Action(() => GameObject.SetActive(false)).AsMileaseKeyEvent());

            showAnimator.UsingResetMode(RuntimeAnimationPart.AnimationResetMode.ResetToInitialState);
            hideAnimator.UsingResetMode(RuntimeAnimationPart.AnimationResetMode.ResetToInitialState);
            
            ManagedLights.Add(this);
            GameObject = gameObject;

            if (!IsMeetCondition())
            {
                Active = false;
                GameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            ManagedLights.Remove(this);
        }
    }
}
