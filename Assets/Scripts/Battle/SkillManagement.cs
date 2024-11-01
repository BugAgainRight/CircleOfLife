using Milutools.Milutools.General;
using Milutools.Recycle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public enum EnemySkillType
    {
        test1
    }

    public enum PlayerSkillType
    {
        test1
    }

    public enum BuildSkillType
    {
        test1
    }

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
        public static void EnemySkill_1(SkillContext skillContext)
        {


        }

        [Skill(PlayerSkillType.test1)]
        public static void PlayerSkill_1(SkillContext skillContext)
        {
            RecyclePool.EnsurePrefabRegistered(PlayerSkillType.test1, skillContext.Prefab, 10);
            var collection = RecyclePool.RequestWithCollection(PlayerSkillType.test1);
            collection.GameObject.SetActive(true);

            collection.GetMainComponent<BulletTrigger>().PassData(new BattleContext()
            {
                AttackerTran = collection.GameObject.transform,
                AttackerData = new Units.NPCData() { Atk = 100 },
                BoomRadius = 5,
                DamageableLayer = 1 << 0,

            });
           
            collection.GetComponent<Rigidbody2D>().velocity = skillContext.Direction * 10;
        }

        [Skill(BuildSkillType.test1)]
        public static void BuildSkill_1(SkillContext skillContext)
        {
           


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
