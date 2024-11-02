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



        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                SkillManagement.GetSkill(PlayerSkillType.test1)(new SkillContext()
                {
                    TriggerPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition),
                    TargetPos = new Vector2(5, 0),
                    Direction = Vector2.right,
                    BodyPrefab = testBulletPrefab,
                    MoveSpeed = 10
                });
            }
        }



    }
}
