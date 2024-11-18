using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    

    /////////Stats
    public enum AnimalStat
    {
        /// <summary>
        /// 藏獒
        /// </summary>
        TibetanMastiff,
    }

    public enum EnemyStat
    {
        EnemyA,
        EnemyBSignal,
        EnemyBGroup,
        EnemyC,
        EnemyD,
        EnemyF
    }

    public enum BuildStat
    {
        TreatmentStation, LogisticsService, SignalTransmitter, ProtectiveNet,
        ElectricFencing, BladeFencing
    }

    /////////GameObject
    
    public enum SharedPrefab
    {
        Melee,
        Ranged,
        RangedGroup,
        RangedLongest,
        RangedFast,
        RangedBoom,

    }


    public enum AnimatonPrefab
    {
        Single, Group, EnergyFull, SkillBurst
    }

    public enum EnemySkillType
    {
        test1
    }

    public enum PlayerSkillType
    {
        /// <summary>
        /// 近战攻击
        /// </summary>
        Melee,
        /// <summary>
        /// 远程攻击
        /// </summary>
        Ranged,
        /// <summary>
        /// 重击
        /// </summary>
        Whack,
        /// <summary>
        /// 挥砍
        /// </summary>
        Slash,
        /// <summary>
        /// 愈战愈勇
        /// </summary>
        FighterBraver,
        /// <summary>
        /// 疗伤
        /// </summary>
        Heal,
        /// <summary>
        /// 荆棘
        /// </summary>
        Thorn,
        /// <summary>
        /// 潜伏
        /// </summary>
        Lurk,
        /// <summary>
        /// 鼓舞
        /// </summary>
        Encouragement
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
        /// <summary>
        /// 召唤物
        /// </summary>
        SignalTransmitterNormalFriend,
        /// <summary>
        /// 召唤物变种1
        /// </summary>
        SignalTransmitter1Friend,
        /// <summary>
        /// 召唤物变种2
        /// </summary>
        SignalTransmitter2Friend,
        /// <summary>
        /// 召唤物变种3
        /// </summary>
        SignalTransmitter3Friend,




        TestBuildFriendFire

    }

    public enum AnimalSkillType
    {
        /// <summary>
        /// 藏獒普攻
        /// </summary>
        TibetanMastiffMelee,
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
