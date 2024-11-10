using System;
using CircleOfLife.Buff;
using CircleOfLife.Level;
using UnityEngine;

namespace CircleOfLife.Battle
{
    public class ProtectAnimal : MonoBehaviour, IBattleEntity
    {
        public BattleStats.Stats Stat;
        public BattleStats Stats { get; set; }
        public FactionType FactionType => FactionType.Friend;

        private void Awake()
        {
            Stats = Stat.Build(gameObject, (context) =>
            {
                if (Stats.Current.Hp <= 0f)
                {
                    LevelManager.Instance.Fail("没能保护好小动物……");
                    gameObject.SetActive(false);
                }
            });
        }
    }
}
