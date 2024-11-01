using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CircleOfLife
{
    public enum BulletTriggerType
    {
        Normal, Boom
    }
    public static class BulletManagement
    {
        private static Dictionary<BulletTriggerType, Action<BattleContext>> allBulletTriggers = new();

        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {


            ///注册所有子弹触发方法
            foreach (var a in RuiRuiTool.AttributeMethodUtility.GetAllActionDatas<BulletTriggerAttribute>())
            {
                BulletTriggerAttribute b = (BulletTriggerAttribute)a.attribute;
                allBulletTriggers.Add((b.BuffType), (Action<BattleContext>)a.method.CreateDelegate(typeof(Action<BattleContext>)));
            }

        }

        public static Action<BattleContext> GetBulletTrigger(BulletTriggerType type)
        {
            if(allBulletTriggers.ContainsKey(type))return allBulletTriggers[type];
            else
            {
                Debug.LogError($"{type} 对应的触发函数未实现！");
                return null;
            }
        }




        [BulletTrigger(BulletTriggerType.Normal)]
        public static void NormalTrigger(BattleContext battleContext)
        {
            DamageManagement.Instance.Damage(battleContext);
        }



        [BulletTrigger(BulletTriggerType.Boom)]
        public static void BoomTrigger(BattleContext battleContext)
        {
            var colls = Physics2D.OverlapCircleAll(battleContext.AttackerTran.position, battleContext.BoomRadius, battleContext.DamageableLayer);
            var idamages = colls.ToList().Where(x => x.GetComponent<IDamageable_>() != null);
            foreach (var idamage in idamages)
            {
                BattleContext midContext=battleContext;
                midContext.HitTran = idamage.transform;
                midContext.HitCollider = idamage;
                midContext.HitData = idamage.GetComponent<IDamageable_>().GetData();

                DamageManagement.Instance.Damage(midContext);
            }

        }




    }


    [AttributeUsage(AttributeTargets.Method)]
    public class BulletTriggerAttribute: Attribute
    {
        public BulletTriggerType BuffType;
        public BulletTriggerAttribute(BulletTriggerType type)
        {
            BuffType = type;
        }

    }


    public static class RuiRuiTool
    {
        public struct ActionAttributeData
        {
            public Attribute attribute;
            public MethodInfo method;
        }
        public static class AttributeMethodUtility
        {
            public static List<ActionAttributeData> GetAllActionDatas<T>() where T : Attribute
            {

                var asm = Assembly.GetAssembly(typeof(T));
                var lists = asm.GetExportedTypes()
                               .Select(cls =>
                                    cls.GetMethods(BindingFlags.Default | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod).Where(method => method.GetCustomAttribute(typeof(T)) != null)
                                                   .Select(method => new ActionAttributeData()
                                                   {
                                                       attribute = method.GetCustomAttribute(typeof(T)),
                                                       method = method
                                                   })
                                       );
                List<ActionAttributeData> mid = new();
                foreach (var list in lists)
                {
                    mid.AddRange(list);
                }

                return mid;
            }

            public static List<ActionAttributeData> GetAllActionDatas<T>(BindingFlags bindingAttr) where T : Attribute
            {

                var asm = Assembly.GetAssembly(typeof(T));
                var lists = asm.GetExportedTypes()
                               .Select(cls =>
                                    cls.GetMethods(bindingAttr).Where(method => method.GetCustomAttribute(typeof(T)) != null)
                                                   .Select(method => new ActionAttributeData()
                                                   {
                                                       attribute = method.GetCustomAttribute(typeof(T)),
                                                       method = method
                                                   })
                                       );
                List<ActionAttributeData> mid = new();
                foreach (var list in lists)
                {
                    mid.AddRange(list);
                }

                return mid;
            }

        }


    }




}
