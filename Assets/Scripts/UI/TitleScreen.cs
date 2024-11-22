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
            SceneRouter.GoTo(SceneIdentifier.Battle);
        }
    }
}
