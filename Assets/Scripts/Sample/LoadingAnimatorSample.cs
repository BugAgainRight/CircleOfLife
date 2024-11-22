using System;
using Milease.Core;
using Milease.Utils;
using Milutools.Audio;
using Milutools.SceneRouter;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace CircleOfLife.Sample
{
    /// <summary>
    /// 自定义场景加载动画 示例
    /// （仿原神）
    /// </summary>
    public class LoadingAnimatorSample : LoadingAnimator
    {
        public TMP_Text TipTitle, Tip;
        public SkeletonGraphic SkeletonGraphic;
        public Image Background;
        
        public override void AboutToLoad()
        {
            var tips = Resources.Load<TextAsset>("Tips").text.Split('\n');
            Tip.text = tips[Random.Range(0, tips.Length)];
            // 准备加载时，这里需要用动画机先把画面铺满
            Background.Milease(UMN.Color, Color.black.Clear(), Color.black, 0.5f)
                .Then(
                    TipTitle.Milease(UMN.Color, Color.white.Clear(), Color.white, 0.25f),
                    Tip.Milease(UMN.Color, Color.white.Clear(), Color.white, 0.25f, 0.25f),
                    Tip.Milease(nameof(Tip.characterSpacing), 5f, 0f, 0.25f, 0.25f)
                )
                .Then(
                    SkeletonGraphic.Milease(UMN.Color, Color.white.Clear(), Color.white, 0.25f)
                )
                .UsingResetMode(RuntimeAnimationPart.AnimationResetMode.ResetToInitialState)
                .PlayImmediately(ReadyToLoad); // 这里指定动画的回调函数，播放结束后 调用 ReadyToLoad 告诉场景路由器可以开始加载
        }

        public override void OnLoaded()
        {
            // 场景加载完成时，这里需要用动画机把动画淡出
            SkeletonGraphic.Milease(UMN.Color, Color.white, Color.white.Clear(), 0.25f, 2f)
                .Then(
                    TipTitle.Milease(UMN.Color, Color.white, Color.white.Clear(), 0.25f),
                    Tip.Milease(UMN.Color, Color.white, Color.white.Clear(), 0.25f)
                )
                .Then(
                    Background.Milease(UMN.Color, Color.black, Color.black.Clear(), 0.5f)
                ).PlayImmediately(FinishLoading); // 这里指定动画的回调函数，播放结束后 调用 FinishLoading 告诉场景路由器整个换场景的过程已完成
        }

        private void Update()
        {

        }
    }
}
