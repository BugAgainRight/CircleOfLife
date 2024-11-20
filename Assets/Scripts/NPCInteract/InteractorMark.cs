using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CircleOfLife.NPCInteract
{
    /// <summary>
    /// 用于创建和显示交互图标，返回交互目标
    /// </summary>
    public class InteractorMark : MonoBehaviour
    {
        public GameObject icon;
        /// <summary>
        /// 一般指玩家,找到相关Layer的物体则显示交互图标
        /// </summary>
        public LayerMask PlayerLayer;
        public float InteractableRange = 1.5f;
        [HideInInspector]
        public bool IsFindTarget = false;

        public bool ShowInteractableRange = false;
        [HideInInspector]
        public string id;
        private float UpdateTime = 0.1f;
        private float currentTime;

        void Awake()
        {
            id = Guid.NewGuid().ToString();
        }
        // Start is called before the first frame update
        void Start()
        {
            CreateIcon();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (currentTime < UpdateTime)
            {
                currentTime += Time.deltaTime;
            }
            else
            {
                currentTime = 0;
                UpdateTarget();
            }
        }

        public void CreateIcon()
        {
            GameObject iconPre = Resources.Load<GameObject>(InteractorManager.IconPrePath);
            icon = Instantiate(iconPre, this.transform);
            icon.GetComponent<InteractableIcon>().Initialize();
            icon.SetActive(false);
        }

        public void UpdateTarget()
        {
            List<Collider2D> mid = Physics2D.OverlapCircleAll(this.transform.position, InteractableRange, PlayerLayer).ToList();
            if (mid.Count > 0)
            {
                if (!IsFindTarget || !InteractorManager.InteractableTarget)
                {
                    InteractorManager.UpdateInteractableTarget(this);
                    icon.SetActive(true);
                    IsFindTarget = true;
                }
            }
            else
            {
                InteractorManager.RemoveInteractableTarget(this);
                IsFindTarget = false;
                icon.SetActive(false);
            }

        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (!ShowInteractableRange) return;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(this.transform.position, InteractableRange);
#endif
        }
    }
}
