using System;
using System.Collections;
using System.Collections.Generic;
using CircleOfLife.Battle;
using CircleOfLife.Buff;
using CircleOfLife.Key;
using CircleOfLife.Units;
using Milease.Enums;
using Milease.Utils;
using Milutools.Recycle;
using UnityEngine;
using UnityEngine.Rendering;

namespace CircleOfLife
{
    public class PlayerSkills : MonoBehaviour
    {
        private const float ENERGY_MAX = 100f;
        private const float RECOVER_ON_HIT = 5f;
        public const float RECOVER_ON_ATTACK = 5f;

        private bool energyChanged = false;
        private float energy;
        
        private Vector3 direction;

        public float Energy
        {
            get => energy;
            set
            {
                var full = energy >= ENERGY_MAX;
                energy = Mathf.Clamp(value, 0f, ENERGY_MAX);
                energyChanged = true;
                if (!full && energy >= ENERGY_MAX)
                {
                    RecyclePool.Request(AnimatonPrefab.EnergyFull, (c) =>
                    {
                        c.Transform.position = transform.position;
                        c.Transform.localScale = Vector3.one * 3f; 
                        c.GameObject.SetActive(true);
                    }, Player.transform);
                }
            }
        }
        
        public PlayerSkillType SkillType, NormalHitType;
        public PlayerController Player;
        public SpriteRenderer EnergyFill;
        public LayerMask EnemyMask;
        public Volume SkillBurstVolume;
        
        private float hitTick = 0f;

        private void Awake()
        {
            Player.HurtAction = (context) =>
            {
                if (context.AttackerData == null)
                {
                    return;
                }
                Energy += RECOVER_ON_HIT;
            };
        }

        private void UpdateAttacks()
        {
            if (!Player.enabled)
            {
                return;
            }
            
            if (hitTick <= Player.Stats.Current.AttackInterval)
            {
                hitTick += Time.deltaTime;
                return;
            }

            if (KeyEnum.Attack.IsKeyUp())
            {
                hitTick = 0f;
                //Energy += RECOVER_ON_ATTACK;
                SkillManagement.GetSkill(NormalHitType)(new SkillContext(EnemyMask, Player.Stats)
                {
                    FireTransform = Player.SkillOffset
                });
            }
        }
        
        private static void Invincible(BattleStats stats, BuffContext buff)
        {
            stats.Current.Armor = int.MaxValue;
            stats.Current.EvasionRate = 1f;
            stats.Current.ReduceDamageRate = 1f;
        }

        private void UpdateEnergy()
        {
            if (!Player.enabled)
            {
                return;
            }
            
            if (Energy >= ENERGY_MAX && KeyEnum.Skill.IsKeyUp())
            {
                Player.enabled = false;
                Player.Stats.ApplyBuff(BuffUtils.ToBuff(Invincible));
                SkillBurstVolume.MileaseTo("weight", 1f, 0.5f, 0f, EaseFunction.Circ, EaseType.Out)
                    .Then(
                        new Action(() =>
                        {
                            RecyclePool.Request(AnimatonPrefab.SkillBurst, (c) =>
                            {
                                c.Transform.localPosition = Vector3.zero;
                                c.Transform.localScale = Vector3.one * 6f;
                                c.GameObject.SetActive(true);
                            }, Player.transform);
                        }).AsMileaseKeyEvent(),
                        SkillBurstVolume.MileaseTo("weight", 0f, 0.5f, 2f)
                    ).Play(() =>
                    {
                        Energy = 0f;
                        Player.Stats.ReduceBuff(BuffUtils.ToBuff(Invincible));
                        SkillManagement.GetSkill(SkillType)(new SkillContext(EnemyMask, Player.Stats)
                        {
                            FireTransform = Player.SkillOffset
                        });
                        Player.enabled = true;
                    });
            }
            
            if (Player.transform.localScale != direction)
            {
                direction = Player.transform.localScale;
                var scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction.x);
                transform.localScale = scale;
            }
            
            if (!energyChanged)
            {
                return;
            }

            energyChanged = false;
            EnergyFill.size = new Vector2(5f * (energy / ENERGY_MAX), EnergyFill.size.y);
        }
        
        private void Update()
        {
            UpdateAttacks();
            UpdateEnergy();
        }
    }
}
