using System;
using System.Collections.Generic;
using CircleOfLife.General;
using Milease.Core.Animator;
using Milease.Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CircleOfLife
{
    public class SaveListItem : MilListViewItem
    {
        public TMP_Text Name, PlayTime, SaveTime, Day;
        public GameObject NewPanel;
        
        protected override IEnumerable<MilStateParameter> ConfigDefaultState()
            => ArraySegment<MilStateParameter>.Empty;

        protected override IEnumerable<MilStateParameter> ConfigSelectedState()
            => ArraySegment<MilStateParameter>.Empty;

        public override void OnSelect(PointerEventData eventData)
        {
            var data = (SaveCombine.SaveData)Binding;
            var saveMode = SaveUI.Instance.SaveMode;
            
            if (saveMode)
            {
                if (data != null)
                {
                    MessageBox.Open(("覆盖存档", "您确定要覆盖当前存档吗，该操作不可逆！"), (o) =>
                    {
                        if (o == MessageBox.Operation.Deny)
                        {
                            return;
                        }
                        SaveManagement.Save(Index);
                        MessageBox.Open(("保存存档", "存档已保存！"));
                        SaveUI.Instance.Close();
                    });
                    return;
                }    
                SaveManagement.Save(Index);
                MessageBox.Open(("保存存档", "存档已保存！"));
                SaveUI.Instance.Close();
            }
            else
            {
                if (data == null)
                {
                    MessageBox.Open(("开始游戏", "这个存档是空的哦，选择其他存档或者新的游戏吧~"));
                    return;
                }    
                SaveManagement.SelectSaveData(Index);
            }
        }
        
        protected override void OnInitialize()
        {

        }

        protected override void OnTerminate()
        {
            
        }

        protected override MilInstantAnimator ConfigClickAnimation()
            => null;

        public override void UpdateAppearance()
        {
            var data = (SaveCombine.SaveData)Binding;
            NewPanel.SetActive(data == null);
            
            if (data == null)
            {
                return;
            }
            
            Name.text = $"存档{Index}";
            PlayTime.text = "游玩时长  " + data.Timer.ToString("g");
            SaveTime.text = data.LastSaveDate.ToString();
            Day.text = $"第 {data.CurrentDay} 天";
        }

        public override void AdjustAppearance(float pos)
        {
            
        }
    }
}
