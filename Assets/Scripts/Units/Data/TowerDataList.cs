using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife.Units
{
    [CreateAssetMenu(fileName = "TowerData", menuName = "ScriptableObject/装置数据")]
    public class TowerDataList : ScriptableObject
    {
        [Header("Tower数据")]
        public List<TowerData> TowerDatas = new List<TowerData>();
    }
}
