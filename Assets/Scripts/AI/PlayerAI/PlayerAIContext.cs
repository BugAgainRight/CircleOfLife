using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CircleOfLife.Battle;
using Milutools.AI;
using UnityEngine;

namespace CircleOfLife
{
    public class SkillCD
    {
        public float CD;
        public float Tick;
        public bool IsReady;

        //从隔壁复制过来的
        public int EffectCount;
        public float MoveSpeed;
        /// <summary>
        /// 各种生命时间，buff，子弹等等
        /// </summary>
        public float LifeTime;
        /// <summary>
        /// 特殊值，例如兽医站全屏小动物回复的血量
        /// </summary>
        public List<float> SpecialValues;
        public SkillCD(float cd)
        {
            CD = cd;
            Tick = 0;
            IsReady = false;
            MoveSpeed = 0f;
            LifeTime = 0f;
            EffectCount = 1;
            SpecialValues = new();
        }
        public SkillCD(float cd, int effectCount, float moveSpeed, float lifeTime, List<float> specialValues)
        {
            CD = cd;
            Tick = 0;
            IsReady = false;
            EffectCount = effectCount;
            MoveSpeed = moveSpeed;
            LifeTime = lifeTime;
            SpecialValues = specialValues;
        }
        public void Update()
        {
            if (Tick >= CD)
            {
                IsReady = true;
            }
            else
            {
                Tick += Time.deltaTime;
            }
        }
        public void Use()
        {
            Tick = 0;
            IsReady = false;
        }
    }
    public class PlayerAIContext : BehaviourContext
    {
        public Transform Player;
        public Transform Enemy;
        public Transform Friend;
        public LayerMask EnemyLayer;
        public LayerMask FriendLayer;
        public BattleStats PlayerBattaleStats;
        public float AnimalDistanceFromPlayer { get; private set; }
        public float EnemyDistanceFromPlayer { get; private set; }
        public float AnimalDistanceFromMouse { get; private set; }
        public float EnemyDistanceFromMouse { get; private set; }
        public float DiscoverEnemyDistance = 10;
        public float EnemyDistance
        {
            get
            {
                if (Enemy == null)
                {
                    return 114514;
                }
                return Vector2.Distance(Player.position, Enemy.position);
            }
        }
        public bool HasEnemyTarget = false;
        public float DiscoverFriendDistance = 10;
        public float FriendDistance
        {
            get
            {
                if (Friend == null)
                {
                    return 114514;
                }
                return Vector2.Distance(Player.position, Friend.position);
            }
        }
        public bool HasFriendTarget = false;
        #region Skill
        public SkillContext SkillContext;
        public Dictionary<PlayerSkillType, SkillCD> SkillDict = new Dictionary<PlayerSkillType, SkillCD>(
            new KeyValuePair<PlayerSkillType, SkillCD>[]
            {
                new KeyValuePair<PlayerSkillType, SkillCD>(PlayerSkillType.Melee, new SkillCD(1f)),
                new KeyValuePair<PlayerSkillType, SkillCD>(PlayerSkillType.Ranged, new SkillCD(1f)),
                new KeyValuePair<PlayerSkillType, SkillCD>(PlayerSkillType.Whack,  new SkillCD(1f)),
                new KeyValuePair<PlayerSkillType, SkillCD>(PlayerSkillType.Slash,  new SkillCD(1f)),
                new KeyValuePair<PlayerSkillType, SkillCD>(PlayerSkillType.FighterBraver,  new SkillCD(1f)),
                new KeyValuePair<PlayerSkillType, SkillCD>(PlayerSkillType.Heal,  new SkillCD(1f)),
                new KeyValuePair<PlayerSkillType, SkillCD>(PlayerSkillType.Thorn,  new SkillCD(1f)),
                new KeyValuePair<PlayerSkillType, SkillCD>(PlayerSkillType.Lurk,  new SkillCD(1f)),
                new KeyValuePair<PlayerSkillType, SkillCD>(PlayerSkillType.Encouragement,  new SkillCD(1f)),
            }
        );
        #endregion
        public override void UpdateContext()
        {
            FindNearestEntity(Enemy, EnemyLayer, DiscoverEnemyDistance, HasEnemyTarget, out Enemy, out HasEnemyTarget);
            FindNearestEntity(Friend, FriendLayer, DiscoverFriendDistance, HasFriendTarget, out Friend, out HasFriendTarget);
            CheckSkill();
        }

        private void FindNearestEntity(Transform entity, LayerMask layer, float distance, bool hasTarget, out Transform outEntity, out bool outHasTarget)
        {
            if (entity == null || entity.gameObject.activeInHierarchy == false ||
                            Vector2.Distance(Player.position, entity.transform.position) > distance) hasTarget = false;
            if (!hasTarget)
            {

                List<Collider2D> mid = Physics2D.OverlapCircleAll(Player.position, distance, layer).ToList();
                /*mid.RemoveAll(x =>
                {
                    var battleEntity = x.GetBattleStats();
                    if (battleEntity == null) return true;

                    if (battleEntity.BattleEntity.FactionType.Equals(FactionType.Enemy)) return true;
                    return false;
                });*/
                if (mid.Count > 0)
                {
                    hasTarget = true;
                    entity = mid[0].transform;
                }
                else { hasTarget = false; }
            }
            outEntity = entity;
            outHasTarget = hasTarget;

        }
        public void CheckSkill()
        {
            List<PlayerSkillType> keys = new List<PlayerSkillType>(SkillDict.Keys);
            foreach (var key in keys)
            {
                SkillDict[key].Update();
            }
        }
        private void SetSkillContext(PlayerSkillType playerSkillType)
        {
            SkillContext = new SkillContext(EnemyLayer, PlayerBattaleStats);
            SkillContext.Direction = (Enemy.position - Player.position).normalized;
            SkillContext.TriggerPos = Player.position;
            SkillContext.HitData = Enemy.GetComponent<Collider2D>().GetBattleStats();
            SkillContext.EffectCount = SkillDict[playerSkillType].EffectCount;
            SkillContext.MoveSpeed = SkillDict[playerSkillType].MoveSpeed;
            SkillContext.LifeTime = SkillDict[playerSkillType].LifeTime;
            SkillContext.SpecialValues = SkillDict[playerSkillType].SpecialValues;
        }
        public void UseMeleeAttack() => UseSkill(PlayerSkillType.Melee);
        public void UseRangeAttack() => UseSkill(PlayerSkillType.Ranged);
        public void UseSkill1() => UseSkill(PlayerSkillType.Whack);
        public void UseSkill2() => UseSkill(PlayerSkillType.Slash);
        public void UseSkill3() => UseSkill(PlayerSkillType.FighterBraver);
        public void UseSkill4() => UseSkill(PlayerSkillType.Heal);
        public void UseSkill5() => UseSkill(PlayerSkillType.Thorn);
        public void UseSkill6() => UseSkill(PlayerSkillType.Lurk);
        public void UseSkill7() => UseSkill(PlayerSkillType.Encouragement);


        public void UseSkill(PlayerSkillType playerSkillType)
        {
            if (SkillDict[playerSkillType].IsReady)
            {
                Debug.Log("Use " + playerSkillType.ToString());
                SetSkillContext(playerSkillType);
                SkillManagement.GetSkill(playerSkillType)(SkillContext);
                SkillDict[playerSkillType].Use();
            }
            else
            {
                Debug.Log(playerSkillType.ToString() + " is not ready");
            }
        }
    }
}
