using System;
using CircleOfLife.Battle;
using CircleOfLife.Buff;
using Milutools.Recycle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class SignalTransmitter : BuildBase
    {
        public int MaxCount;
        public GameObject FriendPrefab;
        public override List<LevelUpDirection> LevelUpDirections => new()
        {
            
        };

        public override int Level { get; protected set; }

        public override void LevelUp(Enum direction = null)
        {
            
        }
        
        public override void HurtAction(BattleContext context)
        {
            
        }

        private void Awake()
        {
            RecyclePool.EnsurePrefabRegistered(BuildSkillType.SignalTransmitter1, FriendPrefab, 20);
            Stats = Attribute.Build(gameObject, HurtAction);
        }

        private void FixedUpdate()
        {
            if (transform.childCount <= MaxCount && TimerFinish)
            {
                SkillContext skillContext = new(1 << 0, Stats);
              
                SkillManagement.GetSkill(BuildSkillType.SignalTransmitter1)(skillContext);

            }
        }


    }
}
