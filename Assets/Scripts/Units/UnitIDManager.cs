using System;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife.Units
{
    public class UnitIDManager
    {
        #region Identity
        //idAccount<ID,GameObject>
        private static Dictionary<string, GameObject> idAccountDict = new Dictionary<string, GameObject>();

        public static string CreatID(BaseNPC mbo)
        {

            string id = Guid.NewGuid().ToString();
            idAccountDict.Add(id, mbo.gameObject);
            return id;
        }

        public static void DestroyID(string id)
        {
            try
            {
                idAccountDict.Remove(id);
            }
            catch
            {
                Debug.Log("id:" + id + "doesn't exist");
            }
        }
        #endregion

        #region MapIcon
        private static string basePath = "Prefabs/Map/MapIcon/";
        public static Dictionary<string, string> MapIconDict = new Dictionary<string, string>(){
            {"Good", basePath +"Good"},
        };
        #endregion

        #region Bullet
        private static string bulletBasePath = "Prefabs/Map/Bullet/";
        public static Dictionary<string, string> BulletDict = new Dictionary<string, string>(){
            {"BaseBullet", bulletBasePath + "BaseBullet"},
        };
        #endregion
    }
}