using System;
using System.Collections.Generic;
using Milutools.Audio;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CircleOfLife.Audio
{
    public class SndPlayer : MonoBehaviour
    {
        public List<SoundEffectsSO.Clips> Clips;

        private void OnEnable()
        {
            AudioManager.PlaySnd(Clips[Random.Range(0, Clips.Count)]);
        }
    }
}
