using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CircleOfLife.ScriptObject
{
    [CreateAssetMenu(fileName = "AnimationPrefabSo", menuName = "RuiRuiSo/AnimationPrefabSo")]
    public class AnimationPrefabSo:AllSettingSo<AnimatonPrefab,GameObject>
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Init2()
        {
            Init("ScriptsObject/AnimationPrefabSo");
        }
    }
}
