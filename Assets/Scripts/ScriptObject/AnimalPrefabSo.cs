using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CircleOfLife.ScriptObject
{
    [CreateAssetMenu(fileName = "AnimalPrefabSo", menuName = "RuiRuiSo/AnimalPrefabSo")]
    public class AnimalPrefabSo : AllSettingSo<AnimalStat, GameObject>
    {

        [RuntimeInitializeOnLoadMethod]
        public static void Init2()
        {
            Init("ScriptsObject/AnimalPrefabSo");
        }
    }
}
