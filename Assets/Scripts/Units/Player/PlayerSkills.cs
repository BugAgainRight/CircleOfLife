using System;
using System.Collections;
using System.Collections.Generic;
using CircleOfLife.Battle;
using CircleOfLife.Key;
using CircleOfLife.Units;
using Milutools.Recycle;
using UnityEngine;

namespace CircleOfLife
{
    public class PlayerSkills : MonoBehaviour
    {
        public static PlayerSkills Instance;
        
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

        private float hitTick = 0f;

        private void Awake()
        {
            Instance = this;
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

        private void UpdateEnergy()
        {
            if (!Player.enabled)
            {
                return;
            }
            
            if (Energy >= ENERGY_MAX && KeyEnum.Skill.IsKeyUp())
            {
                Energy = 0f;
                SkillManagement.GetSkill(SkillType)(new SkillContext(EnemyMask, Player.Stats)
                {
                    FireTransform = Player.SkillOffset
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
