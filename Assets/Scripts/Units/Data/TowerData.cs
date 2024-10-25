using System;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife.Units
{
    [Serializable]
    public class TowerData
    {
        //名字、攻击范围、最大生命值、攻击间隔、攻击力、护甲、暴击率、暴击伤害
        //部署费用、升级费用
        //描述、预制体路径、立绘路径
        public string ID;
        public string Name;
        public float AttackRange;
        public float MaxHp;
        public float AttackInterval;
        public float Atk;
        public float Armor;
        public float CriticalChance;
        public float CriticalStrikeDamage;
        [Header("费用")]
        public float DeploymentCost;
        public float LevelUpCost;
        [Header("其他")]
        public string Description;
        public string PrefabPath;
        public List<string> PicturePath;
    }
}
