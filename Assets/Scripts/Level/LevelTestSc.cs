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
            Debug.Log("StarWave:" + LevelUtils.CurrentWave);
            LevelManager.StartWave();
        }
        private void EndWave()
        {
            Debug.Log("EndWave:" + LevelUtils.CurrentEnemyCount);
            LevelManager.EndWave();
        }
    }
}
