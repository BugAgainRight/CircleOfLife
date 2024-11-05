using System;
using CircleOfLife.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public abstract class BuildBase : MonoBehaviour, IBattleEntity
    {
        public BattleStats.Stats Attribute;
        public BattleStats Stats { get; set; }
        public FactionType FactionType => factionType;
        [SerializeField]
        private FactionType factionType;

        public BattleRange BattleRange;
        public GameObject RangeObj;
        protected bool TimerFinish
        {
            get
            {
                if (timer + Stats.Current.AttackInterval <= Time.time)
                {
                    timer = Time.time;
                    return true;
                }
                return false;
            }
        }

        private float timer = 0;

        public class LevelUpDirection
        {
            public string Title;
            public Enum Value;
            public int NeedLevel;
        }
        public abstract List<LevelUpDirection> LevelUpDirections { get; }
        public abstract int Level { get; protected set; }
        public abstract void LevelUp(Enum direction = null);

        public abstract void HurtAction(BattleContext context);

        public void ShowRange()
        {
            RangeObj.SetActive(true);
            RangeObj.transform.localScale = new Vector3(BattleRange.Range.radius, BattleRange.Range.radius, 1);
        }
        public void OnReset()
        {
            Stats.Reset();
        }

    }
}
