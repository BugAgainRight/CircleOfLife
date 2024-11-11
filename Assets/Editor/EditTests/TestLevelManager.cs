using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using TMPro;
using CircleOfLife.Level;
using CircleOfLife.Units;
using System.Collections.Generic;
using System;
using System.Linq;
using CircleOfLife.Battle;
using CircleOfLife.Buff;
using CircleOfLife.Build;
using CircleOfLife.Build.UI;
using CircleOfLife.General;
using CircleOfLife.ScriptObject;
using Milease.Core;
using Milease.Core.Animator;
using Milease.Enums;
using Milease.Utils;
using Milutools.Recycle;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace CircleOfLife.Tests
{
    public class LevelManagerTests
    {
        private GameObject levelManagerObject;
        private LevelManager levelManager;

        [SetUp]
        public void SetUp()
        {
            levelManagerObject = new GameObject();
            levelManager = levelManagerObject.AddComponent<LevelManager>();
            levelManager.MaterialText = new GameObject().AddComponent<TMP_Text>();
            levelManager.MapGrid = new GameObject().AddComponent<Grid>();
            levelManager.ServicePostProcess = new GameObject().AddComponent<Volume>();
            levelManager.MainCanvas = new GameObject().AddComponent<CanvasGroup>();
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(levelManagerObject);
        }
        /*
        [UnityTest]
        public IEnumerator TestSupplyMaterial()
        {
            int initialMaterial = 10;
            int supplyAmount = 5;

            levelManager.Material = initialMaterial;
            levelManager.SupplyMaterial(supplyAmount);

            yield return null;

            Assert.AreEqual(initialMaterial + supplyAmount, levelManager.Material);
            Assert.IsTrue(levelManager.MaterialText.text.Contains((initialMaterial + supplyAmount).ToString()));
        }

        [UnityTest]
        public IEnumerator TestLoadLevel()
        {
            levelManager.LoadLevel("SampleLevel");

            yield return null;

            Assert.AreEqual(0, levelManager.Material);
            Assert.IsNotNull(levelManager.MapGrid);
            Assert.IsFalse(levelManagerObject.GetComponent<PlayerController>().enabled);
        }

        [UnityTest]
        public IEnumerator TestEnemySummon()
        {
            var wave = new LevelWave
            {
                Enemies = new List<EnemyData>
                {
                    new EnemyData { Enemy = "TestEnemy", SummonCount = 1 }
                }
            };

            var appearPoint = new GameObject().AddComponent<AppearPoint>();
            var rect = new Rect(0, 0, 10, 10);
            levelManager.RegisterPoint(appearPoint, rect);

            levelManager.SummonEnemy(wave);

            yield return null;

            Assert.AreEqual(1, levelManagerObject.GetComponent<RecyclePool>().CountActive("TestEnemy"));
        }

        [UnityTest]
        public IEnumerator TestRoundTransition()
        {
            curRound = 0;
            levelManager.LoadLevel("SampleLevel");

            // Mock the setup to skip to the next round
            levelManager.NotifyEnemyDeath(new GameObject());

            yield return null;

            Assert.AreEqual(1, curRound);
        }*/
    }
}
