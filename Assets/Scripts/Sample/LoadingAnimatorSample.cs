using System;
using Milease.Core;
using Milease.Utils;
using Milutools.Audio;
using Milutools.SceneRouter;
using UnityEngine;
using UnityEngine.UI;

namespace CircleOfLife.Sample
{
    /// <summary>
    /// 自定义场景加载动画 示例
    /// （仿原神）
    /// </summary>
    public class LoadingAnimatorSample : LoadingAnimator
    {
        public Image Background, BackBar, FillBar;
        public RectTransform FullRect, CurrentRect;
        
        public override void AboutToLoad()
        {
            // 准备加载时，这里需要用动画机先把画面铺满
            AudioManager.PlaySnd(AudioResourcesSample.ID.GenshinLoading);
            CurrentRect.sizeDelta = Vector2.zero;
            Background.Milease(UMN.Color, Color.white.Clear(), Color.white, 0.5f)
                .Then(
                    BackBar.Milease(UMN.Color, Color.white.Clear(), Color.white, 0.25f)
                )
                .UsingResetMode(RuntimeAnimationPart.AnimationResetMode.ResetToInitialState)
                .PlayImmediately(ReadyToLoad); // 这里指定动画的回调函数，播放结束后 调用 ReadyToLoad 告诉场景路由器可以开始加载
        }

        public override void OnLoaded()
        {
            // 场景加载完成时，这里需要用动画机把动画淡出
            BackBar.Milease(UMN.Color, Color.white, Color.white.Clear(), 0.25f)
                .While(
                    FillBar.Milease(UMN.Color, Color.white, Color.white.Clear(), 0.25f)
                ).Delayed(1f)
                .Then(
                    Background.Milease(UMN.Color, Color.white, Color.white.Clear(), 0.5f)
                ).PlayImmediately(FinishLoading); // 这里指定动画的回调函数，播放结束后 调用 FinishLoading 告诉场景路由器整个换场景的过程已完成
        }

        private void Update()
        {
            // 此处可以用 Progress 参数更新进度
            CurrentRect.sizeDelta = new Vector2(FullRect.sizeDelta.x * Progress, FullRect.sizeDelta.y);
        }
    }
}
