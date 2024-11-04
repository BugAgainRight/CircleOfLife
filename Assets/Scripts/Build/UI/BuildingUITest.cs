using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CircleOfLife.ScriptObject;
using UnityEngine;

namespace CircleOfLife.Build.UI
{
    public class BuildingUITest : MonoBehaviour
    {
        public List<BuildSoData> TestBuilding;
        public int TestMaterial;
        public Grid MapGrid;

        private void Start()
        {
            BuildingPlaceUI.Open(new BuildingPlaceUIData()
            {
                Buildings = TestBuilding,
                MapGrid = MapGrid,
                AvaliableMaterial = TestMaterial
            });
        }
    }
}
