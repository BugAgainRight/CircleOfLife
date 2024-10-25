using System;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife.Units
{
    [Serializable]
    public class NPCData
    {
        //名字、速度、最大生命值、攻击力、攻击范围、攻击间隔、能量、护甲、暴击率、暴击伤害、闪避率、
        //能量条
        //描述、预制体路径、立绘路径
        [Header("基础数据")]
        public string ID;
        public string Name;
        public float Velocity;
        public float MaxHp;
        public float Atk;
        public float Armor;
        public float AttackRange;
        public float AttackInterval;
        public float CriticalChance;
        public float CriticalStrikeDamage;
        public float EvasionRate;

        [Header("技能")]
        public float MaxEnergy;

        [Header("其他")]
        public string Description;
        public string PrefabPath;
        public List<string> picturePath;
    }
}
