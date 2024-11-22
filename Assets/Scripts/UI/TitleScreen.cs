using System.Collections;
using System.Collections.Generic;
using CircleOfLife.Configuration;
using Milutools.SceneRouter;
using UnityEngine;

namespace CircleOfLife
{
    public class TitleScreen : MonoBehaviour
    {
        public void NewGame()
        {
            SaveManagement.SelectSaveData(-1);
            SceneRouter.GoTo(SceneIdentifier.Battle);
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
