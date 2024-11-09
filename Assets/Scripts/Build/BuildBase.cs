using System;
using CircleOfLife.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CircleOfLife.Buff;
using RuiRuiAstar;
using RuiRuiVectorField;
using Milease.Utils;
using Milease.Core.Animator;

namespace CircleOfLife
{
    public abstract class BuildBase : MonoBehaviour, IBattleEntity, IDestoey
    {
        public bool Switch { get; set; }
        public int DestroyCost;
        public List<BattleStats.Stats> Attribute;
        public BattleStats Stats { get; set; }
        public FactionType FactionType => factionType;
        [SerializeField]
        private FactionType factionType;

        public BattleRange BattleRange;
        public GameObject RangeObj;

        public LayerMask PhysicsLayer;
        private float needResetTime = 0;
        private bool needReset=false;
        protected bool TimerFinish
        {
            get
            {
                if(needReset&& needResetTime<Time.time) timer = Time.time;
                if (timer + Stats.Current.AttackInterval <= Time.time)
                {       
                    needReset = true;
                    return true;
                }
                needReset = false;
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
        public int Level { get; protected set; }

        public Enum NowType { get; protected set; }

        protected List<Enum> allType=new();

        public bool WhetherSelectDirection;

        protected abstract void LevelUpFunc();
         
        public void LevelUp(Enum direction = null)
        {
            Level++;
            NowType = direction;
            if (direction != null&&!allType.Contains(direction)) allType.Add(direction);
            ReplaceStats(Attribute[Level - 1]);
            LevelUpFunc();
        }
        public void ChangeDirection(Enum direction)
        {
            NowType = direction;
        }

        public abstract void HurtAction(BattleContext context);

        public void ShowRange()
        {
            RangeObj.SetActive(true);
            UpdateRange();
        }
        public void CloseRange()
        {
            RangeObj.SetActive(false);
        }
        public void OnReset()
        {
            Stats.Reset();
            
        }

        public void ReplaceStats(BattleStats.Stats stats,bool isReset=false)
        {
            float befoHP = Stats.Current.Hp;
            Stats.ReplaceBaseStat(stats);
            if (!isReset) Stats.SetHP(Mathf.Min(befoHP, Stats.Current.Hp));
        }


        private bool NeedRecovery
        {
            get
            {
                if (recoveryTimer + 1 <= Time.time)
                {
                    recoveryTimer = Time.time;
                    return true;
                }
                return false;

            }
        }

        private float recoveryTimer=0;

        public void RecoveryHP()
        {
            if (NeedRecovery)
            {
                var list = BattleRange.GetAllFriendInRange(PhysicsLayer, factionType);
               if(list.Count>0) DamageManagement.BuffDamage(Stats, -5 * list.Count);
            }
        }

        public void UpdateRange()
        {
            BattleRange.Range.radius = Stats.Current.EffectRange;
            RangeObj.transform.localScale = new Vector3(BattleRange.Range.radius, BattleRange.Range.radius, 1);
        }

        public int DestoryCost()
        {
            return DestroyCost;
        }

        public abstract void OnEnableFunc();
        public abstract void OnDisableFunc();
    
        public abstract void FixedUpdateFunc();
        public abstract void OnColliderEnterFunc(Collision2D collision);
        public abstract void OnColliderTriggerFunc(Collision2D collision);


        private void FixedUpdate()
        {
            if (!Switch) return;
            RecoveryHP();
            FixedUpdateFunc();
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!Switch) return;
            OnColliderEnterFunc(collision);
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (!Switch) return;
            OnColliderTriggerFunc(collision);
        }
        private void OnDisable()
        {
            OnDisableFunc();
            UpdateAstarAndFieldVector();
        }
        private void OnEnable()
        {
            OnEnableFunc();
            UpdateAstarAndFieldVector();
        }

        protected void UpdateAstarAndFieldVector()
        {
            new Action(() =>
            {
                int x = Mathf.RoundToInt(transform.position.x);
                int y = Mathf.RoundToInt(transform.position.y);
                VectorField.PartialUpdates(x - 4, x + 4, y - 4, y + 4);
                Astar.CheckObstacles(new RuiRuiSTL.RangeBox2D(x - 4, x + 4, y - 4, y + 4));
            }).AsMileaseKeyEvent().Delayed(1f).Play();
        }

    }
}
