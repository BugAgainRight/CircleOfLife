using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    //根据tag来判断是否需要纳入遮挡管理体系
    public enum OcclusionWhiteTable
    {
        Player,
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
            RegisterAllOcclusions();
        }
        //找到maincamera，挂上CameraOcclusionController
        private static void FindMainCamera()
        {
            Camera mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            mainCamera.gameObject.AddComponent<CameraOcclusionController>();
            //mainCamera.gameObject.AddComponent<TerrianRayCast>();
            mainCamera.gameObject.AddComponent<TerrainMouseClickEventTest>();
        }

        ///<summary
        ///根据白名单中的tag注册场景中所有在物体
        /// </summary>
        private static void RegisterAllOcclusions()
        {
            GameObject[] gameObjects;
            foreach (OcclusionWhiteTable tag in System.Enum.GetValues(typeof(OcclusionWhiteTable)))
            {
                gameObjects = GameObject.FindGameObjectsWithTag(tag.ToString());
                foreach (GameObject gameObject in gameObjects)
                {
                    AddOcclusion(gameObject);
                }
            }
        }
        ///<summary
        ///注册需要纳入遮挡管理体系的物体
        /// </summary>
        public static void RegisterOcclusion(GameObject gameObject)
        {
            AddOcclusion(gameObject);
        }
        /// <summary>
        /// 为存在SpriteRender或者MeshRender的目标添加OcclusionController(用于控制图层大小)，并记录在字典中
        /// </summary>
        /// <param name="gameObject">一个物体</param> 
        private static void AddOcclusion(GameObject gameObject)
        {
            if (!gameObject.GetComponent<SpriteRenderer>() && !gameObject.GetComponent<MeshRenderer>())
            {
                Debug.LogWarning("The gameObject does not have a SpriteRenderer or MeshRenderer component, so it cannot be added to the occlusion system.");
                return;
            }

            if (gameObject.GetComponent<OcclusionController>() == null)
            {
                gameObject.AddComponent<OcclusionController>();
                OcclusionController occlusionController = gameObject.GetComponent<OcclusionController>();
                string guid = occlusionController.SetGUID();
                occlusionDict.Add(guid, gameObject);
            }

        }
        /// <summary>
        /// 删除存放于字典中的目标
        /// </summary>
        /// <param name="guid">可能存在于字典中的guid,通过组件OcclusionController获得</param>
        public static void UnRegisterOcclusion(string guid)
        {
            if (occlusionDict.ContainsKey(guid))
            {
                occlusionDict.Remove(guid);
            }
            occlusionDict.Remove(guid);
        }
        /// <summary>
        /// 删除存放于字典中的目标
        /// </summary>
        /// <param name="gameObject">可能存在于字典中的物体</param>
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
