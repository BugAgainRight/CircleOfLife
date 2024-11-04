using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CircleOfLife.ScriptObject
{
    [CreateAssetMenu(fileName = "EnemySkillPrefabSo", menuName = "RuiRuiSo/EnemySkillPrefabSo")]
    internal class EnemySkillPrefabSo:AllSettingSo<EnemySkillType,GameObject>
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Init2()
        {
            Init("ScriptsObject/EnemySkillPrefabSo");
        }
    }
}
