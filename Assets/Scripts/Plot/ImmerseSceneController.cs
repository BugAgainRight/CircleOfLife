using System;
using UnityEngine;

namespace CircleOfLife
{
    public class ImmerseSceneController : MonoBehaviour
    {
        public static TextAsset Plot;
        public static Action PostAction;

        private void Awake()
        {
            PlotBox.Open(Plot, PostAction);
        }
    }
}
