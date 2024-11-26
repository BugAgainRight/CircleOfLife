using System;
using CircleOfLife.Configuration;
using CircleOfLife.Level;
using CircleOfLife.ScriptObject;
using Milutools.SceneRouter;
using UnityEngine;

namespace CircleOfLife.General
{
    public class StartBattleTrigger : MonoBehaviour
    {
        public static bool BanBattle = false;

        private void Awake()
        {
            if (SaveManagement.UseSaveData.CurrentDay == 5)
            {
                BanBattle = true;
            }
            else
            {
                BanBattle = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (BanBattle)
            {
                return;
            }
            LevelManager.Level = LevelFlowManager.Levels[SaveManagement.UseSaveData.CurrentDay];
            SceneRouter.GoTo(SceneIdentifier.Battle);
        }
    }
}
