using System;
using System.Collections;
using System.Collections.Generic;
using CircleOfLife.General;
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
        public GameObject Cover;
        public Image Icon;
        public TMP_Text Cost, Title;
        
        protected override IEnumerable<MilStateParameter> ConfigDefaultState()
            => ArraySegment<MilStateParameter>.Empty;

        protected override IEnumerable<MilStateParameter> ConfigSelectedState()
            => ArraySegment<MilStateParameter>.Empty;

        public override void OnSelect(PointerEventData eventData)
        {
            var data = (BuildingUIData)Binding;
            if (BuildingPlaceUI.Instance.Material < data.MetaData.Cost)
            {
                MessageBox.Open(("材料不足", "材料不足哦，无法布置这个装置。"));
                return;
            }
            BuildingPlaceUI.Instance.StartPlacing(data);
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
            var data = (BuildingUIData)Binding;
            var tex = data.MetaData.Icon.texture;
            Title.text = data.MetaData.Name;
            Cost.text = data.MetaData.Cost.ToString();
            Icon.sprite = data.MetaData.Icon;
            if (tex.height > tex.width)
            {
                Icon.rectTransform.sizeDelta = new Vector2(150f / tex.height * tex.width, 150f);
            }
            else
            {
                Icon.rectTransform.sizeDelta = new Vector2(150f, 150f / tex.width * tex.height);
            }
        }

        public void ShowDescription()
        {
            var data = (BuildingUIData)Binding;
            MessageBox.Open((data.MetaData.Name + " - 说明", data.MetaData.Description));
        }
        
        public override void AdjustAppearance(float pos)
        {

        }

        private void Update()
        {
            var data = (BuildingUIData)Binding;
            if (data == null)
            {
                return;
            }
            Cost.color = BuildingPlaceUI.Instance.Material < data.MetaData.Cost ? Color.red : Color.white;
            Cover.SetActive(BuildingPlaceUI.Instance.Material < data.MetaData.Cost);
        }
    }
}
