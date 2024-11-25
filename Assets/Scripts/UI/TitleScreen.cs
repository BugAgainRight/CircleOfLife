using System.Collections;
using System.Collections.Generic;
using CircleOfLife.Configuration;
using CircleOfLife.ScriptObject;
using Milutools.SceneRouter;
using UnityEngine;

namespace CircleOfLife
{
    public class TitleScreen : MonoBehaviour
    {
        public void NewGame()
        {
            SaveManagement.SelectSaveData(-1);
            ImmerseSceneController.Plot = LevelFlowManager.Flow.OpeningPlot;
            ImmerseSceneController.PostAction = () =>
            {
                SceneRouter.GoTo(SceneIdentifier.Village);
            };
            SceneRouter.GoTo(SceneIdentifier.ImmersePlot);
        }

        public void ContinueGame()
        {
            SaveUI.Open(false);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
