using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife.Level
{
    public class LevelTestSc : MonoBehaviour
    {
        private bool isClickDown = false;

        public string LevelID;

        private void Awake()
        {

        }
        void Start()
        {
            LevelManager.OnWaveStart((wave) =>
            {
                Debug.Log("OnWaveStart WaveCount" + wave);
            });
            LevelManager.OnWaveEnd((wave) =>
            {
                Debug.Log("OnWaveEnd WaveCount:" + wave);
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
                Debug.Log("OnEnemyCreated:" + enemy.name);
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
        }

        private void LoadLevel()
        {
            Debug.Log("LoadLevel:" + LevelID);
            LevelManager.LoadLevel(LevelID);
        }

        private void StarWave()
        {
            Debug.Log("StarWave:" + LevelUtils.CurrentWave);
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

        public void Message()
        {
            Debug.Log("Message");
        }
    }
}
