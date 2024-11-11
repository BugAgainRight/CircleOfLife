using Milutools.Milutools.UI;
using UnityEngine;

namespace CircleOfLife
{
    public class PlotBox : ManagedUI<PlotBox, TextAsset>
    {
        public DialogManager DialogManager;
        
        protected override void Begin()
        {
            
        }

        protected override void AboutToClose()
        {

        }

        public override void AboutToOpen(TextAsset parameter)
        {
            DialogManager.DialogDataFile = parameter;
        }
    }
}
