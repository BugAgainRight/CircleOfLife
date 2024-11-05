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
        TreatmentStation, LogisticsService, SignalTransmitter, ProtectiveNet,
        ElectricFencing, BladeFencing
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
        Whack, Slash, Skill3, test4, test5, test6
    }

    public enum BuildSkillType
    {
        /// <summary>
        /// 治疗站
        /// </summary>
        TreatmentStationNormal, 
        /// <summary>
        /// 兽医站
        /// </summary>
        TreatmentStation1, 
        /// <summary>
        /// 战斗医院
        /// </summary>
        TreatmentStation2,
        /// <summary>
        /// 刀片网
        /// </summary>
        BladeFencing, 
        /// <summary>
        /// 电网
        /// </summary>
        ElectricFencing, 
        /// <summary>
        /// 后勤处
        /// </summary>
        LogisticsService,
        /// <summary>
        /// 信号发射器
        /// </summary>
        SignalTransmitterNormal,
        /// <summary>
        /// 信号发射器变种1
        /// </summary>
        SignalTransmitter1,
        /// <summary>
        /// 信号发射器变种2
        /// </summary>
        SignalTransmitter2,
        /// <summary>
        /// 信号发射器变种3
        /// </summary>
        SignalTransmitter3,
        TestBuildFriendFire

    }

    public enum AnimalSkillType
    {
        test1
    }

    public static class EnumExtendFuncs
    {
        public static FactionType Reversal(this FactionType factionType)
        {
            if (factionType.Equals(FactionType.Enemy)) return FactionType.Friend;
            return FactionType.Enemy;
        }
    }


}
