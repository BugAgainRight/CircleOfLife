using System.Collections.Generic;
using CircleOfLife.Level;
using UnityEngine;

namespace CircleOfLife.ScriptObject
{
    [CreateAssetMenu]
    public class LevelFlowSO : ScriptableObject
    {
        public List<LevelScriptableObject> Levels = new();
    }

    public class LevelFlowManager : MonoBehaviour
    {
        public static List<LevelScriptableObject> Levels;
        
        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            Levels = Resources.Load<LevelFlowSO>("LevelFlow").Levels;
        }
    }
}
