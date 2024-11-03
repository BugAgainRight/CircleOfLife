using System;
using System.Collections;
using System.Collections.Generic;
using CircleOfLife.General;
using UnityEngine;

namespace CircleOfLife
{
    public enum LYMTestMouseClickMode
    {
        use,
        unuse,

    }
    public class TerrainMouseClickEventTest : MonoBehaviour
    {
        public LYMTestMouseClickMode lYMTestMouseClickMode = LYMTestMouseClickMode.unuse;
        private bool isMouseClick = false;
        void Start()
        {

        }
        void Update()
        {
            if (!isMouseClick)
            {
                OnMouseClick();
            }
        }
        public void OnMouseClick()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                isMouseClick = true;
                TowerInfo towerInfo = ITowerInTerrian.GetTowerInfo();
                if (towerInfo == null)
                {
                    Debug.Log("You Clicked Nothing");
                }
                else
                {
                    Debug.Log("You have Clicked:"
                + towerInfo.GameObject.name + '\n'
                + "MousePosition:"
                + towerInfo.MousePosition
                + '\n'
                + "TargetPos:"
                + towerInfo.TargetPos);

                }

                isMouseClick = false;
            }
        }

    }
}
