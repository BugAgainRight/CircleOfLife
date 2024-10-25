using System;
using Milutools.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace CircleOfLife.Sample
{
    /// <summary>
    /// 音频管理器 使用示例
    /// </summary>
    public class AudioManagerSample : MonoBehaviour
    {
        public Slider VolumeSlider;

        private void Awake()
        {
            // 读取音量
            VolumeSlider.value = AudioManager.GetVolume(AudioPlayerType.BGMPlayer);
        }

        // 更新音量
        public void ApplyVolume()
        {
            AudioManager.SetVolume(AudioPlayerType.BGMPlayer, VolumeSlider.value);
        }
        
        // 更换 BGM
        public void ChangeBGM1()
        {
            AudioManager.SetBGM(AudioResourcesSample.ID.BGM1);
        }
        
        // 更换 BGM
        public void ChangeBGM2()
        {
            AudioManager.SetBGM(AudioResourcesSample.ID.BGM2);
        }

        // 播放音效
        public void PlaySnd()
        {
            AudioManager.PlaySnd(AudioResourcesSample.ID.Click);
        }
    }
}
