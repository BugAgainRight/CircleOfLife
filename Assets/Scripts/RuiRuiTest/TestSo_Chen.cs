using Milutools.Milutools.General;
using RuiRuiTool;
using System;
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
      
        private Dictionary<EnumIdentifier, GameObject> allSkills = new();

        public GameObject GetSkillPrefab<T>(T type) where T : Enum
        {
            if (allSkills.TryGetValue(EnumIdentifier.Wrap(type), out var obj)) return obj;
            else
            {
                Debug.LogError($"{typeof(TestSo_Chen)} 中的 {typeof(T)} 枚举类型值{type} 没有对应的预制体！");
                return null;
               
            }
        }
        
        private void OnEnable()
        {
            allSkills = new();
            foreach (var a in allPlayerSkill)
            {
                allSkills.Add(EnumIdentifier.Wrap(a.value1), a.value2);
            }
            foreach (var a in allEnemySkill)
            {
                allSkills.Add(EnumIdentifier.Wrap(a.value1), a.value2);
            }
            foreach (var a in allBuildSkill)
            {
                allSkills.Add(EnumIdentifier.Wrap(a.value1), a.value2);
            }

        }


    }

}
