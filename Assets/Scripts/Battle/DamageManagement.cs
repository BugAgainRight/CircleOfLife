using CircleOfLife.Units;
using Milutools.Recycle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public interface IDamageable_
    {
        public NPCData GetData();
    }
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
        private FlyWordStyle EnemyStyle;

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
        public void Damage(BattleContext battleContext)
        {
            bool isCrit=false;

            ///基础伤害
            float damage = battleContext.AttackerData.Atk;

            ///防御
            damage *= 1 - battleContext.HitData.Armor / (100 + battleContext.HitData.Armor);

            ///暴击伤害计算
            if (Random.Range(0, 1f) <= battleContext.AttackerData.CriticalChance)
            {
                damage *= (1 + battleContext.AttackerData.CriticalStrikeDamage);
                isCrit = true;
            }

            ///扣血
            //battleContext.HitData.-= damage;

            ///飘字
            var flyWord = RecyclePool.RequestWithCollection(Recycle.DamageText);

            ///激活
            flyWord.GameObject.SetActive(true);

            flyWord.Transform.position = battleContext.HitTran.position;

           
            ///样式选择
            FlyWordStyle style = new();
            FactionType attackerFaction = FactionType.Friend /*battleContext.AttackerData.*/;
            if (attackerFaction.Equals(FactionType.Friend))
            {
                if (isCrit) style = FriendCritStyle;
                else style = FriendNormalStyle;
            }
            else
            {
                style = EnemyStyle;
            }

            ///飘字初始化
            var flyWordComponent = flyWord.GetMainComponent<FlyWord>();
            flyWordComponent.Init(Mathf.RoundToInt(damage).ToString(), style);

           

        }




    }
}
