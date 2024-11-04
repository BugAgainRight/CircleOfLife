using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife.ScriptObject
{
    [CreateAssetMenu(fileName = "EnemyPrefabSo", menuName = "RuiRuiSo/EnemyPrefabSo")]
    public class EnemyPrefabSo:AllSettingSo<EnemyStat,GameObject>
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Init2()
        {
            Init("ScriptsObject/EnemyPrefabSo");
        }
    }
}
