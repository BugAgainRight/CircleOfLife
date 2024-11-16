using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife.Atlas
{
    public class AtlasManager : MonoBehaviour
    {
        public static List<AtlasData> Data { get; private set; } = new();

        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            Data = Resources.Load<AtlasSO>("Atlas/AtlasData").Data;
        }
    }
}
