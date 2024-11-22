using System;
using System.Collections;
using System.Collections.Generic;
using CircleOfLife.General;
using Milease.Core.UI;
using Milutools.Milutools.UI;
using TMPro;
using UnityEngine;

namespace CircleOfLife
{
    public delegate string EnumDescriber(Enum value);
    public class EnumPopupUIData
    {
        public string Title, Description;
        public EnumDescriber Describer;
        public List<object> List;
        public int DefaultSelection = -1;
    }
    public class EnumPopupUI : ManagedUI<EnumPopupUI, EnumPopupUIData, object>
    {
        public TMP_Text Title, Description;
        public MilListView ListView;
        private List<object> srcList;
        
        protected override void Begin()
        {
            
        }

        public void Confirm()
        {
            if (ListView.SelectedIndex == -1)
            {
                MessageBox.Open(("请先选择", "必须选择一个哦~"));
                return;
            }
            Close(srcList[ListView.SelectedIndex]);
        }
        
        protected override void AboutToClose()
        {
 
        }
        
        public override void AboutToOpen(EnumPopupUIData parameter)
        {
            Title.text = parameter.Title;
            Description.text = parameter.Description;
            
            srcList = parameter.List;
            foreach (var value in srcList)
            {
                ListView.Add(parameter.Describer((Enum)value));
            }

            if (parameter.DefaultSelection != -1)
            {
                ListView.Select(parameter.DefaultSelection, true);
            }
        }
    }
}
