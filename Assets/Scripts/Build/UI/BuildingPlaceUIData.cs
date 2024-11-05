using System.Collections.Generic;
using CircleOfLife.ScriptObject;
using UnityEngine;

namespace CircleOfLife.Build.UI
{
    public class BuildingUIData
    {
        public BuildSoData MetaData;
        public BuildStat Type;
    }
    public class BuildingPlaceUIData
    {
        public List<BuildingUIData> Buildings = new();
        public int AvaliableMaterial;
        public Grid MapGrid;
    }
}
