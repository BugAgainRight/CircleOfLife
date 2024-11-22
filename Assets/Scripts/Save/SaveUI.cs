using Milease.Core.UI;
using Milutools.Milutools.UI;

namespace CircleOfLife
{
    public class SaveUI : ManagedUI<SaveUI, bool>
    {
        public static SaveUI Instance;
        
        public MilListView ListView;
        public bool SaveMode;
        
        protected override void Begin()
        {
            
        }

        protected override void AboutToClose()
        {
            
        }

        public override void AboutToOpen(bool parameter)
        {
            Instance = this;
            SaveMode = parameter;
            foreach (var data in SaveManagement.GetAllSaveData())
            {
                ListView.Add(data);
            }
        }
    }
}
