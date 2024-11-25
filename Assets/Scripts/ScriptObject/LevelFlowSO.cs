using System.Collections.Generic;
using CircleOfLife.Level;
using UnityEngine;

namespace CircleOfLife.ScriptObject
{
    [CreateAssetMenu]
    public class LevelFlowSO : ScriptableObject
    {
        public TextAsset OpeningPlot;
        public List<LevelScriptableObject> Levels = new();
    }

    public class LevelFlowManager : MonoBehaviour
    {
        public static LevelFlowSO Flow;
        public static List<LevelScriptableObject> Levels;
        
        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            Flow = Resources.Load<LevelFlowSO>("LevelFlow");
            Levels = Flow.Levels;
        }
    }
}
