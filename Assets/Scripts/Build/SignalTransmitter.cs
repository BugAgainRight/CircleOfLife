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
        public GameObject EffectPrefab;
        
        public override List<LevelUpDirection> LevelUpDirections => new()
        {
            new LevelUpDirection() { NeedLevel = 1, Title = "普通", Value = BuildSkillType.SignalTransmitterNormal },
            new LevelUpDirection() { NeedLevel = 2, Title = "变种1", Value = BuildSkillType.SignalTransmitter1 },
            new LevelUpDirection() { NeedLevel = 3, Title = "变种2", Value = BuildSkillType.SignalTransmitter2 },
            new LevelUpDirection() { NeedLevel = 3, Title = "变种3", Value = BuildSkillType.SignalTransmitter3 }
        };

        
        public override void HurtAction(BattleContext context)
        {
            if (context.HitData.Current.Hp <= 0)
            {
                RecyclePool.ReturnToPool(gameObject);
                ReturnAllFriend();
            }
        }

        private void Awake()
        {
            Level = 1;
            NowType = BuildSkillType.SignalTransmitterNormal;
            Stats = Attribute[0].Build(gameObject, HurtAction);
            RecyclePool.EnsurePrefabRegistered(BuildEffects.NewFriend, EffectPrefab, 20);
        }

        protected override void LevelUpFunc()
        {
            BattleRange.Range.radius = Stats.Current.EffectRange;
        }

        public override void FixedUpdateFunc()
        {
           
            if (transform.childCount <= MaxCount && TimerFinish)
            {
                SkillContext skillContext = new(PhysicsLayer, Stats);

                SkillManagement.GetSkill((BuildSkillType)NowType)(skillContext);

            }
        }

        public override void OnColliderEnterFunc(Collision2D collision)
        {
           
        }

        public override void OnColliderTriggerFunc(Collision2D collision)
        {
           
        }

        public override void OnEnableFunc()
        {
           
            NowType = BuildSkillType.SignalTransmitterNormal;
           
           
        }

        public override void OnDisableFunc()
        {
          
        }

        public void ReturnAllFriend()
        {
            for (var i = 0; ; i++)
            {
                if (i >= transform.childCount)
                {
                    break;
                }
                    
                var child = transform.GetChild(i);
                if (!child.CompareTag("SignalFriend")) continue;
                RecyclePool.ReturnToPool(child.gameObject);
                i--;
            }
        }
    }
}
