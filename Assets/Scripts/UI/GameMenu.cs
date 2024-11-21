using Milutools.Milutools.UI;
using System;
using System.Linq;
using CircleOfLife.Atlas;
using CircleOfLife.General;
using Milease.Utils;
using Milutools.Audio;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace CircleOfLife
{
    public class GameMenu : ManagedUI<GameMenu, object>
    {
        public Slider BGMVolume, SndVolume;
        
        protected override void Begin()
        {
            BGMVolume.value = AudioManager.GetVolume(AudioPlayerType.BGMPlayer);
            SndVolume.value = AudioManager.GetVolume(AudioPlayerType.SndPlayer);
        }

        public void Continue()
        {
            Close();
        }

        public void ViewAtlas()
        {
            AtlasUI.Open();
        }
        
        public void ApplyBGMVolume()
        {
            AudioManager.SetVolume(AudioPlayerType.BGMPlayer, BGMVolume.value);
        }

        public void ApplySndVolume()
        {
            AudioManager.SetVolume(AudioPlayerType.SndPlayer, SndVolume.value);
        }

        public void GoToTitleScreen()
        {
            MessageBox.Open(("返回标题画面", "确定要返回标题画面吗？\n当前未保存的所有游戏进度都将会丢失！"), (o) =>
            {
                if (o == MessageBox.Operation.Deny)
                {
                    return;
                }
            });
        }
        
        public void Retry()
        {
            MessageBox.Open(("重试游戏", "确定要重试吗？"), (o) =>
            {
                if (o == MessageBox.Operation.Deny)
                {
                    return;
                }
            });
        }

        protected override void AboutToClose()
        {
            Time.timeScale = 1f;
        }

        public override void AboutToOpen(object parameter)
        {
            Time.timeScale = 0f;
        }
    }
}
