using System;
using System.Collections;
using System.Collections.Generic;
using CircleOfLife.Battle;
using CircleOfLife.Buff;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CircleOfLife.Experiments
{
    public class TestEntity : MonoBehaviour, IBattleEntity
    {
        public SpriteRenderer HPBar;
        public BattleStats.Stats Stat;
        public BattleStats Stats { get; set; }
        public FactionType FactionType { get; } = FactionType.Enemy;

        private float curDirection, moveTime;

        private void Awake()
        {
            Stats = Stat.Build(gameObject, (s) =>
            {
                if (s.Current.Hp <= 0f)
                {
                    Destroy(gameObject);
                }
            });
        }

        private void FixedUpdate()
        {
            moveTime -= Time.fixedDeltaTime;
            if (moveTime <= 0f)
            {
                moveTime = Random.Range(0.2f, 0.6f);
                curDirection = Random.Range(-1f * Mathf.PI, Mathf.PI * 1f);
            }

            var pos = transform.position;
            pos.x += Stats.Current.Velocity * Mathf.Sin(curDirection) * Time.fixedDeltaTime;
            pos.y += Stats.Current.Velocity * Mathf.Cos(curDirection) * Time.fixedDeltaTime;
            transform.position = pos;

            HPBar.size = new Vector2(2.8f * Mathf.Max(0f, Stats.Current.Hp / Stats.Max.Hp), HPBar.size.y);
        }
    }
}
