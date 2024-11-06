using System.Collections.Generic;
using Milease.Core.Animator;
using Milease.Core.UI;
using Milease.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CircleOfLife.Build.UI
{
    public class DirectionListItem : MilListViewItem
    {
        public TMP_Text Content;
        public Image Mark;

        protected override IEnumerable<MilStateParameter> ConfigDefaultState()
            => new[]
            {
                Mark.MilState(UMN.Color, Color.white.Clear())
            };

        protected override IEnumerable<MilStateParameter> ConfigSelectedState()
            => new[]
            {
                Mark.MilState(UMN.Color, Color.white)
            };

        public override void OnSelect(PointerEventData eventData)
        {
            
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
            Content.text = ((BuildBase.LevelUpDirection)Binding).Title;
        }

        public override void AdjustAppearance(float pos)
        {

        }
    }
}
