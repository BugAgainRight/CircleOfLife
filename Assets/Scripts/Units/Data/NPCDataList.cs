using System.Collections;
using System.Collections.Generic;
using CircleOfLife.Units;
using UnityEngine;

namespace CircleOfLife.Units
{
    [CreateAssetMenu(fileName = "NPCData", menuName = "ScriptableObject/NPC数据")]
    public class NPCDataList : ScriptableObject
    {
        [Header("NPC数据")]
        public List<NPCData> NPCDatas = new List<NPCData>();
    }
}
