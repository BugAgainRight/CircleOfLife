using System.Collections;
using System.Collections.Generic;
using Milutools.Audio;
using UnityEngine;

namespace CircleOfLife.Audio
{
    [CreateAssetMenu]
    public class SoundEffectsSO : AudioResources<SoundEffectsSO.Clips>
    {
        public enum Clips
        {
            Spawn, Shoot1, Shoot2, Shoot3, Blood, Thunder, Hit1, Hit2, Hit3, SkillBurst, SkillReady, Coin
        }
    }
}
