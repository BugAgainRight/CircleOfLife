using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CircleOfLife.ScriptObject
{
    [CreateAssetMenu(fileName = "BuildSkillPrefabSo", menuName = "RuiRuiSo/BuildSkillPrefabSo")]
    public class BuildSkillPrefabSo:AllSettingSo<BuildSkillType,GameObject>
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Init2()
        {
            Init("ScriptsObject/BuildSkillPrefabSo");
        }
    }
}
