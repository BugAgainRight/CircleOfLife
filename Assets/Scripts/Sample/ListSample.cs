using System;
using System.Collections;
using System.Collections.Generic;
using CircleOfLife.Sample;
using Milease.Core.UI;
using UnityEngine;

namespace CircleOfLife
{
    /// <summary>
    /// 列表功能示例代码
    /// </summary>
    public class ListSample : MonoBehaviour
    {
        public static ListItemDataSample CurrentSelected;
        
        // 具体的列表组件，需要在 Inspector 中设置排布方式，绑定列表项控制器，详情参考场景中的设置
        public MilListView ListView;
        public List<ListItemDataSample> Data;
        
        private void Awake()
        {
            CurrentSelected = null;
            // 可直接往列表加入数据，无需其他操作
            foreach (var data in Data)
            {
                ListView.Add(data);
            }
        }
    }
}
