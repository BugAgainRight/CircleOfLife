using System;
using System.Collections.Generic;
using System.Linq;
using CircleOfLife.General;
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

        public GameObject RotateBtn, PeopleBtn;

        public GameObject LevelUpEffect, RemoveEffect;
        
        public bool PlacingMode { get; set; }
        public int Material { get; set; }
        public BuildingUIData PlacingBuilding { get; set; }
        
        private BuildingPlaceUIData uiData;
        private readonly MilStateAnimator stateAnimator = new();
        private readonly List<GameObject> revertable = new();
        private static readonly Dictionary<GameObject, BuildingUIData> typeDict = new();

        private BuildBase editing;
        
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
            Close(Material);
        }

        public void CloseDetailPanel()
        {
            stateAnimator.Transition(UIState.FoldOut);
        }

        private int GetLevelUpCost(int level)
        {
            return level * 50 + 50;
        }

        private int GetRemoveCompensate(BuildBase build)
        {
            if (revertable.Contains(build.gameObject))
            {
                return typeDict[build.gameObject].MetaData.Cost;
            }

            int cost = 0;
            for (var i = 1; i < build.Level; i++)
            {
                cost += GetLevelUpCost(i);
            }

            return Mathf.RoundToInt(Mathf.Log(cost, build.Level));
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

        public void RotateBuilding()
        {
            
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
            }, (direction) =>
            {
                if (direction == null)
                {
                    return;
                }
                editing.LevelUp(direction.Value);
                Material -= need;
                RecyclePool.Request(Effect.LevelUp, (c) =>
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
            Cursor.visible = false;
            PlacingBuilding = target;
            PlacingMode = true;
            PlacingIcon.gameObject.SetActive(true);
            PlacingIcon.sprite = target.MetaData.Icon;
            PlacingIcon.transform.localScale = Vector3.one * 0.98f;
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
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                EndPlacing();
                stateAnimator.Transition(UIState.FoldOut);
                return;
            }
            
            var size = uiData.MapGrid.cellSize;
            var gridPos = uiData.MapGrid.transform.position;
            var pos = (Vector2)Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            pos.x = Mathf.RoundToInt((pos.x - gridPos.x) / size.x) * size.x + gridPos.x;
            pos.y = Mathf.RoundToInt((pos.y - gridPos.y) / size.y) * size.y + gridPos.y;
            var offset = new Vector2(size.x * (PlacingBuilding.MetaData.BuildSize.x - 1) / 2f, 
                                    size.y * (PlacingBuilding.MetaData.BuildSize.y - 1) / 2f)
                        - (Vector2)size / 2f;
            pos -= offset;
            PlacingIcon.transform.position = pos;
            
            var boxSize = PlacingBuilding.MetaData.BuildSize * size - Vector2.one * 0.1f;
            var colliders = Physics2D.OverlapAreaAll(pos - boxSize / 2f, pos + boxSize / 2f);
            var canPlace = !colliders.Any(x => x.CompareTag("Building") || x.gameObject.layer == 8);
            PlacingIcon.color = canPlace ? Color.white : Color.red;
            PlaceLight.color = PlacingIcon.color;

            if (canPlace && Input.GetKeyUp(KeyCode.Return))
            {
                Material -= PlacingBuilding.MetaData.Cost;
                RecyclePool.Request(PlacingBuilding.Type, (c) =>
                {
                    c.Transform.position = pos;
                    c.Transform.localScale = size;
                    c.GameObject.SetActive(true);
                    revertable.Add(c.GameObject);
                    typeDict.TryAdd(c.GameObject, PlacingBuilding);
                });
                EndPlacing();
                stateAnimator.Transition(UIState.FoldOut);
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
            editing = build.transform.GetComponent<BuildBase>();
            CameraController.Instance.FollowTarget = build.transform.gameObject;
            var data = typeDict[editing.gameObject];
            EditingTitle.text = data.MetaData.Name;
            RotateBtn.SetActive(data.MetaData.WhetherRotate);
            PeopleBtn.SetActive(editing.WhetherSelectDirection);
        }

        private void Update()
        {
            MaterialText.text = Material.ToString();
            
            if (!PlacingMode)
            {
                return;
            }

            UpdatePlacingMode();
        }
    }
}
