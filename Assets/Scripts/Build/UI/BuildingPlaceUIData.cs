using System.Collections.Generic;
using CircleOfLife.ScriptObject;
using UnityEngine;

namespace CircleOfLife.Build.UI
{
    public class BuildingPlaceUIData
    {
        public List<BuildSoData> Buildings = new();
        public int AvaliableMaterial;
        public Grid MapGrid;
    }
}
