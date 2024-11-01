using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class RuiRuiTest : MonoBehaviour
    {
        public GameObject testBulletPrefab;
        private void Start()
        {
            SkillManagement.GetSkill(PlayerSkillType.test1)(new SkillContext() 
            { TriggerPos = new Vector2(-5, 0), Direction = Vector2.right, Prefab = testBulletPrefab });


        }



    }
}
