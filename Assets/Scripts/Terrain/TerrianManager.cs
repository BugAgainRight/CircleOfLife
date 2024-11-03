using System.Collections;
using System.Collections.Generic;
using CircleOfLife.Key;
using UnityEngine;

namespace CircleOfLife
{

    public static class TerrianManager
    {

        //从鼠标处发射射线，获取射线触碰的所有
        public static void SetRayCastFromMousePos()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);
            //ToDo 对于特殊碰撞体给予高亮提示
        }


        //在碰撞体中筛选出一个tag为Tower的碰撞体,打包成一个类TowerInfo,并返回
        private static TowerInfo SetTowerInfo(RaycastHit2D[] hits, Vector2 orignPosOnScreen)
        {
            foreach (RaycastHit2D hit in hits)
            {
                foreach (TowerTag towerTag in System.Enum.GetValues(typeof(TowerTag)))
                {
                    if (hit.collider.tag == towerTag.ToString())
                    {
                        return new TowerInfo(hit.collider.gameObject, orignPosOnScreen);
                    }
                }
            }

            return null;
        }
        //将TowerInfo传递给TowerManager
        //默认在鼠标位置上发射射线
        public static TowerInfo GetTowerInfo()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);
            return SetTowerInfo(hits, Input.mousePosition);
        }
        //从屏幕上指定位置发射射线
        public static TowerInfo GetTowerInfo(Vector2 orignPositionOnScreen)
        {
            Vector3 orignPosOnWorld = Camera.main.ScreenToWorldPoint(orignPositionOnScreen);
            RaycastHit2D[] hits = Physics2D.RaycastAll(orignPosOnWorld, Vector2.zero);
            return SetTowerInfo(hits, orignPositionOnScreen);
        }

        public static void DestroyTargetTower(TowerInfo towerInfo)
        {
            OcclusionManager.RemoveOcclusion(towerInfo.GameObject);
            GameObject.Destroy(towerInfo.GameObject);
        }

        public static void CreateTargetTower(TowerInfo towerInfo)
        {
            OcclusionManager.AddOcclusion(towerInfo.GameObject);
            GameObject.Instantiate(towerInfo.GameObject);
        }

        public static void UpdateTargetTower(TowerInfo lastTowerInfo, TowerInfo currentTowerInfo)
        {
            DestroyTargetTower(currentTowerInfo);
            CreateTargetTower(lastTowerInfo);
        }
    }
}
