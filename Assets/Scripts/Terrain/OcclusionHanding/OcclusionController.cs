using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class OcclusionController : MonoBehaviour
    {
        // 根据Y轴改变OrderInLayer
        private string guid;

        void Start()
        {
            ChangeOrderInLayer();
        }
        void Update()
        {
            ChangeOrderInLayer();
        }
        //根据Y轴的位置，更改OrderInLayer
        private void ChangeOrderInLayer()
        {
            int orderInLayer = (int)(transform.position.y * -100);
            GetComponent<SpriteRenderer>().sortingOrder = orderInLayer;
        }
        void OnDestroy()
        {
            OcclusionManager.RemoveOcclusion(guid);
        }

        public string SetGUID()
        {
            if (guid == null)
            {
                guid = UIDCreater.CreatID();
            }
            return guid;
        }
    }
}
