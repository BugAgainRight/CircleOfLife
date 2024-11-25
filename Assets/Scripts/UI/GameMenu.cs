using Milutools.Milutools.UI;
using System;
using System.Linq;
using CircleOfLife.Atlas;
using CircleOfLife.Configuration;
using CircleOfLife.General;
using Milease.Utils;
using Milutools.Audio;
using Milutools.SceneRouter;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace CircleOfLife
{
    public class GameMenu : ManagedUI<GameMenu, bool>
    {
        public Slider BGMVolume, SndVolume;
        public GameObject RetryBtn, TitleScreenBtn;
        public bool VillageMode = false;
        
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

                SceneRouter.GoTo(SceneIdentifier.TitleScreen);
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

        public override void AboutToOpen(bool parameter)
        {
            Time.timeScale = 0f;
            VillageMode = parameter;
            RetryBtn.SetActive(!parameter);
            TitleScreenBtn.SetActive(parameter);
        }
    }
}
