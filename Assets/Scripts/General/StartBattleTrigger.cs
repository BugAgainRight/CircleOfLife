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
        private void OnTriggerEnter2D(Collider2D other)
        {
            LevelManager.Level = LevelFlowManager.Levels[SaveManagement.UseSaveData.CurrentDay];
            SceneRouter.GoTo(SceneIdentifier.Battle);
        }
    }
}
