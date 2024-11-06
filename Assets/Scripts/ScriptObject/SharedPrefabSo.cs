using CircleOfLife.ScriptObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    [CreateAssetMenu(fileName = "SharedPrefabSo", menuName = "RuiRuiSo/SharedPrefabSo")]
    public class SharedPrefabSo : AllSettingSo<SharedPrefab, GameObject>
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Init2()
        {
            Init("ScriptsObject/SharedPrefabSo");
        }
    }
}
