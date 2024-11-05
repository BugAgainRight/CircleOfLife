using CircleOfLife.Battle;
using CircleOfLife.Buff;
using Milutools.Recycle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class TestBuildFriend_Chen : MonoBehaviour,IBattleEntity
    {
        public BuildFriendContext context;
        public BattleStats Stats { get; set; }

        public FactionType FactionType => FactionType.Friend;

        public void PassData(BattleStats.Stats stats)
        {
            Stats = stats.Build(gameObject, HurtAction);
            context.BattleStat = Stats;
        }
        public void HurtAction(BattleContext context)
        {
            if (context.HitData.Current.Hp <= 0) RecyclePool.ReturnToPool(gameObject);
            return;

        }
    }
}
