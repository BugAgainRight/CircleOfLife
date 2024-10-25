using System;
using System.Collections;
using System.Collections.Generic;
using Milease.Core.Animator;
using Milease.Core.UI;
using Milease.Enums;
using Milease.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CircleOfLife.Sample
{
    // UI 列表功能示例
    // 使用 UI 列表，首先需要实现一个列表项的控制类，如下
    // 这个控制脚本要挂在 列表项物体上
    public class ListItemSample : MilListViewItem // 实现 抽象类 MilListViewItem
    {
        /**
         * 这个列表封装的局限性
         * 1. 列表项必须是等宽或者等高的
         * 2. 目前仅支持横向和纵向排布，不支持Grid布局
         */
        public Color ActiveColor;
        public Image Background;
        public TMP_Text Content;
        public RectTransform ContentRect;
        
        // 设置默认的列表项样式（不需要随选中状态改变外观的话，可以直接返回空数组）
        protected override IEnumerable<MilStateParameter> ConfigDefaultState()
            => new[]
            {
                // 这里的 Color.Clear() 也是框架里面对 Color 类型的扩展函数，表示将当前颜色的alpha设置为 0
                Background.MilState(UMN.Color, ActiveColor.Clear()),
                Content.MilState(nameof(Content.characterSpacing), 0f),
                Content.MilState(UMN.Color, Color.black)
            };

        // 设置被选中的样式，会自动在默认样式和选中样式中平滑过渡样式
        protected override IEnumerable<MilStateParameter> ConfigSelectedState()
            => new[]
            {
                Background.MilState(UMN.Color, ActiveColor),
                Content.MilState(nameof(Content.characterSpacing), 30f, EaseFunction.Back, EaseType.Out),
                Content.MilState(UMN.Color, Color.white)
            };

        public override void OnSelect(PointerEventData eventData)
        {
            // 选中时触发
            if (ListSample.CurrentSelected != null)
            {
                ListSample.CurrentSelected.LinkController.SetActive(false);
            }

            var data = (ListItemDataSample)Binding;
            data.LinkController.SetActive(true);
            
            ListSample.CurrentSelected = data;
        }

        protected override void OnInitialize()
        {
            // 初始化时触发（新列表项加入时不会触发，这个只能用于初始化控制器）
        }

        protected override void OnTerminate()
        {
            // 被销毁时触发（移除列表项并不会触发这个函数，只有当列表物体被销毁的时候才会）
        }

        // 点击时播放的动画，没有就返回null
        protected override MilInstantAnimator ConfigClickAnimation()
            => null;

        public override void UpdateAppearance()
        {
            // 更新外观
            var data = (ListItemDataSample)Binding; // 取得绑定的数据
            Content.text = data.Title;
        }

        public override void AdjustAppearance(float pos)
        {
            // 微调外观，pos是在列表位置的百分比
            // 如果需要做一些特殊的效果可以用这个，否则就可以留空
            
            // 例如这里会让列表的文本位置呈现一个特殊的排布，并会随列表滚动而变化
            var p = ContentRect.anchoredPosition;
            p.x = 45f + Mathf.Sin(pos * Mathf.PI) * 200f;
            ContentRect.anchoredPosition = p;
        }
    }
}
