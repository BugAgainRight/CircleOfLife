using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public static class TowerInTerrianUtils
    {
        /// <summary>
        /// 返回鼠标点击到的装置信息
        /// </summary>
        public static TowerInfo GetTowerInfo()
        {
            return TerrianManager.GetTowerInfo();
        }
        /// <summary>
        /// 返回屏幕上指定位置对应的的装置信息
        /// </summary> 
        /// <param name="orignPositionOnScreen">屏幕上的位置</param> 
        public static TowerInfo GetTowerInfo(Vector2 orignPositionOnScreen)
        {
            return TerrianManager.GetTowerInfo(orignPositionOnScreen); ;
        }

        /// <summary>
        /// 去除原来的装置，创建新的装置
        /// </summary> 
        /// <param name="lastTowerInfo">新的装置(一定要定义其中的GameObject和TargetPos)</param>
        /// <param name="currentTowerInfo">原来的装置</param>
        public static void UpdateTargetTower(TowerInfo lastTowerInfo, TowerInfo currentTowerInfo)
        {
            TerrianManager.UpdateTargetTower(lastTowerInfo, currentTowerInfo);
        }
        /// <summary>
        /// 销毁装置
        /// </summary> 
        /// <param name="towerInfo">需要删除的装置(只会用到GameObject)</param>
        public static void DestroyTargetTower(TowerInfo towerInfo)
        {
            TerrianManager.DestroyTargetTower(towerInfo);
        }
        /// <summary>
        /// 创建
        /// </summary> 
        /// <param name="towerInfo">需要创建的装置(会用到其中的GameObject、TargetInfo)</param>
        public static void CreateTargetTower(TowerInfo towerInfo)
        {
            TerrianManager.CreateTargetTower(towerInfo);
        }
    }
}
