using Milutools.Audio;
using UnityEngine;

namespace CircleOfLife.Audio
{
    [CreateAssetMenu]
    public class BGMSO : AudioResources<BGMSO.Clips>
    {
        public enum Clips
        {
            Battle, TitleScreen, Map, Credits, Village
        }
    }
}
