using CircleOfLife.Battle;
using CircleOfLife.Buff;
using Milutools.Recycle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class SignalTransmitterFriend : MonoBehaviour, IBattleEntity
    {
        public BuildFriendContext context;

        public BattleStats.Stats Attribute;
        public BattleStats Stats { get; set; }

        public FactionType FactionType => FactionType.Friend;

        private void Awake()
        {
            Stats = Attribute.Build(gameObject, HurtAction);
            context.BattleStat = Stats;
        }
        private void HurtAction(BattleContext context)
        {
            if (context.HitData.Current.Hp <= 0) RecyclePool.ReturnToPool(gameObject);
            return;
        }

    }
}
