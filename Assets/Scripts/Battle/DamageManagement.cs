using CircleOfLife.Battle;
using CircleOfLife.Units;
using Milutools.Recycle;
using System.Collections;
using System.Collections.Generic;
using CircleOfLife.Buff;
using UnityEngine;

namespace CircleOfLife
{
    public class DamageManagement : MonoBehaviour
    {
        
        public enum Recycle
        {
            DamageText
        }

        public static DamageManagement Instance;

        [SerializeField]
        private GameObject FlyWordPrefab;



        [Tooltip("友军攻击飘字样式")]
        [SerializeField]
        private FlyWordStyle FriendNormalStyle;
        [Tooltip("友军攻击暴击飘字样式")]
        [SerializeField]
        private FlyWordStyle FriendCritStyle;

        [Tooltip("敌军攻击飘字样式")]
        [SerializeField]
        private FlyWordStyle EnemyNormalStyle;

        [Tooltip("敌军攻击暴击飘字样式")]
        [SerializeField]
        private FlyWordStyle EnemyCritStyle;

        [Tooltip("友军治疗飘字样式")]
        [SerializeField]
        private FlyWordStyle FriendRecovery;  
        
        [Tooltip("敌军治疗飘字样式")]
        [SerializeField]
        private FlyWordStyle EnemyRecovery;



        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            RecyclePool.EnsurePrefabRegistered(Recycle.DamageText, FlyWordPrefab, 20);
        }

        /// <summary>
        /// 计算造成的伤害数值
        /// </summary>
        public static float GetDamage(BattleStats.Stats attackStats, BattleStats.Stats hitStats, float skillRate, out bool isCrit)
        {
            isCrit = false;
            
            //基础伤害
            float damage = attackStats.Attack;
            
            //治疗
            if(damage >0)
            {
                //防御
                damage *= 1 - hitStats.Armor / (100 + hitStats.Armor);

                //暴击伤害计算
                if (Random.Range(0, 1f) <= attackStats.CriticalChance)
                {
                    damage *= 2;
                    isCrit = true;
                }
                damage *= skillRate;
                damage *= Mathf.Max(0, (100 - hitStats.Armor)) *0.01f;
                damage *= (1 - attackStats.ReduceDamageRate);
            }

            return damage;
        }

        /// <summary>
        /// battleContext 必须填入参数：AttackerData HitData（受击者）HitTran
        /// </summary>
        /// <param name="context"></param>
        public static void Damage(BattleContext context)
        {
            var damage = GetDamage(context.AttackerData.Current, context.HitData.Current, context.SkillRate, out var isCrit);
            context.HitData.Damage(damage, context);
            FlyWord(damage, context.HitData, isCrit);
        }

        /// <summary>
        /// 由 Buff 造成伤害
        /// </summary>
        /// <param name="hitData">受伤者</param>
        /// <param name="damage">数值</param>
        public static void BuffDamage(BattleStats hitData, float damage)
        {
            hitData.Damage(damage, hitData.WrapBuffBattleContext());
            FlyWord(damage, hitData, false);
        }
        
        public static void FlyWord(float damage, BattleStats hitData, bool isCrit = false)
        {
            bool isRecovery = damage < 0;

            ///飘字
            var flyWord = RecyclePool.RequestWithCollection(Recycle.DamageText);

            ///激活
            flyWord.GameObject.SetActive(true);

            flyWord.Transform.position = hitData.Transform.position + new Vector3(Random.Range(-1, 1f), Random.Range(-1, 1f));


            ///样式选择
            FactionType attackerFaction = hitData.BattleEntity.FactionType.Reversal();
            FlyWordStyle style = Instance.GetStyle(attackerFaction, isCrit, isRecovery);

            ///飘字初始化
            var flyWordComponent = flyWord.GetMainComponent<FlyWord>();
            if (damage < 0) flyWordComponent.Init("+" + Mathf.RoundToInt(-damage).ToString(), style, false);
            else flyWordComponent.Init(Mathf.RoundToInt(damage).ToString(), style);
        }

        public void Damage(BattleContext battleContext,System.Action<BattleContext> otherAction)
        {
            Damage(battleContext);
            otherAction?.Invoke(battleContext);
        }




        private FlyWordStyle GetStyle(FactionType factionType,bool isCrit,bool isRecovery)
        {
            FlyWordStyle result;
            if (factionType.Equals(FactionType.Friend))
            {
                if (isRecovery) result = FriendRecovery;
                else if(isCrit) result = FriendCritStyle;
                else result = FriendNormalStyle;
            }
            else
            {
                if (isRecovery) result = EnemyRecovery;
                else if (isCrit) result = EnemyCritStyle;
                else result = EnemyNormalStyle;
             
            }

            return result;

        }



    }
}
