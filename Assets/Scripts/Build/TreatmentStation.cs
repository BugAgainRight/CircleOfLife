using System;
using CircleOfLife.Battle;
using CircleOfLife.Buff;
using Milutools.Recycle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class TreatmentStation : BuildBase
    {
        public override List<LevelUpDirection> LevelUpDirections => new()
        {
            new LevelUpDirection(){NeedLevel=2,Title="兽医站",Value=BuildSkillType.TreatmentStation1},
            new LevelUpDirection(){NeedLevel=2,Title="战地医院",Value=BuildSkillType.TreatmentStation2},
            
        };
        public override void HurtAction(BattleContext context)
        {
            if (context.HitData.Current.Hp <= 0)
            {
                //更新寻路场

                RecyclePool.ReturnToPool(gameObject);
            }
        }

        private void Awake()
        {
            Level = 1;
            NowType = BuildSkillType.TreatmentStationNormal;
            Stats = Attribute[0].Build(gameObject, HurtAction);
        }
        private void OnEnable()
        {
            Level = 1;
            NowType = BuildSkillType.TreatmentStationNormal;
            ReplaceStats(Attribute[0], true);
            UpdateRange();
            
        }


        private void FixedUpdate()
        {
            RecoveryHP();
            if (TimerFinish)
            {
                SkillContext skillContext = new(PhysicsLayer, Stats)
                {
                    EffectCount = effectCount,
                    SpecialValues = new() { RecvoeryValueAll, RecvoeryValueEnemy }
                };

                SkillManagement.GetSkill(BuildSkillType.TreatmentStationNormal)(skillContext);

            }
        }
        private int effectCount = 1;
        private float RecvoeryValueAll;
        private float RecvoeryValueEnemy;

        public int HospitalEffectCount;
        public List<TreatmentStationUse> AllLevelValues=new List<TreatmentStationUse>();
        protected override void LevelUpFunc()
        {
            
            effectCount = AllLevelValues[Level - 1].EffectCount;
            RecvoeryValueAll = AllLevelValues[Level - 1].RecvoeryValueAll;
            RecvoeryValueEnemy = AllLevelValues[Level - 1].RecvoeryValueAnimal;
            if (((BuildSkillType)NowType).Equals(BuildSkillType.TreatmentStation2)) effectCount += HospitalEffectCount;
            UpdateRange();


        }

        [Serializable]
        public class TreatmentStationUse
        {
            public int EffectCount;
            public float RecvoeryValueAll;
            public float RecvoeryValueAnimal;
           
        }
    }
}
