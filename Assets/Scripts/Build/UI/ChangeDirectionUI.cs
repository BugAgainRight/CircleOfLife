using System;
using System.Linq;
using CircleOfLife.General;
using Milease.Core.UI;
using Milutools.Milutools.UI;
using TMPro;
using UnityEngine;

namespace CircleOfLife.Build.UI
{
    public class ChangeDirectionUI : ManagedUI<ChangeDirectionUI, LevelUpUIData, LevelUpResponse>
    {
        public MilListView ListView;
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
            Close(new LevelUpResponse()
            {
                Confirm = false
            });
        }
        
        public void Finish()
        {
            Close(new LevelUpResponse()
            {
                Direction = ListView.Items.Count == 0 ? null : (BuildBase.LevelUpDirection)ListView.Items[ListView.SelectedIndex],
                Confirm = true
            });
        }

        public override void AboutToOpen(LevelUpUIData parameter)
        {
            data = parameter;

            var direction = data.Target.LevelUpDirections;
            foreach (var dir in direction.Where(x => x.NeedLevel <= data.Target.Level))
            {
                ListView.Add(dir);
            }

            if (ListView.Items.Count == 0)
            {
                NoDirection.SetActive(true);
            }
            else
            {
                ListView.Select(
                    Math.Max(0,
                        ListView.Items.FindIndex(x => 
                            Equals(((BuildBase.LevelUpDirection)x).Value, data.Target.NowType)
                        ))
                    );
            }
        }
    }
}
