using Milutools.Milutools.General;
using Milutools.Recycle;
using RuiRuiTool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CircleOfLife.ScriptObject
{
    public class AllSettingSo<T1,T2> : ScriptableObject where T1 : Enum
    {
        
        public static AllSettingSo<T1, T2> Instance;

        [HideInInspector]
        public List<TwoValue<T1, T2>> AllBuildSettings;

        protected static Dictionary<T1, T2> allSettings=new();


        public static T2 GetSetting(T1 type)
        {
            if(allSettings.TryGetValue(type,out var result))return result;
            else
            {
                Debug.Log($"AllPrefabsSetting 中的 {typeof(T1)}.{type} 没有对应设置");
                return default(T2);
                
            }
        }

        protected static void Init(string path)
        {
            Instance = Resources.Load<AllSettingSo<T1, T2>>(path);
            foreach (var a in Instance.AllBuildSettings)
            {
                allSettings.Add(a.value1, a.value2);
                if (a.value2 is GameObject gameObject)
                {
                    RecyclePool.EnsurePrefabRegistered(a.value1, gameObject, 20);
                }
            }
        }

    }


}
