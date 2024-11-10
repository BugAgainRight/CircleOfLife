using System;
using System.Collections.Generic;
using System.Linq;
using CircleOfLife.Battle;
using CircleOfLife.Buff;
using CircleOfLife.Build;
using CircleOfLife.Build.UI;
using CircleOfLife.General;
using CircleOfLife.ScriptObject;
using CircleOfLife.Units;
using Milease.Core;
using Milease.Core.Animator;
using Milease.Enums;
using Milease.Utils;
using Milutools.Recycle;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace CircleOfLife.Level
{
    public class LevelManager : MonoBehaviour
    {
        public enum UIEffect
        {
            AddMaterial
        }
        
        public static LevelManager Instance;
        public CanvasGroup MainCanvas;
        public Grid MapGrid;
        public TMP_Text MaterialText;
        public GameObject MaterialWordPrefab;
        public Volume ServicePostProcess;
        
        public int Material;

        private LevelScriptableObject Level;

        private int curWave, curRound;
        private float waveTick;
        private bool battling = false;
        
        private readonly List<GameObject> remaining = new();
        private readonly Dictionary<AppearPoint, Rect> registeredPoints = new();

        public void RegisterPoint(AppearPoint point, Rect rect)
        {
            registeredPoints.Add(point, rect);
        }
        
        private void Awake()
        {
            Instance = this;
            RecyclePool.EnsurePrefabRegistered(UIEffect.AddMaterial, MaterialWordPrefab, 20);
        }

        public void SupplyMaterial(int count)
        {
            Material += count;
            RecyclePool.Request(UIEffect.AddMaterial, (c) =>
            {
                var text = c.GameObject.GetComponent<TMP_Text>();
                text.text = "+" + count;
                var rect = c.GetMainComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(Random.Range(-1f, 1f) * 100f, Random.Range(-1f, 1f) * 60f);
                c.GameObject.SetActive(true);
                c.Transform.Milease(UMN.LScale, Vector3.one * 3f, Vector3.one * 1f, 0.5f, 0f, EaseFunction.Circ, EaseType.Out)
                    .Then(
                        c.Transform.Milease(UMN.LScale, Vector3.one * 1f, Vector3.one * 0.8f, 0.5f, 0f, EaseFunction.Circ, EaseType.Out),
                        text.Milease(UMN.Color, Color.cyan, Color.cyan.Clear(), 0.5f)
                    )
                    .UsingResetMode(RuntimeAnimationPart.AnimationResetMode.ResetToInitialState)
                    .PlayImmediately(() => c.RecyclingController.ReturnToPool());
            }, MaterialText.transform);
        }
        
        public void NotifyEnemyDeath(GameObject go)
        {
            remaining.Remove(go);
        }

        public void LoadLevel(string level)
        {
            Level = Resources.Load<LevelScriptableObject>("Levels/" + level);
            curWave = curRound = 0;
            waveTick = 0f;
            Material = Level.InitialMaterial;
            StartPlacing();
        }

        private void StartPlacing()
        {
            battling = false;
            BuildUtils.DisableAllBuilding();
            PlayerController.Instance.enabled = false;
            CameraController.Instance.CameraMode = CameraMoveMode.Free;
            MainCanvas.MileaseTo("alpha", 0f, 0.5f, 
                0f, EaseFunction.Quad, EaseType.Out).Play();
            BuildingPlaceUI.Open(new BuildingPlaceUIData()
            {
                Buildings = 
                    BuildPrefabSo.Instance.AllBuildSettings
                        .Select(x => new BuildingUIData()
                        {
                            MetaData = x.value2,
                            Type = x.value1
                        }).ToList(),
                MapGrid = MapGrid,
                AvaliableMaterial = Material
            }, (r) =>
            {
                battling = true;
                Material = r;
                PlayerController.Instance.enabled = true;
                CameraController.Instance.FollowTarget = PlayerController.Instance.gameObject;
                CameraController.Instance.CameraMode = CameraMoveMode.Follow;
                BuildUtils.EnableAllBuilding();
                MainCanvas.MileaseTo("alpha", 1f, 0.5f, 
                    0f, EaseFunction.Quad, EaseType.Out).Play();
            });
        }

        private void SummonEnemy(LevelWave wave)
        {
            var points = registeredPoints.Keys.ToList();
            foreach (var enemy in wave.Enemies)
            {
                for (var i = 0; i < enemy.SummonCount; i++)
                {
                    var point = enemy.AppearPoints.Count switch
                    {
                        0 => points[Random.Range(0, points.Count)],
                        _ => enemy.AppearPoints[Random.Range(0, enemy.AppearPoints.Count)]
                    };
                    var rect = registeredPoints[point];
                    RecyclePool.Request(enemy.Enemy, (c) =>
                    {
                        remaining.Add(c.GameObject);
                        c.Transform.position = new Vector2(rect.x + rect.width * Random.Range(0f, 1f),
                                                                rect.y + rect.height * Random.Range(0f, 1f));
                        c.GameObject.SetActive(true);
                    });
                }
            }
        }

        public void Fail(string cause)
        {
            MessageBox.Open(("游戏失败！", cause));
        }

        private void PrepareNextRound()
        {
            PlayerController.Instance.enabled = false;
            var animator =  
                ServicePostProcess.MileaseTo(nameof(ServicePostProcess.weight), 1f, 0.5f, 
                            0f, EaseFunction.Quad, EaseType.Out);
            
            foreach (var stat in BuffManager.GetAllStats())
            {
                if (stat.BattleEntity is LogisticsService service)
                {
                    animator.Then(new Action(() =>
                    {
                        CameraController.Instance.FollowTarget = service.gameObject;
                    }).AsMileaseKeyEvent(1f));
                    animator.Then(new Action(() =>
                    {
                        service.SupplyMaterial();
                    }).AsMileaseKeyEvent(1f));
                }
            }

            animator.Then(new Action(() =>
                {
                    CameraController.Instance.FollowTarget = PlayerController.Instance.gameObject;
                }).AsMileaseKeyEvent(1f))
                .Then(
                    ServicePostProcess.MileaseTo(nameof(ServicePostProcess.weight), 0f, 0.5f, 
                        0f, EaseFunction.Quad, EaseType.Out)
                );
            
            animator.Then(new Action(() =>
            {
                MessageBox.Open(("休整时间", $"回合 {curRound}/{Level.Rounds.Count} 成功守护了小动物，接下来调整装置继续作战吧！"), (_) =>
                {
                    StartPlacing();
                });
            }).AsMileaseKeyEvent(1f));

            animator.Play();
        }
        
        private void Update()
        {
            MaterialText.text = Material.ToString();
            
            if (!battling || curRound >= Level.Rounds.Count)
            {
                return;
            }

            var round = Level.Rounds[curRound];

            if (curWave >= round.Waves.Count)
            {
                if (remaining.Count == 0)
                {
                    battling = false;
                    curRound++;
                    curWave = 0;
                    waveTick = 0f;
                    
                    if (curRound >= Level.Rounds.Count)
                    {
                        MessageBox.Open(("胜利！", $"成功守护了小动物并击退了所有的盗猎者！"));
                    }
                    else
                    {
                        PrepareNextRound();
                    }
                }
                return;
            }
            
            waveTick += Time.deltaTime;
            
            var time = 0f;
            for (var i = 0; i < curWave; i++)
            {
                var wave = round.Waves[i];
                time += wave.AfterTime;
            }

            if (waveTick >= round.Waves[curWave].AfterTime + time)
            {
                SummonEnemy(round.Waves[curWave]);
                curWave++;
            }
        }
    }
}
