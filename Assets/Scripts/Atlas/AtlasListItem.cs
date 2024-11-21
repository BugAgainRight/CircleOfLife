using System.Collections;
using System.Collections.Generic;
using CircleOfLife.Atlas;
using Spine.Unity;
using UnityEngine;

namespace CircleOfLife.Atlas
{
    public class AtlasListItem : MonoBehaviour
    {
        public SkeletonGraphic SkeletonGraphic;
        public AtlasUIData Data;

        public void Select()
        {
            if (Data.Unlocked)
            {
                AtlasUI.Instance.Title.text = Data.Atlas.Title;
                AtlasUI.Instance.Description.text = Data.Atlas.Description;
            }
            else
            {
                AtlasUI.Instance.Title.text = "？？？";
                AtlasUI.Instance.Description.text = "尚未解锁图鉴";
            }

            AtlasUI.Instance.SkeletonGraphic.skeletonDataAsset = Data.Atlas.SkeletonData;
            AtlasUI.Instance.SkeletonGraphic.Initialize(true);
            AtlasUI.Instance.SkeletonGraphic.AnimationState.SetAnimation(0, "walk", true);
            AtlasUI.Instance.SkeletonGraphic.color = Data.Unlocked ? Color.white : Color.black;
        }
    }
}
