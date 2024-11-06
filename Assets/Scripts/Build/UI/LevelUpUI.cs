using System.Linq;
using CircleOfLife.General;
using Milease.Core.UI;
using Milutools.Milutools.UI;
using TMPro;
using UnityEngine;

namespace CircleOfLife.Build.UI
{
    public class LevelUpUI : ManagedUI<LevelUpUI, LevelUpUIData, BuildBase.LevelUpDirection>
    {
        public MilListView ListView;
        public TMP_Text Cost, Title;
        public GameObject NoDirection;
        
        private LevelUpUIData data;
        
        protected override void Begin()
        {
            
        }

        protected override void AboutToClose()
        {
            
        }

        public void Cancel()
        {
            Close(null);
        }
        
        public void Finish()
        {
            if (data.Material < data.Need)
            {
                MessageBox.Open(("啊哦", "材料不足升级！"));
                return;
            }
            Close((BuildBase.LevelUpDirection)ListView.Items[ListView.SelectedIndex]);
        }

        public override void AboutToOpen(LevelUpUIData parameter)
        {
            data = parameter;
            Title.text = $"等级 {data.Target.Level} -> {data.Target.Level + 1}";
            Cost.text = $"消耗 {data.Need} 升级";
            var direction = data.Target.LevelUpDirections;
            foreach (var dir in direction.Where(x => x.NeedLevel == data.Target.Level + 1))
            {
                ListView.Add(dir);
            }

            if (ListView.Items.Count == 0)
            {
                NoDirection.SetActive(true);
            }
            else
            {
                ListView.Select(0);
            }
        }
    }
}
