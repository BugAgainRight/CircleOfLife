using CircleOfLife.Battle;
using CircleOfLife.Buff;
using CircleOfLife.ScriptObject;
using RuiRuiTool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    [Serializable]
    public class testsdadsa   
    {
        public int asdas;
        public float sdas;
    
    }

    public class RuiRuiTest : MonoBehaviour,IBattleEntity
    {
        public testsdadsa aaa;
        public List<BuildSoData> aaas;
        public List<TwoValue<int, float>> bbb;
        public List<TwoValue<int, BuildSoData>> aaa2;
        public List<BuildSoData> testData = new();
        private bool selectCurve = false;
        public GameObject testBulletPrefab_Curve;
        public GameObject testBulletPrefab_2;

        public TestSo_Chen TestSo_Chen;

        public List<TwoValue<int, float>> testasdsad = new();
        private void Start()
        {
            TestSo_Chen.GetSkillPrefab(PlayerSkillType.Whack);
        }

        public BattleStats.Stats Attribute;
        public BattleStats Stats { get; set; }
        [SerializeField]
        private FactionType factionType;
        public FactionType FactionType => factionType;

        private void Awake()
        {
            Stats = Attribute.Build(gameObject, (aaa)=>{ });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                SkillManagement.GetSkill(PlayerSkillType.Slash)(new SkillContext()
                {
                    TriggerPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition),
                    TargetPos = new Vector2(5, 0),
                    Direction = Vector2.right,
                    MoveSpeed = 10,
                    AttackerData = Stats,
                    PhysicsLayer = 1 << 0,

                }) ;
                //if (selectCurve)
                //{
                //    SkillManagement.GetSkill(PlayerSkillType.Whack)(new SkillContext()
                //    {
                //        TriggerPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition),
                //        TargetPos = new Vector2(5, 0),
                //        Direction = Vector2.right,
                //        MoveSpeed = 10,
                //        AttackerData = Stats
                        

                //    });
                //}
                //else
                //{
                //    SkillManagement.GetSkill(EnemySkillType.test1)(new SkillContext()
                //    {
                //        TriggerPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition),
                //        TargetPos = new Vector2(5, 0),
                //        Direction = Vector2.right,
                //        MoveSpeed = 10,
                //        AttackerData = Stats
                //    });
                //}
            }
        }

        public void SelectCurve(bool select)
        {
            selectCurve = select;
        }

    }
}
