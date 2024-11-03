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
        private SpriteRenderer spriteRenderer;
        private new Rigidbody2D rigidbody2D;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            TryGetComponent<Rigidbody2D>(out rigidbody2D);
        }

        void Start()
        {
            ChangeOrderInLayer();
        }
        void Update()
        {
            if (rigidbody2D)
            {
                ChangeOrderInLayer();
            }
        }
        //根据Y轴的位置，更改OrderInLayer
        public void ChangeOrderInLayer()
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

        public void SetColorAlpha()
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.b, spriteRenderer.color.a, 0.5f);
        }
        public void ResetColorAlpha()
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.b, spriteRenderer.color.a, 1f);
        }
    }
}
