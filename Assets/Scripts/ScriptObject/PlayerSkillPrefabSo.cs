using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CircleOfLife.ScriptObject
{
    [CreateAssetMenu(fileName = "PlayerSkillPrefabSo", menuName = "RuiRuiSo/PlayerSkillPrefabSo")]
    internal class PlayerSkillPrefabSo:AllSettingSo<PlayerSkillType,GameObject>
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Init2()
        {
            Init("ScriptsObject/PlayerSkillPrefabSo");
        }
    }
}
