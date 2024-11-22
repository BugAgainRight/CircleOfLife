using System.Collections;
using System.Collections.Generic;
using CircleOfLife.Configuration;
using Milutools.SceneRouter;
using UnityEngine;

namespace CircleOfLife
{
    public class CreditsHelper : MonoBehaviour
    {
        public void GoBackToTitleScreen()
        {
            SceneRouter.GoTo(SceneIdentifier.TitleScreen);
        }
    }
}
