using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System;

namespace RuiRuiTool
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



    [System.Serializable]
    public class TwoValue<T1, T2>
    {
        public T1 value1;
        public T2 value2;
    }

    public static class RuiRuiExtend
    {
        public static int Length<T>(this T enum_) where T : Type
        {
            if (enum_.IsEnum)
                return System.Enum.GetValues(enum_).Length;
            Debug.LogError($"{enum_}不是枚举类型!");
            return 0;
        }
    }


}
