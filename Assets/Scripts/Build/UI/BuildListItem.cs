using System;
using System.Collections;
using System.Collections.Generic;
using CircleOfLife.ScriptObject;
using Milease.Core.Animator;
using Milease.Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CircleOfLife.Build.UI
{
    public class BuildListItem : MilListViewItem
    {
        public Image Icon;
        public TMP_Text Cost, Title;
        
        protected override IEnumerable<MilStateParameter> ConfigDefaultState()
            => ArraySegment<MilStateParameter>.Empty;

        protected override IEnumerable<MilStateParameter> ConfigSelectedState()
            => ArraySegment<MilStateParameter>.Empty;

        public override void OnSelect(PointerEventData eventData)
        {
            BuildingPlaceUI.Instance.StartPlacing((BuildSoData)Binding);
        }

        protected override void OnInitialize()
        {
            
        }

        protected override void OnTerminate()
        {

        }

        protected override MilInstantAnimator ConfigClickAnimation()
            => null;

        public override void UpdateAppearance()
        {
            var data = (BuildSoData)Binding;
            Title.text = "标题";
            Cost.text = data.Cost.ToString();
            Icon.sprite = data.Icon;
        }

        public override void AdjustAppearance(float pos)
        {

        }
    }
}
