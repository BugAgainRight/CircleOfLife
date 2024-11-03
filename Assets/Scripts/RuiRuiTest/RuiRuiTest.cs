using CircleOfLife.Battle;
using CircleOfLife.Buff;
using RuiRuiTool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class RuiRuiTest : MonoBehaviour
    {
        private bool selectCurve=false;
        public GameObject testBulletPrefab_Curve;
        public GameObject testBulletPrefab_2;

        public TestSo_Chen TestSo_Chen;

        public List<TwoValue<int, float>> testasdsad = new();
        private void Start()
        {
            TestSo_Chen.GetSkillPrefab(PlayerSkillType.test1);
        }

        public BattleStats.Stats Attribute;
        public BattleStats Stats;

        private void Awake()
        {
            Stats = Attribute.Build(gameObject, (aaa)=>{ });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {

                if (selectCurve)
                {
                    SkillManagement.GetSkill(PlayerSkillType.test1)(new SkillContext()
                    {
                        TriggerPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition),
                        TargetPos = new Vector2(5, 0),
                        Direction = Vector2.right,
                        BodyPrefab = testBulletPrefab_Curve,
                        MoveSpeed = 10,
                        AttackerData = Stats
                        

                    });
                }
                else
                {
                    SkillManagement.GetSkill(EnemySkillType.test1)(new SkillContext()
                    {
                        TriggerPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition),
                        TargetPos = new Vector2(5, 0),
                        Direction = Vector2.right,
                        BodyPrefab = testBulletPrefab_2,
                        MoveSpeed = 10,
                        AttackerData = Stats
                    });
                }
            }
        }

        public void SelectCurve(bool select)
        {
            selectCurve = select;
        }

    }
}
