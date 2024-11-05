using System;
using CircleOfLife.Battle;
using CircleOfLife.Buff;
using Milutools.Recycle;
using UnityEngine;

namespace CircleOfLife.Experiments
{
    public class BuffTest : MonoBehaviour
    {
        public enum Effect
        {
            Skill1, Skill2, Hit1, Hit2
        }
        public GameObject Entity;
        
        public GameObject Skill1Prefab, Skill2Prefab;
        public GameObject Hit1Prefab, Hit2Prefab;

        public static void Excited(BattleStats stats, BuffContext buff)
        {
            stats.Current.Velocity *= 10f;
            stats.Current.AttackInterval *= (1f - buff.Level * 0.25f);
        }
        
        public static void Blood(BattleStats stats, BuffContext buff)
        {
            var position = stats.Transform.position;
            if (buff.IsMeet<Vector3>("position", x => x.Equals(position)))
            {
                return;
            }

            buff.Set<Vector3>("position", _ => position);

            if (buff.TickedTime >= 1f)
            {
                buff.ResetTickedTime();
                var collection = RecyclePool.RequestWithCollection(Effect.Hit2);
                collection.Transform.position = stats.Transform.position;
                collection.GameObject.SetActive(true);
                DamageManagement.BuffDamage(stats, 20f);
            }
        }
        
        private void Awake()
        {
            RecyclePool.EnsurePrefabRegistered(Effect.Skill1, Skill1Prefab, 10);
            RecyclePool.EnsurePrefabRegistered(Effect.Skill2, Skill2Prefab, 10);
            RecyclePool.EnsurePrefabRegistered(Effect.Hit1, Hit1Prefab, 10);
            RecyclePool.EnsurePrefabRegistered(Effect.Hit2, Hit2Prefab, 10);
        }

        private void TestBuff(BuffContext buff, Effect skill, Effect hit)
        {
            var pos = (Vector2)Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            
            var collection = RecyclePool.RequestWithCollection(skill);
            collection.Transform.localScale = Vector3.one * 2f; 
            collection.Transform.position = pos;
            collection.GameObject.SetActive(true);
            
            var colliders = Physics2D.OverlapCircleAll(pos, 2f);
            foreach (var col in colliders)
            {
                var stat = col.GetBattleStats();
                collection = RecyclePool.RequestWithCollection(hit);
                collection.Transform.position = stat.Transform.position;
                collection.GameObject.SetActive(true);
                stat.ApplyBuff(buff);
            }
        }
        
        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                TestBuff(BuffUtils.ToBuff(Excited, 3f), Effect.Skill1, Effect.Hit1);
            }
            
            if (Input.GetMouseButtonUp(1))
            {
                TestBuff(BuffUtils.ToBuff(Blood, 3f), Effect.Skill2, Effect.Hit2);
            }
            
            if (Input.GetMouseButtonUp(2))
            {
                var go = Instantiate(Entity);
                go.transform.position = (Vector2)Camera.main!.ScreenToWorldPoint(Input.mousePosition);
                go.SetActive(true);
            }
        }
    }
}
