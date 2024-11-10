using System;
using CircleOfLife.Level;
using UnityEngine;

namespace CircleOfLife.Experiments
{
    public class LevelTest : MonoBehaviour
    {
        public string Level;

        private void Start()
        {
            LevelManager.Instance.LoadLevel(Level);
        }
    }
}
