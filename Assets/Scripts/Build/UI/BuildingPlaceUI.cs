using System;
using System.Collections.Generic;
using System.Linq;
using CircleOfLife.ScriptObject;
using Milease.Core.Animator;
using Milease.Core.UI;
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
        public enum UIState
        {
            FoldOut, FoldIn, Placing
        }
        
        public static BuildingPlaceUI Instance;

        public SpriteRenderer PlacingIcon;
        public GameObject UICover;
        public MilListView ListView;
        public RectTransform FoldoutButton;
        public RectTransform Container, UpBlack, DownBlack;
        public TMP_Text PlaceTip;
        public Light2D PlaceLight;
        
        public bool PlacingMode { get; set; }
        public int Material { get; set; }
        public BuildingUIData PlacingBuilding { get; set; }
        
        private BuildingPlaceUIData uiData;
        private readonly MilStateAnimator stateAnimator = new();
        private readonly List<GameObject> revertable = new();
        
        protected override void Begin()
        {
            Instance = this;
            stateAnimator.AddState(UIState.FoldOut, 0.25f, new[]
            {
                Container.MilState(UMN.AnchoredPosition, new Vector2(0f, 214f)),
                FoldoutButton.MilState(UMN.LEulerAngles, new Vector3(0f, 0f, 0f)),
                FoldoutButton.MilState(UMN.LScale, Vector3.one * 1.6667f),
                PlaceTip.MilState(UMN.Color, Color.white.Clear()),
                UpBlack.MilState(UMN.LScale, new Vector3(1f, 0f, 1f)),
                DownBlack.MilState(UMN.LScale, new Vector3(1f, 0f, 1f))
            });
            stateAnimator.AddState(UIState.FoldIn, 0.25f, new[]
            {
                Container.MilState(UMN.AnchoredPosition, new Vector2(0f, -131f)),
                FoldoutButton.MilState(UMN.LEulerAngles, new Vector3(0f, 0f, 180f)),
                FoldoutButton.MilState(UMN.LScale, Vector3.one * 2.2f),
                PlaceTip.MilState(UMN.Color, Color.white.Clear()),
                UpBlack.MilState(UMN.LScale, new Vector3(1f, 0f, 1f)),
                DownBlack.MilState(UMN.LScale, new Vector3(1f, 0f, 1f))
            });
            stateAnimator.AddState(UIState.Placing, 0.25f, new[]
            {
                Container.MilState(UMN.AnchoredPosition, new Vector2(0f, -180f)),
                FoldoutButton.MilState(UMN.LEulerAngles, new Vector3(0f, 0f, 0f)),
                FoldoutButton.MilState(UMN.LScale, Vector3.one * 1.6667f),
                PlaceTip.MilState(UMN.Color, Color.white),
                UpBlack.MilState(UMN.LScale, new Vector3(1f, 1f, 1f)),
                DownBlack.MilState(UMN.LScale, new Vector3(1f, 1f, 1f))
            });
            stateAnimator.SetDefaultState(UIState.FoldOut);
        }

        public void StartBattle()
        {
            Close(Material);
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
                });
                EndPlacing();
                stateAnimator.Transition(UIState.FoldOut);
            }
        }

        private void Update()
        {
            if (!PlacingMode)
            {
                return;
            }

            UpdatePlacingMode();
        }
    }
}
