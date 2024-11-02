using RuiRuiTool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    [CreateAssetMenu(fileName = "NewTestSo", menuName = "RuiRuiSo/TestSo_Chen")]
    public class TestSo_Chen : ScriptableObject
    {
        [HideInInspector]
        public List<TwoValue<PlayerSkillType, GameObject>> allPlayerSkill = new();
        [HideInInspector]
        public List<TwoValue<EnemySkillType, GameObject>> allEnemySkill = new();
        [HideInInspector]
        public List<TwoValue<BuildSkillType, GameObject>> allBuildSkill = new();
      
       

    }

}
