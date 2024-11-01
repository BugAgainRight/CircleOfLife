using Milease.Utils;
using Milutools.Recycle;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Milease.Enums;

namespace CircleOfLife
{
    [Serializable]
    public class FlyWordStyle 
    {
        public int FontSize=3;
        public Color FontColor;
    }
   
    public class FlyWord : MonoBehaviour
    {

        [SerializeField]
        private TextMeshPro contentText;
        /// <summary>
        /// 飘字初始化
        /// </summary>
        /// <param name="content">飘字内容</param>
        /// <param name="style">飘字样式</param>

        public void Init(string content, FlyWordStyle style)
        {
            contentText.text = content;
            contentText.fontSize=style.FontSize;
            contentText.color = style.FontColor;
            GetComponent<MeshRenderer>().sortingOrder = 100;

            //动画放置位置

            transform.Milease(UMN.LScale, Vector3.one * 1.5f, Vector3.one, 0.5f, 0f, EaseFunction.Circ, EaseType.Out)
                .Then(
                    transform.Milease(UMN.LScale, Vector3.one, Vector3.one * 0.8f, 0.5f, 0f, EaseFunction.Circ, EaseType.Out),
                    contentText.Milease(UMN.Color, style.FontColor, style.FontColor.Clear(), 0.5f)
                )
                .PlayImmediately(() => RecyclePool.ReturnToPool(gameObject));

        }
        
    }
}
