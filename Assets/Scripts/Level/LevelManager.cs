using System;
using System.Collections.Generic;
using System.Linq;
using CircleOfLife.Audio;
using CircleOfLife.Battle;
using CircleOfLife.Buff;
using CircleOfLife.Build;
using CircleOfLife.Build.UI;
using CircleOfLife.Configuration;
using CircleOfLife.General;
using CircleOfLife.ScriptObject;
using CircleOfLife.Units;
using CircleOfLife.Utils;
using CircleOfLife.Weather;
using Milease.Core;
using Milease.Core.Animator;
using Milease.Enums;
using Milease.Utils;
using Milutools.Audio;
using Milutools.Recycle;
using Milutools.SceneRouter;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
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
        public static LevelScriptableObject Level;
        
        public CanvasGroup MainCanvas, FailUI, SuccessUI;
        public TMP_Text MaterialText, FailCause;
        public GameObject MaterialWordPrefab;
        public Volume ServicePostProcess, FailPostProcess, SuccessPostProcess;
        public PathFinderBootstrapper PathFinderBootstrapper;
        public SkeletonAnimation ProtectAnimalSkeleton;
        
        public int Material;
        
        private Grid mapGrid;
        private int curWave, curRound;
        private float waveTick;
        private bool battling = false;
        
        private readonly List<GameObject> remaining = new();
        private readonly Dictionary<AppearPoint, Rect> registeredPoints = new();

        private bool failed = false;

        private bool infinityMode = false;
        
        public void RegisterPoint(AppearPoint point, Rect rect)
        {
            registeredPoints.Add(point, rect);
        }

        public void PopupGameMenu()
        {
            GameMenu.Open(false);
        }
        
        public void StartInfinityMode()
        {
            MessageBox.Open(("开启无尽模式", "该功能用于不断重复第二回合的敌人内容，方便体验全部的装置功能使用。\n" +
                                       "开启后，游戏将永远不会触发“胜利”画面，回合将无限循环。"), (o) =>
            {
                if (o == MessageBox.Operation.Deny)
                {
                    return;
                }

                infinityMode = true;
                
                battling = false;
                curRound--;
                curWave = 0;
                waveTick = 0f;
                
                SuccessPostProcess.MileaseTo(nameof(ServicePostProcess.weight), 0f, 0.5f, 
                        0f, EaseFunction.Quad, EaseType.Out)
                    .While(SuccessUI.MileaseTo("alpha", 0f, 0.5f))
                    .Then(new Action(() => SuccessPostProcess.gameObject.SetActive(false)).AsMileaseKeyEvent())
                    .Play();
                
                PrepareNextRound();
            });
        }
        
        private void Awake()
        {
            Instance = this;
            RecyclePool.EnsurePrefabRegistered(UIEffect.AddMaterial, MaterialWordPrefab, 20);
        }

        private void Start()
        {
            LoadLevel();
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
        
        public void LoadLevel()
        {
            SaveManagement.UseSaveData.Unlock(PlayerSkillType.Melee);
            
            AudioManager.SetBGM(Level.BGM);
            
            var map = Instantiate(Level.MapPrefab);
            map.SetActive(true);
            mapGrid = map.GetComponentInChildren<Grid>();
            PathFinderBootstrapper.Setup();
            
            curWave = curRound = 0;

            BattleUI.Instance.AnimalGraphic.skeletonDataAsset = Level.ProtectAnimal;
            //BattleUI.Instance.AnimalGraphic.Initialize(true);
            BattleUI.Instance.UpdateRoundText(curRound, Level.Rounds.Count);

            ProtectAnimalSkeleton.skeletonDataAsset = Level.ProtectAnimal;
            //ProtectAnimalSkeleton.Initialize(true);
            ProtectAnimalSkeleton.state.SetAnimation(0, "idel", true);
            
            waveTick = 0f;
            Material = Level.InitialMaterial;
            battling = false;
            WeatherSystem.CurrentWeather = (WeatherSystem.Weather)Level.Weather;
            BuildUtils.DisableAllBuilding();
            PlayerController.Instance.enabled = false;

            if (SaveManagement.UseSaveData.AtlasAnimalUnlocks.Any())
            {
                EnumPopupUI.Open(new EnumPopupUIData()
                {
                    Title = "选择出战的小动物",
                    Description = "和小动物并肩作战吧！",
                    List = SaveManagement.UseSaveData.AtlasAnimalUnlocks.Select(x => (object)x).ToList(),
                    Describer = (x) => ((AnimalStat)x) switch
                    {
                        AnimalStat.TibetanMastiff => "藏獒",
                        AnimalStat.TibetanAntelope => "藏羚羊",
                        AnimalStat.Wolf => "狼",
                        AnimalStat.FalcoCherrug => "猎隼",
                        AnimalStat.Bear => "藏棕熊",
                        AnimalStat.WildYak => "野牦牛",
                        _ => throw new ArgumentOutOfRangeException(nameof(x), x, null)
                    }
                }, (o) =>
                {
                    var animal = (AnimalStat)o;
                    RecyclePool.Request(animal, (c) =>
                    {
                        c.Transform.position = Vector3.zero;
                        c.GameObject.SetActive(true);
                    });
                });
            }
            else
            {
                LaunchNextRound();
            }
        }

        public void Retry()
        {
            SceneRouter.GoTo(SceneIdentifier.Battle);
        }

        private void LaunchNextRound()
        {
            MainCanvas.MileaseTo("alpha", 0f, 0.5f, 
                0f, EaseFunction.Quad, EaseType.Out).Play();
            
            BattleUI.Instance.UpdateRoundText(curRound, Level.Rounds.Count);
            
            var round = Level.Rounds[curRound];
            if (round.BeforePlot && !infinityMode)
            {
                PlotBox.Open(round.BeforePlot, () =>
                {
                    StartRound(round);
                });
                return;
            }
            StartRound(round);
        }

        private void StartRound(LevelRound round)
        {
            if (round.SkipBuildPlace)
            {
                StartBattle();
            }
            else
            {
                StartPlacing();
            }
        }

        private void StartBattle()
        {
            battling = true;
            PlayerController.Instance.enabled = true;
            CameraController.Instance.FollowTarget = PlayerController.Instance.gameObject;
            CameraController.Instance.CameraMode = CameraMoveMode.Follow;
            BuildUtils.EnableAllBuilding();
            MainCanvas.MileaseTo("alpha", 1f, 0.5f, 
                0f, EaseFunction.Quad, EaseType.Out).Play();
        }

        private void StartPlacing()
        {
            battling = false;
            BuildUtils.DisableAllBuilding();
            PlayerController.Instance.enabled = false;
            CameraController.Instance.CameraMode = CameraMoveMode.Free;

            BuildingPlaceUI.Open(new BuildingPlaceUIData()
            {
                Buildings = 
                    BuildPrefabSo.Instance.AllBuildSettings
                        .Select(x => new BuildingUIData()
                        {
                            MetaData = x.value2,
                            Type = x.value1
                        }).ToList(),
                MapGrid = mapGrid,
                AvaliableMaterial = Material
            }, (r) =>
            {
                Material = r;
                StartBattle();
            });
        }

        private void SummonEnemy(LevelWave wave)
        {
            var points = registeredPoints.Keys.ToList();
            foreach (var enemy in wave.Enemies)
            {
                for (var i = 0; i < enemy.SummonCount; i++)
                {
                    SaveManagement.UseSaveData.Unlock(enemy.Enemy);
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
            if (failed)
            {
                return;
            }

            failed = true;
            FailCause.text = cause;
            FailPostProcess.gameObject.SetActive(true);
            FailPostProcess.MileaseTo(nameof(ServicePostProcess.weight), 1f, 0.5f, 
                0f, EaseFunction.Quad, EaseType.Out)
                .Then(FailUI.MileaseTo("alpha", 1f, 0.5f))
                .Play();
        }

        private void PrepareNextRound()
        {
            PlayerController.Instance.ResetState();
            PlayerController.Instance.enabled = false;
            BuildUtils.DisableAllBuilding();
            
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
                        AudioManager.PlaySnd(SoundEffectsSO.Clips.Coin);
                        service.SupplyMaterial();
                    }).AsMileaseKeyEvent(1f));
                }

                if (stat.BattleEntity is SignalTransmitter signal)
                {
                    signal.ReturnAllFriend();
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
                var round = Level.Rounds[curRound];
                if (round.SkipBuildPlace)
                {
                    LaunchNextRound();
                }
                else
                {
                    MessageBox.Open(("休整时间", $"回合 {curRound}/{Level.Rounds.Count} 成功守护了小动物，接下来调整装置继续作战吧！"), (_) =>
                    {
                        LaunchNextRound();
                    });
                }
            }).AsMileaseKeyEvent(1f));

            animator.Play();
        }

        private void OnWin()
        {
            MainCanvas.MileaseTo("alpha", 0f, 0.5f, 
                0f, EaseFunction.Quad, EaseType.Out).Play();
            PlayerController.Instance.ResetState();
            PlayerController.Instance.enabled = false;
            BuildUtils.DisableAllBuilding();

            if (Level.WinPlot)
            {
                PlotBox.Open(Level.WinPlot, PostWin);
                return;
            }
            
            PostWin();
        }

        private void PostWin()
        {
            SaveManagement.UseSaveData.CurrentDay++;
            foreach (var skill in Level.UnlockSkills)
            {
                SaveManagement.UseSaveData.Unlock(skill);
            }
            SaveManagement.UseSaveData.Unlock(Level.UnlockAnimal);
            
            SuccessPostProcess.gameObject.SetActive(true);
            SuccessPostProcess.MileaseTo(nameof(ServicePostProcess.weight), 1f, 0.5f, 
                    0f, EaseFunction.Quad, EaseType.Out)
                .Then(SuccessUI.MileaseTo("alpha", 1f, 0.5f))
                .Play(() =>
                {
                    MessageBox.Open(("恭喜！", "你解锁了新的技能！同时，受你救助的小动物加入了你的守卫计划！"));
                });
        }

        public void OnClickNextLevel()
        {
            if (Level.IsFinal)
            {
                SceneRouter.GoTo(SceneIdentifier.Credits);
            }
            else
            {
                SceneRouter.GoTo(SceneIdentifier.Village);
            }
        }
        
        private void Update()
        {
            MaterialText.text = Material.ToString();

            if (Input.GetKeyUp(KeyCode.Escape) && PlayerController.Instance.enabled)
            {
                PopupGameMenu();
            }
            
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

                    if (infinityMode)
                    {
                        curRound--;
                    }
                    
                    if (curRound >= Level.Rounds.Count)
                    {
                        OnWin();
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
