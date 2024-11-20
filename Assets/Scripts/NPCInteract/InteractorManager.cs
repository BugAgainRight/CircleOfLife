using System.Collections;
using System.Collections.Generic;
using Milutools.Recycle;
using UnityEngine;

namespace CircleOfLife.NPCInteract
{
    /// <summary>
    /// 注意给能够交互的目标添加一个交互标记InteractorMark
    /// </summary>
    public static class InteractorManager
    {
        public static InteractorMark InteractableTarget;
        public static string IconPrePath = "Prefabs/TestIcon";
        public static void UpdateInteractableTarget(InteractorMark interactorMark)
        {
            if (InteractableTarget)
            {
                InteractableTarget.icon.SetActive(false);
            }
            InteractableTarget = interactorMark;
        }

        public static void RemoveInteractableTarget(InteractorMark interactorMark)
        {
            if (InteractableTarget == interactorMark)
            {
                InteractableTarget = null;
            }
        }
        /// <summary>
        /// 获取当前交互目标
        /// </summary>
        /// <returns>返回当前交互目标</returns>
        public static GameObject GetInteractableTarget()
        {
            if (InteractableTarget)
            {
                return InteractableTarget.gameObject;
            }
            return null;
        }
    }
}
