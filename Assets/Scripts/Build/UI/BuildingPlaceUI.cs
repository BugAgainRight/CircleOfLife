using System;
using System.Linq;
using CircleOfLife.ScriptObject;
using Milease.Core.UI;
using Milutools.Milutools.UI;
using UnityEngine;
using UnityEngine.UI;

namespace CircleOfLife.Build.UI
{
    public class BuildingPlaceUI : ManagedUI<BuildingPlaceUI, BuildingPlaceUIData, int>
    {
        public static BuildingPlaceUI Instance;

        public SpriteRenderer PlacingIcon;
        
        public bool PlacingMode = false;
        public GameObject UICover;
        
        public int Material;
        public MilListView ListView;

        public BuildSoData PlacingBuilding;

        private BuildingPlaceUIData uiData;
        
        protected override void Begin()
        {
            Instance = this;
        }

        public void StartBattle()
        {
            Close(Material);
        }

        public void StartPlacing(BuildSoData target)
        {
            PlacingBuilding = target;
            PlacingMode = true;
            PlacingIcon.gameObject.SetActive(true);
            PlacingIcon.sprite = target.Icon;
            PlacingIcon.transform.localScale = Vector3.one * 0.98f;
            UICover.SetActive(true);
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
            pos.x = Mathf.RoundToInt(pos.x / size.x) * size.x;
            pos.y = Mathf.RoundToInt(pos.y / size.y) * size.y;
            pos.x -= PlacingBuilding.BuildSize.x * size.x / 2f;
            pos.y -= PlacingBuilding.BuildSize.y * size.y / 2f;
            PlacingIcon.transform.position = pos;

            var boxSize = PlacingBuilding.BuildSize * 0.9f;
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
