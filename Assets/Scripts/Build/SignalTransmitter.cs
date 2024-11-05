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
        public override void HurtAction(BattleStats battleStats)
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
