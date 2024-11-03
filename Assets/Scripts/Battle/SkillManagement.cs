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

        [Skill(EnemySkillType.test1)]
        private static void EnemySkill_1(SkillContext skillContext)
        {

            RecyclePool.EnsurePrefabRegistered(EnemySkillType.test1, skillContext.BodyPrefab, 10);
            var collection = RecyclePool.RequestWithCollection(EnemySkillType.test1);
            collection.GameObject.SetActive(true);
            collection.GameObject.transform.position = skillContext.TriggerPos;

            collection.GetComponent<BulletTrigger>().PassData(new BattleContext()
            {
                AttackerTran = collection.GameObject.transform,
                AttackerData = skillContext.AttackerData,
                BoomRadius = 5,
                DamageableLayer = 1 << 0,

            });

            collection.GetComponent<BulletMove>().PassData(new BulletMoveContext()
            {
                LaunchTime = Time.time,
                Direction = skillContext.Direction,
                StartPos = collection.GameObject.transform.position,
                TargetPos = skillContext.TargetPos,
                Transform = collection.GameObject.transform,
                Speed = skillContext.MoveSpeed,
                MaxHeight = UnityEngine.Random.Range(4, 6f)
            });
        }

        [Skill(PlayerSkillType.test1)]
        private static void PlayerSkill_1(SkillContext context)
        {
           
            RecyclePool.EnsurePrefabRegistered(PlayerSkillType.test1, context.BodyPrefab, 10);
            var collection = RecyclePool.RequestWithCollection(PlayerSkillType.test1);
            collection.GameObject.SetActive(true);
            collection.GameObject.transform.position = context.TriggerPos;

            collection.GetComponent<BulletTrigger>().PassData(new BattleContext()
            {
                AttackerTran = collection.GameObject.transform,
                AttackerData = context.AttackerData,
                BoomRadius = 5,
                DamageableLayer = 1 << 0,

            }) ;

            collection.GetComponent<BulletMove>().PassData(new BulletMoveContext()
            {
                LaunchTime = Time.time,
                Direction=context.Direction,
                StartPos = collection.GameObject.transform.position,
                TargetPos = context.TargetPos,
                Transform= collection.GameObject.transform,
                Speed = context.MoveSpeed,
                MaxHeight = UnityEngine.Random.Range(4, 6f)
            });
        }

        [Skill(BuildSkillType.TreatmentStation)]
        private static void BuildSkill_0(SkillContext context)
        {
            foreach (var coll in Physics2D.OverlapCircleAll(
                context.RangeSetting.gizmosCenter, context.RangeSetting.radius, context.PhysicsLayer))
            {
                if (coll.gameObject.Equals(context.TriggerObj)) continue;
                if (coll.TryGetComponent(out IBattleEntity battleEntity))
                {
                    if (battleEntity.FactionType.Equals(context.FactionType))
                    {
                        DamageManagement.Instance.Damage(new BattleContext()
                        {
                            AttackerTran = context.TriggerObj.transform,
                            HitTran = coll.transform,
                            AttackerData = context.AttackerData,
                            HitData = battleEntity.Stats,
                        });

                    }
                }

            }


        }

        [Skill(BuildSkillType.SignalTransmitter1)]
        private static void BuildSkill_1(SkillContext context)
        {
            var collection = RecyclePool.RequestWithCollection(BuildSkillType.SignalTransmitter1,context.TriggerObj.transform);
            collection.GameObject.SetActive(true);
            var passData = context.AttackerData.Max;
            passData.AttackInterval = 1;
            collection.GetComponent<TestBuildFriend_Chen>().PassData(passData);
            var buildContext = collection.GetMainComponent<BuildFriendContext>();
            buildContext.StandPos = context.TriggerPos + new Vector2(UnityEngine.Random.Range(-3, 3f), UnityEngine.Random.Range(-3, 3f));
            collection.GameObject.transform.position=buildContext.StandPos;
        }




        [Skill(BuildSkillType.TestBuildFriendFire)]
        private static void BuildSkill_2(SkillContext context)
        {
            BulletManagement.GetBulletTrigger(BulletTriggerType.Normal)(new BattleContext()
            {
                AttackerData = context.AttackerData,
                HitTran = context.TargetObj.transform,
                HitData=context.HitData

            });


        }

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
