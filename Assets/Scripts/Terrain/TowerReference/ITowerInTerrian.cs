using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public interface ITowerInTerrian
    {
        //返回鼠标点击到的装置信息
        public static TowerInfo GetTowerInfo()
        {
            return TerrianManager.GetTowerInfo();
        }
        //返回屏幕上指定位置对应的的装置信息
        public static TowerInfo GetTowerInfo(Vector2 orignPositionOnScreen)
        {
            return TerrianManager.GetTowerInfo(orignPositionOnScreen); ;
        }
        //去除原来的装置，创建新的装置
        public static void UpdateTargetTower(TowerInfo lastTowerInfo, TowerInfo currentTowerInfo)
        {
            TerrianManager.UpdateTargetTower(lastTowerInfo, currentTowerInfo);
        }
        //销毁装置
        public static void DestroyTargetTower(TowerInfo towerInfo)
        {
            TerrianManager.DestroyTargetTower(towerInfo);
        }
        //创建装置
        public static void CreateTargetTower(TowerInfo towerInfo)
        {
            TerrianManager.CreateTargetTower(towerInfo);
        }
    }
}
