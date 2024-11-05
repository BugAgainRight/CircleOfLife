using System;
using System.Linq;
using CircleOfLife.ScriptObject;
using Milease.Core.Animator;
using Milease.Core.UI;
using Milease.Utils;
using Milutools.Milutools.UI;
using UnityEngine;
using UnityEngine.UI;

namespace CircleOfLife.Build.UI
{
    public class BuildingPlaceUI : ManagedUI<BuildingPlaceUI, BuildingPlaceUIData, int>
    {
        public enum UIState
        {
            FoldOut, FoldIn
        }
        
        public static BuildingPlaceUI Instance;

        public SpriteRenderer PlacingIcon;
        public GameObject UICover;
        public MilListView ListView;
        public RectTransform FoldoutButton;
        public RectTransform Container;
        
        public bool PlacingMode { get; set; }
        public int Material { get; set; }
        public BuildSoData PlacingBuilding { get; set; }
        
        private BuildingPlaceUIData uiData;
        private MilStateAnimator stateAnimator = new();
        
        protected override void Begin()
        {
            Instance = this;
            stateAnimator.AddState(UIState.FoldOut, 0.5f, new[]
            {
                Container.MilState(UMN.AnchoredPosition, new Vector2(0f, 214f)),
                FoldoutButton.MilState(UMN.LEulerAngles, new Vector3(0f, 0f, 0f)),
                FoldoutButton.MilState(UMN.LScale, Vector3.one * 1.6667f)
            });
            stateAnimator.AddState(UIState.FoldIn, 0.5f, new[]
            {
                Container.MilState(UMN.AnchoredPosition, new Vector2(0f, -131f)),
                FoldoutButton.MilState(UMN.LEulerAngles, new Vector3(0f, 0f, 180f)),
                FoldoutButton.MilState(UMN.LScale, Vector3.one * 2.2f)
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
        
        public void StartPlacing(BuildSoData target)
        {
            PlacingBuilding = target;
            PlacingMode = true;
            PlacingIcon.gameObject.SetActive(true);
            PlacingIcon.sprite = target.Icon;
            PlacingIcon.transform.localScale = Vector3.one * 0.98f;
            UICover.SetActive(true);
            stateAnimator.Transition(UIState.FoldIn);
        }

        public void EndPlacing()
        {
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

        private void Update()
        {
            if (!PlacingMode)
            {
                return;
            }

            var size = uiData.MapGrid.cellSize;
            var pos = (Vector2)Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            pos.x = Mathf.CeilToInt(pos.x / size.x) * size.x;
            pos.y = Mathf.CeilToInt(pos.y / size.y) * size.y;
            pos.x -= PlacingBuilding.BuildSize.x * size.x / 2f;
            pos.y -= PlacingBuilding.BuildSize.y * size.y / 2f;
            PlacingIcon.transform.position = pos;

            var boxSize = PlacingBuilding.BuildSize * 0.98f - Vector2.one * 0.08f;
            var colliders = Physics2D.OverlapAreaAll(pos - boxSize / 2f, pos + boxSize / 2f);
            if (colliders.Any(x => x.CompareTag("Building") || x.gameObject.layer == 8))
            {
                PlacingIcon.color = Color.red;
            }
            else
            {
                PlacingIcon.color = Color.white;
            }
        }
    }
}
