using System;
using Milutools.Audio;
using UnityEngine;

namespace CircleOfLife.Audio
{
    public class SndVolumeApply : MonoBehaviour
    {
        private void OnEnable()
        {
            GetComponent<AudioSource>().volume = AudioManager.GetVolume(AudioPlayerType.SndPlayer);
        }
    }
}
