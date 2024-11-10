using System;
using System.Collections;
using System.Collections.Generic;
using CircleOfLife.ScriptObject;
using UnityEngine;

namespace CircleOfLife.Level.Old
{
    public class LevelTestSc : MonoBehaviour
    {
        private bool isClickDown = false;

        public LevelEnum LevelID;

        void Start()
        {
            LevelManager.OnWaveStart((wave) =>
            {
                Debug.Log("OnWaveStart WaveCount:" + wave);
            });
            LevelManager.OnWaveEnd((wave) =>
            {
                Debug.Log("OnWaveEnd WaveCount:" + wave);
                Debug.Log("CurrentCost:" + LevelManager.GetCost());
            });
            LevelManager.OnLevelWin((id) =>
            {
                Debug.Log("OnLevelWin LevelID:" + id);
            });
            LevelManager.OnLevelLose((id) =>
            {
                Debug.Log("OnLevelLose LevelID:" + id);
            });
            LevelManager.OnEnemyCreated((enemy) =>
            {
                //Debug.Log("OnEnemyCreated:" + enemy.name);
            });

        }

        // Update is called once per frame
        void Update()
        {
            if (!isClickDown)
            {
                CheckKeyBoard();
                isClickDown = false;
            }
        }
        private void CheckKeyBoard()
        {
            if (!isClickDown && Input.GetKeyDown(KeyCode.Q))
            {
                isClickDown = true;
                LoadLevel();
            }
            if (!isClickDown && Input.GetKeyDown(KeyCode.W))
            {
                isClickDown = true;
                StarWave();
            }
            if (!isClickDown && Input.GetKeyDown(KeyCode.E))
            {
                isClickDown = true;
                Win();
            }
            if (!isClickDown && Input.GetKeyDown(KeyCode.R))
            {
                isClickDown = true;
                Lose();
            }
            if (!isClickDown && Input.GetKeyDown(KeyCode.T))
            {
                isClickDown = true;
                EndWave();
            }
        }

        private void LoadLevel()
        {
            Debug.Log("LoadLevel:" + LevelID);
            LevelManager.LoadLevel(LevelID);
        }

        private void StarWave()
        {
            Debug.Log("StarWave:" + LevelContext.CurrentWave);
            LevelManager.StartWave();
        }
        private void Win()
        {
            Debug.Log("Win");
            LevelManager.LevelWin();
        }
        private void Lose()
        {
            Debug.Log("Lose");
            LevelManager.LevelLose();
        }
        private void EndWave()
        {
            Debug.Log("EndWave");
            LevelManager.EndWave();
        }

        public void Message()
        {
            Debug.Log("Message");
        }
    }
}
