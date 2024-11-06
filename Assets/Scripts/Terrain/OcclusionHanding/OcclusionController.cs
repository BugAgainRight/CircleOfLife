using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public enum GetOrderInLayerMode
    {
        SpriteRenderer,
        MeshRenderer,
        None
    }
    public class OcclusionController : MonoBehaviour
    {
        // 根据Y轴改变OrderInLayer，只对存在SpriteRenderer和MeshRenderer的物体有效
        private GetOrderInLayerMode getOrderInLayerMode;
        private string guid;
        void Start()
        {
            TryGetOrderInLayer();
            ChangeOrderInLayer();
        }
        void FixedUpdate()
        {
            ChangeOrderInLayer();
        }

        //根据Y轴的位置，更改OrderInLayer
        private void ChangeOrderInLayer()
        {
            switch (getOrderInLayerMode)
            {
                case GetOrderInLayerMode.SpriteRenderer:
                    ChangeOrderInLayerBySpriteRenderer();
                    break;
                case GetOrderInLayerMode.MeshRenderer:
                    ChangeOrderInLayerByMeshRenderer();
                    break;
                case GetOrderInLayerMode.None:
                    break;
            }
        }

        private void ChangeOrderInLayerByMeshRenderer()
        {
            int orderInLayer = (int)(transform.position.y * -10);
            GetComponent<MeshRenderer>().sortingOrder = orderInLayer;
        }

        private void ChangeOrderInLayerBySpriteRenderer()
        {
            int orderInLayer = (int)(transform.position.y * -10);
            GetComponent<SpriteRenderer>().sortingOrder = orderInLayer;
        }

        //判断是否存在SpriteRenderer或MeshRenderer
        private void TryGetOrderInLayer()
        {
            SpriteRenderer spriteRenderer;
            MeshRenderer meshRenderer;
            if (TryGetComponent<SpriteRenderer>(out spriteRenderer))
            {
                getOrderInLayerMode = GetOrderInLayerMode.SpriteRenderer;
            }
            else if (TryGetComponent<MeshRenderer>(out meshRenderer))
            {
                getOrderInLayerMode = GetOrderInLayerMode.MeshRenderer;
            }
            else
            {
                getOrderInLayerMode = GetOrderInLayerMode.None;
            }
        }

        void OnDestroy()
        {
            OcclusionManager.UnRegisterOcclusion(guid);
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
