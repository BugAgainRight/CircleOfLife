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
        /// <summary>
        /// 藏羚羊
        /// </summary>
        TibetanAntelope,
        /// <summary>
        /// 狼
        /// </summary>
        Wolf,
        ///<summary>
        /// 猎隼
        /// </summary>
        FalcoCherrug,
        /// <summary>
        /// 藏棕熊
        /// </summary>
        Bear,
        /// <summary>
        /// 野牦牛
        /// </summary>
        WildYak
    }

    public enum EnemyStat
    {
        EnemyA,
        EnemyBSignal,
        EnemyBGroup,
        EnemyC,
        EnemyD,
        EnemyF,
        Boss
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
        RangedGroupDizz

    }


    public enum AnimatonPrefab
    {
        Single, Group, EnergyFull, SkillBurst, EnemyRecovery, SummonEnemy, Dizzy
    }

    public enum EnemySkillType
    {
        test1,
        /// <summary>
        /// boss防御技能
        /// </summary>
        BossSkill1,
        /// <summary>
        /// boss攻击技能
        /// </summary>
        BossSkill2,
        /// <summary>
        /// boss攻击技能
        /// </summary>
        BossSkill3,
        /// <summary>
        /// boss辅助技能
        /// </summary>
        BossSkill4
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
        /// <summary>
        /// 藏羚羊普攻
        /// </summary>
        TibetanAntelopeMelee,
        /// <summary>
        /// 狼普攻
        /// </summary>
        WolfMelee,
        /// <summary>
        /// 猎隼普攻
        /// </summary>
        FalcoCherrugMelee,
        /// <summary>
        /// 熊普攻
        /// </summary>
        BearMelee,
        /// <summary>
        /// 野牦牛普攻
        /// </summary>
        WildYakMelee,
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
