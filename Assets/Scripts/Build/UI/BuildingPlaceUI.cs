using System;
using System.Collections.Generic;
using System.Linq;
using CircleOfLife.Buff;
using CircleOfLife.General;
using CircleOfLife.Key;
using CircleOfLife.ScriptObject;
using Milease.Core.Animator;
using Milease.Core.UI;
using Milease.Enums;
using Milease.Utils;
using Milutools.Milutools.UI;
using Milutools.Recycle;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace CircleOfLife.Build.UI
{
    public class BuildingPlaceUI : ManagedUI<BuildingPlaceUI, BuildingPlaceUIData, int>
    {
        private enum UIState
        {
            FoldOut, FoldIn, Placing, Modify
        }

        private enum Effect
        {
            LevelUp, Remove
        }
        
        public static BuildingPlaceUI Instance;

        public SpriteRenderer PlacingIcon;
        public GameObject UICover;
        public MilListView ListView;
        public RectTransform FoldoutButton;
        public RectTransform Container, UpBlack, DownBlack;
        public TMP_Text PlaceTip, MaterialText, EditingTitle;
        public Light2D PlaceLight;
        public CanvasGroup DetailPanel;

        public GameObject PeopleBtn;

        public GameObject LevelUpEffect, RemoveEffect;
        
        public bool PlacingMode { get; set; }
        public int Material { get; set; }
        public BuildingUIData PlacingBuilding { get; set; }
        
        private BuildingPlaceUIData uiData;
        private readonly MilStateAnimator stateAnimator = new();
        private readonly List<GameObject> revertable = new();
        private static readonly Dictionary<GameObject, BuildingUIData> typeDict = new();

        private BuildBase editing;

        private bool finishPlacingKeyUp = false;
        
        protected override void Begin()
        {
            Instance = this;
            RecyclePool.EnsurePrefabRegistered(Effect.LevelUp, LevelUpEffect, 20);
            RecyclePool.EnsurePrefabRegistered(Effect.Remove, RemoveEffect, 20);
            stateAnimator.AddState(UIState.FoldOut, 0.25f, new[]
            {
                Container.MilState(UMN.AnchoredPosition, new Vector2(0f, 214f)),
                FoldoutButton.MilState(UMN.LEulerAngles, new Vector3(0f, 0f, 0f)),
                FoldoutButton.MilState(UMN.LScale, Vector3.one * 1.6667f),
                PlaceTip.MilState(UMN.Color, Color.white.Clear()),
                UpBlack.MilState(UMN.LScale, new Vector3(1f, 0f, 1f)),
                DownBlack.MilState(UMN.LScale, new Vector3(1f, 0f, 1f)),
                Camera.main!.MilState("orthographicSize", 5f, EaseFunction.Linear),
                DetailPanel.MilState(nameof(DetailPanel.alpha), 0f)
            });
            stateAnimator.AddState(UIState.FoldIn, 0.25f, new[]
            {
                Container.MilState(UMN.AnchoredPosition, new Vector2(0f, -131f)),
                FoldoutButton.MilState(UMN.LEulerAngles, new Vector3(0f, 0f, 180f)),
                FoldoutButton.MilState(UMN.LScale, Vector3.one * 2.2f),
                PlaceTip.MilState(UMN.Color, Color.white.Clear()),
                UpBlack.MilState(UMN.LScale, new Vector3(1f, 0f, 1f)),
                DownBlack.MilState(UMN.LScale, new Vector3(1f, 0f, 1f)),
                Camera.main!.MilState("orthographicSize", 5f, EaseFunction.Linear),
                DetailPanel.MilState(nameof(DetailPanel.alpha), 0f)
            });
            stateAnimator.AddState(UIState.Placing, 0.25f, new[]
            {
                Container.MilState(UMN.AnchoredPosition, new Vector2(0f, -200f)),
                FoldoutButton.MilState(UMN.LEulerAngles, new Vector3(0f, 0f, 0f)),
                FoldoutButton.MilState(UMN.LScale, Vector3.one * 1.6667f),
                PlaceTip.MilState(UMN.Color, Color.white),
                UpBlack.MilState(UMN.LScale, new Vector3(1f, 1f, 1f)),
                DownBlack.MilState(UMN.LScale, new Vector3(1f, 1f, 1f)),
                Camera.main!.MilState("orthographicSize", 5f, EaseFunction.Linear),
                DetailPanel.MilState(nameof(DetailPanel.alpha), 0f)
            });
            stateAnimator.AddState(UIState.Modify, 0.25f, new[]
            {
                Container.MilState(UMN.AnchoredPosition, new Vector2(0f, -200f)),
                FoldoutButton.MilState(UMN.LEulerAngles, new Vector3(0f, 0f, 0f)),
                FoldoutButton.MilState(UMN.LScale, Vector3.one * 1.6667f),
                PlaceTip.MilState(UMN.Color, Color.white.Clear()),
                UpBlack.MilState(UMN.LScale, new Vector3(1f, 1f, 1f)),
                DownBlack.MilState(UMN.LScale, new Vector3(1f, 1f, 1f)),
                Camera.main!.MilState("orthographicSize", 4f, EaseFunction.Linear),
                DetailPanel.MilState(nameof(DetailPanel.alpha), 1f)
            });
            stateAnimator.SetDefaultState(UIState.FoldOut);
        }

        public void StartBattle()
        {
            PreStartBattle();
        }

        private void PostStartBattle()
        {
            MessageBox.Open(("开始战斗", "确定要开始战斗吗？\n开始后将暂时不能调整装置布置。"), (o) =>
            {
                if (o == MessageBox.Operation.Deny)
                {
                    return;
                }
                Close(Material);
            });
        }
        
        private void PreStartBattle()
        {
            if (!BuffManager.GetAllStats().Any(x => x.BattleEntity is LogisticsService))
            {
                MessageBox.Open(("警告", "您没有布置任何的【后勤处】，回合结束后，将不会获得任何材料补给，确定要继续吗？"), (o) =>
                {
                    if (o == MessageBox.Operation.Deny)
                    {
                        return;
                    }

                    PostStartBattle();
                });
            }
            else
            {
                PostStartBattle();
            }
        }

        public void CloseDetailPanel()
        {
            stateAnimator.Transition(UIState.FoldOut);
            editing?.CloseRange();
        }

        private int GetLevelUpCost(int level)
        {
            return level * 50 + 50;
        }

        private int GetRemoveCompensate(BuildBase build)
        {
            int cost = 0;
            for (var i = 1; i < build.Level; i++)
            {
                cost += GetLevelUpCost(i);
            }

            if (revertable.Contains(build.gameObject))
            {
                return typeDict[build.gameObject].MetaData.Cost + cost;
            }
            
            return Mathf.RoundToInt((typeDict[build.gameObject].MetaData.Cost + cost) * 0.7f);
        }
        
        public void RemoveBuilding()
        {
            var compensate = GetRemoveCompensate(editing);
            MessageBox.Open(("拆除装置", $"你确定要拆除当前装置吗？\n将返还材料：{compensate}"), (o) =>
            {
                if (o == MessageBox.Operation.Deny)
                {
                    return;
                }

                Material += compensate;
                RecyclePool.Request(Effect.Remove, (c) =>
                {
                    c.Transform.position = editing.transform.position;
                    c.GameObject.SetActive(true);
                });
                revertable.Remove(editing.gameObject);
                RecyclePool.ReturnToPool(editing.gameObject);
                CloseDetailPanel();
            });
        }

        public void LevelUpBuilding()
        {
            if (editing.Level >= 3)
            {
                MessageBox.Open(("已满级", "该装置已经满级，无需升级。"));
                return;
            }

            var need = GetLevelUpCost(editing.Level);
            LevelUpUI.Open(new LevelUpUIData()
            {
                Target = editing,
                Material = Material,
                Need = need
            }, (resp) =>
            {
                if (!resp.Confirm)
                {
                    return;
                }
                editing.LevelUp(resp.Direction?.Value);
                Material -= need;
                RecyclePool.Request(Effect.LevelUp, (c) =>
                {
                    c.Transform.position = editing.transform.position;
                    c.GameObject.SetActive(true);
                });
            });
        }
        
        public void ChangeDirection()
        {
            ChangeDirectionUI.Open(new LevelUpUIData()
            {
                Target = editing,
                Material = Material,
                Need = 0
            }, (resp) =>
            {
                if (!resp.Confirm)
                {
                    return;
                }
                editing.ChangeDirection(resp.Direction?.Value);
                RecyclePool.Request(BuildEffects.NewFriend, (c) =>
                {
                    c.Transform.position = editing.transform.position;
                    c.GameObject.SetActive(true);
                });
            });
        }

        public void FoldOutUI()
        {
            if (stateAnimator.CurrentState == (int)UIState.FoldOut)
            {
                stateAnimator.Transition(UIState.FoldIn);
            }
            else
            {
                if (PlacingMode)
                {
                    EndPlacing();
                }
                stateAnimator.Transition(UIState.FoldOut);
            }
        }
        
        public void StartPlacing(BuildingUIData target)
        {
            if (target.MetaData.Cost > Material)
            {
                return;
            }
            finishPlacingKeyUp = false;
            Cursor.visible = false;
            PlacingBuilding = target;
            PlacingMode = true;
            PlacingIcon.gameObject.SetActive(true);
            PlacingIcon.sprite = target.MetaData.Icon;
            PlacingIcon.transform.localScale = Vector3.one * 0.98f;
            PlacingIcon.transform.localEulerAngles = Vector3.zero;
            
            PlaceTip.text = "按下 鼠标左键 键 <color=green>确认</color> 放置装置，按下 鼠标右键 键 <color=red>取消</color> 放置";
            if (target.MetaData.WhetherRotate)
            {
                PlaceTip.text += "，按下 R 键 <color=yellow>旋转</color> 装置";
            }
            UICover.SetActive(true);
            stateAnimator.Transition(UIState.Placing);
        }

        public void EndPlacing()
        {
            Cursor.visible = true;
            PlacingMode = false;
            PlacingIcon.gameObject.SetActive(false);
            UICover.SetActive(false);
        }
        
        protected override void AboutToClose()
        {
            
        }

        public override void AboutToOpen(BuildingPlaceUIData parameter)
        {
            uiData = parameter;
            PlacingIcon.transform.SetParent(null);
            Material = parameter.AvaliableMaterial;
            foreach (var data in parameter.Buildings)
            {
                ListView.Add(data);
            }
        }
        
        private void UpdatePlacingMode()
        {
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                EndPlacing();
                stateAnimator.Transition(UIState.FoldOut);
                return;
            }

            if (Input.GetKeyUp(KeyCode.R) && PlacingBuilding.MetaData.WhetherRotate)
            {
                PlacingIcon.transform.localEulerAngles = 
                    new Vector3(0f, 0f, 
                        Mathf.Approximately(PlacingIcon.transform.localEulerAngles.z, 0f) ? 90f : 0f);
                if (PlacingBuilding.MetaData.RotatedIcon)
                {
                    PlacingIcon.sprite =
                        Mathf.RoundToInt(PlacingIcon.transform.localEulerAngles.z) % 180 == 0
                            ? PlacingBuilding.MetaData.Icon
                            : PlacingBuilding.MetaData.RotatedIcon;
                }
            }
            
            var size = uiData.MapGrid.cellSize;
            var gridPos = uiData.MapGrid.transform.position;
            var buildSize = PlacingBuilding.MetaData.BuildSize;
            if (Mathf.RoundToInt(PlacingIcon.transform.localEulerAngles.z) % 180 != 0)
            {
                (buildSize.x, buildSize.y) = (buildSize.y, buildSize.x);
            }
            
            var pos = (Vector2)Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            pos.x = Mathf.RoundToInt((pos.x - gridPos.x) / size.x) * size.x + gridPos.x;
            pos.y = Mathf.RoundToInt((pos.y - gridPos.y) / size.y) * size.y + gridPos.y;
            var offset = new Vector2(size.x * (buildSize.x - 1) / 2f, 
                                    size.y * (buildSize.y - 1) / 2f)
                        - (Vector2)size / 2f;
            pos -= offset;
            PlacingIcon.transform.position = pos;
            
            var boxSize = buildSize * size - Vector2.one * 0.1f;
            var colliders = Physics2D.OverlapAreaAll(pos - boxSize / 2f, pos + boxSize / 2f);
            var canPlace = !colliders.Any(x => x.CompareTag("Building") || x.gameObject.layer == 8);
            PlacingIcon.color = canPlace ? Color.white : Color.red;
            PlaceLight.color = PlacingIcon.color;

            if (canPlace && finishPlacingKeyUp)
            {
                finishPlacingKeyUp = false;
                Material -= PlacingBuilding.MetaData.Cost;
                RecyclePool.Request(PlacingBuilding.Type, (c) =>
                {
                    c.Transform.position = pos;
                    c.Transform.localScale = size;
                    c.Transform.localEulerAngles = PlacingIcon.transform.localEulerAngles;
                    if (PlacingBuilding.MetaData.RotatedIcon)
                    {
                        c.GameObject.GetComponent<SpriteRenderer>().sprite =
                            Mathf.RoundToInt(c.Transform.localEulerAngles.z) % 180 == 0
                                ? PlacingBuilding.MetaData.Icon
                                : PlacingBuilding.MetaData.RotatedIcon;
                    }
                    c.GameObject.SetActive(true);
                    revertable.Add(c.GameObject);
                    typeDict.TryAdd(c.GameObject, PlacingBuilding);
                });
                EndPlacing();
                stateAnimator.Transition(UIState.FoldOut);
            }
        }
        
        public void OnClickFullScreen()
        {
            if (PlacingMode)
            {
                finishPlacingKeyUp = true;
            }
            else
            {
                FindSelection();
            }
        }
        
        public void FindSelection()
        {
            var pos = (Vector2)Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            var colliders = Physics2D.RaycastAll(pos, Vector2.zero);
            var build = colliders.FirstOrDefault(x => x.collider.CompareTag("Building"));
            if (!build)
            {
                return;
            }
            stateAnimator.Transition(UIState.Modify);
            editing?.CloseRange();
            editing = build.transform.GetComponent<BuildBase>();
            CameraController.Instance.CameraMode = CameraMoveMode.Follow;
            CameraController.Instance.FollowTarget = build.transform.gameObject;
            var data = typeDict[editing.gameObject];
            EditingTitle.text = data.MetaData.Name;
            PeopleBtn.SetActive(editing.WhetherSelectDirection);
            
            editing.ShowRange();
        }

        private void Update()
        {
            MaterialText.text = Material.ToString();

            if (KeyEnum.Left.IsPressing() || KeyEnum.Right.IsPressing() || KeyEnum.Up.IsPressing() ||
                KeyEnum.Down.IsPressing())
            {
                CameraController.Instance.CameraMode = CameraMoveMode.Free;
            }
            
            if (!PlacingMode)
            {
                return;
            }

            UpdatePlacingMode();
        }
    }
}
