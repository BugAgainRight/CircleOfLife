using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{

    /////////Stats
    public enum AnimalStat
    {

    }

    public enum EnemyStat
    {

    }

    public enum BuildStat
    {

    }

    /////////GameObject
    public enum AnimatonPrefab
    {

    }

    public enum EnemySkillType
    {
        test1
    }

    public enum PlayerSkillType
    {
        test1, test2, test3, test4, test5, test6
    }

    public enum BuildSkillType
    {
        TreatmentStation,SignalTransmitter1,TestBuildFriendFire
    }

    public class BuildSoData
    {
        public int Cost;
        public bool WhetherRotate;
        public GameObject Prefab;
    }
}
