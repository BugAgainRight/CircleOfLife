using System.Collections;
using System.Collections.Generic;
using CircleOfLife.Level;
using UnityEngine;

namespace CircleOfLife.Level
{
    public class LevelEnemy : MonoBehaviour
    {
        [HideInInspector]
        public string ID;
        void OnDestroy()
        {
            LevelController.Instance.UnRegisterEnemy(ID);
        }
    }
}
