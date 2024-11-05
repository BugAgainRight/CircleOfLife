using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CircleOfLife.ScriptObject;
using UnityEngine;

namespace CircleOfLife.Build.UI
{
    public class BuildingUITest : MonoBehaviour
    {
        public int TestMaterial;
        public Grid MapGrid;

        private void Start()
        {
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
                AvaliableMaterial = TestMaterial
            });
        }
    }
}
