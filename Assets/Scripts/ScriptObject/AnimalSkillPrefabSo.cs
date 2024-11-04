using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CircleOfLife.ScriptObject
{
    [CreateAssetMenu(fileName = "AnimalSkillPrefabSo", menuName = "RuiRuiSo/AnimalSkillPrefabSo")]
    internal class AnimalSkillPrefabSo:AllSettingSo<AnimalSkillType,GameObject>
    {


        [RuntimeInitializeOnLoadMethod]
        public static void Init2()
        {
            Init("ScriptsObject/AnimalSkillPrefabSo");
        }
    }
}
