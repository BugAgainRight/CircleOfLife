using CircleOfLife.Battle;
using CircleOfLife.Buff;
using log4net.Util;
using Milutools.Milutools.General;
using Milutools.Recycle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public static class SkillManagement 
    {

        private static Dictionary<EnumIdentifier, Action<SkillContext>> allSkills = new();

        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            
            ///注册所有技能方法
            foreach (var a in RuiRuiTool.AttributeMethodUtility.GetAllActionDatas<SkillAttribute>())
            {
                SkillAttribute b = (SkillAttribute)a.attribute;
                allSkills.Add(b.Key, (Action<SkillContext>)a.method.CreateDelegate(typeof(Action<SkillContext>)));
            } 
           
        }

        public static Action<SkillContext> GetSkill<T>(T type)where T : Enum
        {

            if (allSkills.TryGetValue(EnumIdentifier.Wrap(type), out var action)) return action;
            else
            {
                Debug.LogError($"{typeof(T)}枚举类型 {type}没有实现技能函数！");
                return null;
            }


        }



        #region PlayerSkill
        [Skill(PlayerSkillType.Whack)]
        private static void PlayerSkill_1(SkillContext context)
        {
            float angle = Mathf.Atan2(context.Direction.y, context.Direction.x);
            var collection = RecyclePool.RequestWithCollection(PlayerSkillType.Whack);
            collection.GameObject.SetActive(true);
            collection.GameObject.transform.position = context.TriggerPos;
            collection.GameObject.transform.localEulerAngles = new Vector3(0, 0, angle);

            var list = collection.GetMainComponent<BattleRange>().GetAllEnemyInRange(
                context.PhysicsLayer, context.AttackerData.BattleEntity.FactionType);

            foreach (var item in list)
            {
                BattleStats mid = item.GetBattleStats();
                BulletManagement.GetBulletTrigger(BulletTriggerType.Normal)(
                    new BattleContext(context.PhysicsLayer, context.AttackerData, mid.BattleEntity.Stats) 
                    { SkillRate=3});
                
                mid.BattleEntity.Stats.ApplyBuff(BuffUtils.ToBuff(UniversalBuff.Dizzy, 3f));
                mid.BattleEntity.Stats.ApplyBuff(BuffUtils.ToBuff(UniversalBuff.Blood, 3f));

            }
        }

        [Skill(PlayerSkillType.Slash)]
        private static void PlayerSkill_2(SkillContext context)
        {
            float angle = Mathf.Atan2(context.Direction.y, context.Direction.x);
            var collection = RecyclePool.RequestWithCollection(PlayerSkillType.Slash);
            collection.GameObject.SetActive(true);
            collection.GameObject.transform.position = context.TriggerPos;
            collection.GameObject.transform.localEulerAngles = new Vector3(0, 0, angle);

            var list = collection.GetMainComponent<BattleRange>().GetAllEnemyInRange(
                context.PhysicsLayer, context.AttackerData.BattleEntity.FactionType);

            foreach (var item in list)
            {
                BattleStats mid = item.GetBattleStats();
                BulletManagement.GetBulletTrigger(BulletTriggerType.Normal)(
                    new BattleContext(context.PhysicsLayer, context.AttackerData, mid.BattleEntity.Stats));

                mid.BattleEntity.Stats.ApplyBuff(BuffUtils.ToBuff(UniversalBuff.SlowDown, 3f));
                

            }
        }

        /// <summary>
        /// 愈战愈勇
        /// </summary>
        /// <param name="context"></param>
        [Skill(PlayerSkillType.Skill3)]
        private static void PlayerSkill_3(SkillContext context)
        {
            //context.AttackerData.ApplyBuff(BuffUtils.ToBuff(UniversalBuff.))
        }


        #endregion




        #region EnemySkill

        [Skill(EnemySkillType.test1)]
        private static void EnemySkill_1(SkillContext context)
        {

      
            var collection = RecyclePool.RequestWithCollection(EnemySkillType.test1);
            collection.GameObject.SetActive(true);
            collection.GameObject.transform.position = context.TriggerPos;

            collection.GetComponent<BulletTrigger>().PassData(
                new BattleContext(context.PhysicsLayer, context.AttackerData, context.HitData)
                {
                    BoomRadius = 5,

                });

            collection.GetComponent<BulletMove>().PassData(new BulletMoveContext()
            {
                LaunchTime = Time.time,
                Direction = context.Direction,
                StartPos = collection.GameObject.transform.position,
                TargetPos = context.TargetPos,
                Transform = collection.GameObject.transform,
                Speed = context.MoveSpeed,
                MaxHeight = UnityEngine.Random.Range(4, 6f)
            });
        }


        #endregion



        #region AnimalSkill


        #endregion



        #region BuildSkill

        [Skill(BuildSkillType.TreatmentStation)]
        private static void BuildSkill_0(SkillContext context)
        {
            var list = context.AttackerData.Transform.GetComponent<BuildBase>().BattleRange.GetAllFriendInRange(
                context.PhysicsLayer, context.AttackerData.BattleEntity.FactionType);
            foreach (var coll in list)
            {
                DamageManagement.Damage(
                    new BattleContext(context.PhysicsLayer, context.AttackerData, coll.GetBattleStats()));
            }

        }

        [Skill(BuildSkillType.SignalTransmitter1)]
        private static void BuildSkill_1(SkillContext context)
        {
            var collection = RecyclePool.RequestWithCollection(BuildSkillType.SignalTransmitter1, context.AttackerData.Transform);
            collection.GameObject.SetActive(true);
            var passData = context.AttackerData.Max;
            passData.AttackInterval = 1;
            collection.GetComponent<TestBuildFriend_Chen>().PassData(passData);
            var buildContext = collection.GetMainComponent<BuildFriendContext>();
            buildContext.StandPos = context.TriggerPos + new Vector2(UnityEngine.Random.Range(-3, 3f), UnityEngine.Random.Range(-3, 3f));
            collection.GameObject.transform.position = buildContext.StandPos;
        }




        [Skill(BuildSkillType.TestBuildFriendFire)]
        private static void BuildSkill_2(SkillContext context)
        {
            BulletManagement.GetBulletTrigger(BulletTriggerType.Normal)(
                new BattleContext(context.PhysicsLayer, context.AttackerData, context.HitData));


        }

        #endregion



    }

    [AttributeUsage(AttributeTargets.Method)]
    public class SkillAttribute : Attribute
    { 
        public Enum BuffType;
        public EnumIdentifier Key;
        public SkillAttribute(object type)
        {
            BuffType = (Enum)type;
            Key = EnumIdentifier.WrapReflection(BuffType);
        }

    }
}
