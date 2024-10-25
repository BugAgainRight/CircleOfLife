using System;
using UnityEngine;

namespace CircleOfLife.Sample
{
    // 列表项的数据模型，如果是简单的数据，可以直接用 string、int 等
    // 如果数据比较复杂则需要创建一个
    [Serializable]
    public class ListItemDataSample
    {
        public string Title;
        public GameObject LinkController;
    }
}
