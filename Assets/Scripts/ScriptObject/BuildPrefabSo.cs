using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CircleOfLife.ScriptObject
{
    [CreateAssetMenu(fileName = "BuildPrefabSo", menuName = "RuiRuiSo/BuildPrefabSo")]
    public class BuildPrefabSo:AllSettingSo<BuildStat,BuildSoData>
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Init2()
        {
            Init("ScriptsObject/BuildPrefabSo");

        }

        public static BuildSoData GetBuildData(BuildStat buildStat)
        {
            if (allSettings.TryGetValue(buildStat, out var result)) return result;
            else
            {
                Debug.Log($"BuildPrefabSo 中的 {typeof(BuildStat)}.{buildStat} 没有对应设置");
                return default(BuildSoData);

            }
        }
    }
    [Serializable]
    public class BuildSoData
    {
        public GameObject Prefab;
        public Sprite Icon;
        public Vector2 BuildSize;
        public int Cost;
        public bool WhetherRotate;
        public string Name;
        [Multiline]
        public string Description;

    }
}
