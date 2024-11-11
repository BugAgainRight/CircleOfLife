using CircleOfLife.AI;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

public class TestDamageToHP : MonoBehaviour
{
    
    [UnityTest]
    public IEnumerator TestDamageToHp()
    {

        Resources.Load("TestPrefabs/MapTest");
        GameObject enemyA = Resources.Load<GameObject>("TestPrefabs/EnemyA");

        Instantiate(enemyA);
        yield return new WaitForSeconds(2);

        yield return null;
    }
}
