using Milutools.Audio;
using UnityEngine;

namespace CircleOfLife.Sample
{
    // 音频资源清单示例
    // 【重要】随后创建的 ScriptableObject 要放在统一的位置，例如当前项目就放在：Resources/AudioResources
    // 音频文件位置没有要求
    [CreateAssetMenu] // 需要有这一行，才能在 Unity 中创建 ScriptableObject
    public class AudioResourcesSample : AudioResources<AudioResourcesSample.ID> // 继承 AudioResources<ID枚举>
    {
        // 列举音频列表，稍后在 ScriptableObject 中设置
        public enum ID
        {
            // 可以看情况分类，一种类别一个 AudioResources，这里是演示方便合在一起了
            // 目前不足的是，不能在中间插入或者删除值，会导致枚举值变动，音频关联顺序会乱（保证值不变的情况下改动是可以的）
            BGM1, BGM2, Click, GenshinLoading, Story
        }
    }
}
