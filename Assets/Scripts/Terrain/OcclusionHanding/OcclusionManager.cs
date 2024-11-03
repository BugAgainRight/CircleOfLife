using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    //根据tag来判断是否需要注册
    public enum OcclusionWhiteTable
    {
        Player,
        test
    }

    public class OcclusionManager : MonoBehaviour
    {
        //该脚本适用于tilemap以外的地图中的物体
        //对于tilemap，建议将其分割成多个tilemap，将一个障碍物按照最高OrderInLayer和最低OrderInLayer拆分，最后虚空制作空气墙(避免穿帮)进行阻挡
        //遍历所有的物体，将白名单中的物体添加到字典中
        //为所有子物体添加一个脚本，用于更改OrderInLayer
        //为所有子物体添加一个脚本，用于更改透明度
        // Start is called before the first frame update
        private static Dictionary<string, GameObject> occlusionDict = new Dictionary<string, GameObject>();

        void Start()
        {
            FindMainCamera();
            Register();
        }
        //找到maincamera，挂上CameraOcclusionController
        public static void FindMainCamera()
        {
            Camera mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            mainCamera.gameObject.AddComponent<CameraOcclusionController>();
            //mainCamera.gameObject.AddComponent<TerrianRayCast>();
            mainCamera.gameObject.AddComponent<TerrainMouseClickEventTest>();
        }


        //将场景中所有在白名单中的物体注册
        public static void Register()
        {
            GameObject[] gameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (GameObject gameObject in gameObjects)
            {
                foreach (OcclusionWhiteTable occlusionWhiteTable in System.Enum.GetValues(typeof(OcclusionWhiteTable)))
                {
                    if (gameObject.CompareTag(occlusionWhiteTable.ToString()))
                    {
                        AddOcclusion(gameObject);
                    }
                }

            }
        }

        public static void AddOcclusion(GameObject gameObject)
        {
            if (gameObject.GetComponent<SpriteRenderer>() == null)
            {
                return;
            }
            if (gameObject.GetComponent<OcclusionController>() != null)
            {
                gameObject.AddComponent<OcclusionController>();
            }
            OcclusionController occlusionController = gameObject.AddComponent<OcclusionController>();
            occlusionController.ChangeOrderInLayer();
            string guid = occlusionController.SetGUID();
            occlusionDict.Add(guid, gameObject);
        }

        public static void RemoveOcclusion(string guid)
        {
            if (occlusionDict.ContainsKey(guid))
            {
                occlusionDict.Remove(guid);
            }
            occlusionDict.Remove(guid);
        }
        public static void RemoveOcclusion(GameObject gameObject)
        {
            OcclusionController occlusionController;
            if (gameObject.TryGetComponent(out occlusionController))
            {
                string guid = occlusionController.SetGUID();
                if (occlusionDict.ContainsKey(guid))
                {
                    occlusionDict.Remove(guid);
                }
                occlusionDict.Remove(guid);
            }
        }
    }
}
