using System;
using CircleOfLife.Build;
using UnityEngine;

namespace CircleOfLife.Experiments
{
    public class BattleTest : MonoBehaviour
    {
        private void Start()
        {
            BuildUtils.EnableAllBuilding();
        }
    }
}
