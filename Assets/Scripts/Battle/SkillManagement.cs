using CircleOfLife.Battle;
using CircleOfLife.Buff;
using Milutools.Milutools.General;
using Milutools.Recycle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace CircleOfLife
{
    public static class SkillManagement 
    {

        private static Dictionary<EnumIdentifier, Action<SkillContext>> allSkills = new();

        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            
            ///注册所有技能方法
            foreach (var a in RuiRuiTool.AttributeMethodUtility.GetAllActionDatas<SkillAttribute>())
            {
                SkillAttribute b = (SkillAttribute)a.attribute;
                allSkills.Add(b.Key, (Action<SkillContext>)a.method.CreateDelegate(typeof(Action<SkillContext>)));
            } 
           
        }

        public static Action<SkillContext> GetSkill<T>(T type)where T : Enum
        {

            if (allSkills.TryGetValue(EnumIdentifier.Wrap(type), out var action)) return action;
            else
            {
                Debug.LogError($"{typeof(T)}枚举类型 {type}没有实现技能函数！");
                return null;
            }


        }



        #region PlayerSkill
        [Skill(PlayerSkillType.Melee)]
        private static void PlayerSkill_Melee(SkillContext context)
        {
            float angle = Mathf.Atan2(context.Direction.y, context.Direction.x);
            var collection = RecyclePool.RequestWithCollection(SharedPrefab.Melee);
            collection.GameObject.SetActive(true);
            collection.GameObject.transform.position = context.TriggerPos;
            collection.GameObject.transform.localEulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);

            var list = collection.GetMainComponent<BattleRange>().GetAllEnemyInRange(
                context.PhysicsLayer, context.AttackerData.BattleEntity.FactionType);
            if (list.Count > 0)
            {
                //添加能量
                context.AttackerData.Transform.GetComponentInChildren<PlayerSkills>().Energy += PlayerSkills.RECOVER_ON_ATTACK;
            }
            foreach (var item in list)
            {
                BattleStats mid = item.GetBattleStats();
                BulletManagement.GetBulletTrigger(BulletTriggerType.Normal)(
                    new BattleContext(context.PhysicsLayer, context.AttackerData, mid.BattleEntity.Stats));
            }

        }


        [Skill(PlayerSkillType.Whack)]
        private static void PlayerSkill_1(SkillContext context)
        {
            float angle = Mathf.Atan2(context.Direction.y, context.Direction.x);
            var collection = RecyclePool.RequestWithCollection(PlayerSkillType.Whack);
            collection.GameObject.SetActive(true);
            collection.GameObject.transform.position = context.TriggerPos;
            collection.GameObject.transform.localEulerAngles = new Vector3(0, 0, angle);

            var list = collection.GetMainComponent<BattleRange>().GetAllEnemyInRange(
                context.PhysicsLayer, context.AttackerData.BattleEntity.FactionType);

            foreach (var item in list)
            {
                BattleStats mid = item.GetBattleStats();
                BulletManagement.GetBulletTrigger(BulletTriggerType.Normal)(
                    new BattleContext(context.PhysicsLayer, context.AttackerData, mid.BattleEntity.Stats) 
                    { SkillRate=3});
                
                mid.BattleEntity.Stats.ApplyBuff(BuffUtils.ToBuff(UniversalBuff.Dizzy, 3f));
                mid.BattleEntity.Stats.ApplyBuff(BuffUtils.ToBuff(UniversalBuff.Blood, 3f));

            }
        }

        [Skill(PlayerSkillType.Slash)]
        private static void PlayerSkill_2(SkillContext context)
        {
            float angle = Mathf.Atan2(context.Direction.y, context.Direction.x);
            var collection = RecyclePool.RequestWithCollection(PlayerSkillType.Slash);
            collection.GameObject.SetActive(true);
            collection.GameObject.transform.position = context.TriggerPos;
            collection.GameObject.transform.localEulerAngles = new Vector3(0, 0, angle);

            var list = collection.GetMainComponent<BattleRange>().GetAllEnemyInRange(
                context.PhysicsLayer, context.AttackerData.BattleEntity.FactionType);
            Debug.Log(list.Count);
            foreach (var item in list)
            {
                BattleStats mid = item.GetBattleStats();
                BulletManagement.GetBulletTrigger(BulletTriggerType.Normal)(
                    new BattleContext(context.PhysicsLayer, context.AttackerData, mid.BattleEntity.Stats));

                mid.BattleEntity.Stats.ApplyBuff(BuffUtils.ToBuff(UniversalBuff.SlowDown, 3f));
                

            }
        }



        /// <summary>
        /// 愈战愈勇
        /// </summary>
        /// <param name="context"></param>
        [Skill(PlayerSkillType.FighterBraver)]
        private static void PlayerSkill_3(SkillContext context)
        {
            context.AttackerData.ApplyBuff(BuffUtils.ToBuff(PlayerSkill_3_Buff, 10));
        }
        private static void PlayerSkill_3_Buff(BattleStats stats, BuffContext buff)
        {
            stats.Current.LifeStealRate += BuffConsts.LIFE_STEAL_UNIT;

        }


        /// <summary>
        /// 疗伤
        /// </summary>
        /// <param name="context"></param>
        [Skill(PlayerSkillType.Heal)]
        private static void PlayerSkill_4(SkillContext context)
        {
            context.AttackerData.ApplyBuff(BuffUtils.ToBuff(PlayerSkill_4_Buff, 10));
        }

        private static void PlayerSkill_4_Buff(BattleStats stats, BuffContext buff)
        {
            stats.Current.Armor += 2*BuffConsts.ARMOR_UNIT;
            if (buff.TickedTime >= 1f)
            {
                DamageManagement.BuffDamage(stats, -BuffConsts.HP_UNIT);
                buff.ResetTickedTime();
            }
        }


        /// <summary>
        /// 荆棘
        /// </summary>
        /// <param name="context"></param>
        [Skill(PlayerSkillType.Thorn)]
        private static void PlayerSkill_5(SkillContext context)
        {
            context.AttackerData.ApplyBuff(BuffUtils.ToBuff(PlayerSkill_5_Buff, 10));
        }
        private static void PlayerSkill_5_Buff(BattleStats stats, BuffContext buff)
        {
            stats.Current.ReboundDamageRate += BuffConsts.REBOUND_DAMAGE_RATE_UNIT;

        }

        /// <summary>
        /// 潜伏
        /// </summary>
        /// <param name="context"></param>
        [Skill(PlayerSkillType.Lurk)]
        private static void PlayerSkill_6(SkillContext context)
        {
            context.AttackerData.ApplyBuff(BuffUtils.ToBuff(PlayerSkill_6_Buff, 10));
        }
        private static void PlayerSkill_6_Buff(BattleStats stats, BuffContext buff)
        {
            stats.Current.Velocity += BuffConsts.SPEED_UNIT * 2;
            stats.Current.EvasionRate += BuffConsts.EVASION_UNIT * 2;
            stats.Current.CriticalChance += 1;

        }
        /// <summary>
        /// 鼓舞
        /// </summary>
        /// <param name="context"></param>
        [Skill(PlayerSkillType.Encouragement)]
        private static void PlayerSkill_7(SkillContext context)
        {
            var builds = BuffManager.GetAllStats().Where(x => x.GameObject.activeInHierarchy && x.GameObject.CompareTag("Building"));
            var friends = BuffManager.GetAllStats().Where(x => x.GameObject.activeInHierarchy && x.GameObject.CompareTag("Friend"));

            foreach (var build in builds)
            {
                DamageManagement.BuffDamage(build,-100);
            }

            foreach (var friend in friends)
            {
                DamageManagement.BuffDamage(friend, -50);
            }

        }
        ///// <summary>
        ///// 近战普攻
        ///// </summary>
        ///// <param name="context"></param>
        //[Skill(PlayerSkillType.Melee)]
        //private static void PlayerSkill_8(SkillContext context)
        //{

        //}
        ///// <summary>
        ///// 远程普攻
        ///// </summary>
        ///// <param name="context"></param>
        //[Skill(PlayerSkillType.Ranged)]
        //private static void PlayerSkill_9(SkillContext context)
        //{

        //}

        #endregion




        #region EnemySkill

        [Skill(EnemySkillType.test1)]
        private static void EnemySkill_1(SkillContext context)
        {

      
            var collection = RecyclePool.RequestWithCollection(EnemySkillType.test1);
            collection.GameObject.SetActive(true);
            collection.GameObject.transform.position = context.TriggerPos;

            collection.GetComponent<BulletTrigger>().PassData(
                new BattleContext(context.PhysicsLayer, context.AttackerData, context.HitData)
                {
                    BoomRadius = 5,

                });

            collection.GetComponent<BulletMove>().PassData(new BulletMoveContext()
            {
                LaunchTime = Time.time,
                Direction = context.Direction,
                StartPos = collection.GameObject.transform.position,
                TargetPos = context.TargetPos,
                Transform = collection.GameObject.transform,
                Speed = context.MoveSpeed,
                MaxHeight = UnityEngine.Random.Range(4, 6f)
            });
        }


        #endregion




        #region AnimalSkill


        #endregion



        #region BuildSkill

        [Skill(BuildSkillType.TreatmentStationNormal)]
        private static void BuildSkill_0(SkillContext context)
        {
            var list = context.AttackerData.Transform.GetComponent<BuildBase>().BattleRange.GetAllFriendInRange(
                context.PhysicsLayer, context.AttackerData.BattleEntity.FactionType);
            int count = 0;
            list.Sort((y,x) => (x.GetBattleStats().Max.Hp - x.GetBattleStats().Current.Hp).CompareTo(y.GetBattleStats().Max.Hp - y.GetBattleStats().Current.Hp));
            foreach (var coll in list)
            {
                if (count >= context.EffectCount) break;
                var stats = coll.GetBattleStats();
                if (stats == context.AttackerData) continue; // 治疗站能治疗自己感觉有点超模
                DamageManagement.BuffDamage(stats, -context.SpecialValues[1]);
                //Debug.Log(-context.SpecialValues[1]);
                RecyclePool.Request(BuildEffects.Recovery, (c) =>
                {
                    c.Transform.position = coll.transform.position;
                    c.GameObject.SetActive(true);
                });
                count++;
            }

        }

        [Skill(BuildSkillType.TreatmentStation1)]
        private static void BuildSkill_0_1(SkillContext context)
        {
            BuildSkill_0(context);
            var list = GameObject.FindGameObjectsWithTag("小动物");
            foreach (var item in list)
            {
                if (item.TryGetComponent(out IBattleEntity battleEntity))
                {
                    if (battleEntity.FactionType == FactionType.Friend)
                    {
                        DamageManagement.Damage(
                            new BattleContext(context.PhysicsLayer, context.AttackerData, battleEntity.Stats));
                        RecyclePool.Request(BuildEffects.Recovery, (c) =>
                        {
                            c.Transform.position = item.transform.position;
                            c.GameObject.SetActive(true);
                        });
                    }
                }
            }

        }


        [Skill(BuildSkillType.TreatmentStation2)]
        private static void BuildSkill_0_2(SkillContext context)
        {
            BuildSkill_0(context);
        }
        #region 信号发射器
       

        private static void SignalTransmitterFunc(BuildSkillType buildSkillType, SkillContext context)
        {
            float range = context.AttackerData.Current.EffectRange;
            Vector2 instantiatePos;
            for(int i=0;i<10 ;i++ )
            {
                instantiatePos = UnityEngine.Random.insideUnitCircle * range;
                if (Physics2D.Raycast(instantiatePos, Vector2.zero).collider == null)
                {
                    var collection = RecyclePool.RequestWithCollection(buildSkillType, context.AttackerData.Transform);
                    collection.GameObject.SetActive(true);
                    var passData = context.AttackerData.Max;
                    passData.AttackInterval = 1;

                    var buildContext = collection.GetMainComponent<BuildFriendContext>();


                    buildContext.StandPos = context.TriggerPos + instantiatePos;
                    collection.GameObject.transform.position = buildContext.StandPos;

                    RecyclePool.Request(BuildEffects.NewFriend, (c) =>
                    {
                        c.Transform.position = buildContext.StandPos;
                        c.GameObject.SetActive(true);
                    });
                    break;
                }
            }

            
        }
        [Skill(BuildSkillType.SignalTransmitterNormal)]
        private static void BuildSkill_1(SkillContext context)
        {
            SignalTransmitterFunc(BuildSkillType.SignalTransmitterNormal, context);
        }
        [Skill(BuildSkillType.SignalTransmitter1)]
        private static void BuildSkill_1_1(SkillContext context)
        {
            SignalTransmitterFunc(BuildSkillType.SignalTransmitter1, context);
        }
        [Skill(BuildSkillType.SignalTransmitter2)]
        private static void BuildSkill_1_2(SkillContext context)
        {
            SignalTransmitterFunc(BuildSkillType.SignalTransmitter2, context);
        }
        [Skill(BuildSkillType.SignalTransmitter3)]
        private static void BuildSkill_1_3(SkillContext context)
        {
            SignalTransmitterFunc(BuildSkillType.SignalTransmitter3, context);
        }

        #endregion



        [Skill(BuildSkillType.TestBuildFriendFire)]
        private static void BuildSkill_2(SkillContext context)
        {
            BulletManagement.GetBulletTrigger(BulletTriggerType.Normal)(
                new BattleContext(context.PhysicsLayer, context.AttackerData, context.HitData));


        }

        #endregion


        #region SharedSkill

        /// <summary>
        /// 共用近战普攻
        /// </summary>
        [Skill(EnemyStat.EnemyA)]
        [Skill(EnemyStat.EnemyC)]
        [Skill(EnemyStat.EnemyD)]
        [Skill(AnimalSkillType.TibetanMastiffMelee)]
        private static void SharedSkill_Melee(SkillContext context)
        {
            float angle = Mathf.Atan2(context.Direction.y, context.Direction.x);
            var collection = RecyclePool.RequestWithCollection(SharedPrefab.Melee);
            collection.GameObject.SetActive(true);
            collection.GameObject.transform.position = context.TriggerPos;
            collection.GameObject.transform.localEulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);

            var list = collection.GetMainComponent<BattleRange>().GetAllEnemyInRange(
                context.PhysicsLayer, context.AttackerData.BattleEntity.FactionType);
            
            foreach (var item in list)
            {
                BattleStats mid = item.GetBattleStats();
                BulletManagement.GetBulletTrigger(BulletTriggerType.Normal)(
                    new BattleContext(context.PhysicsLayer, context.AttackerData, mid.BattleEntity.Stats));
            }

        }

        /// <summary>
        /// 远程攻击发射模板
        /// </summary>
        private static void RangedAttackTemplate(RecycleCollection collection, SkillContext context)
        {
            collection.GameObject.SetActive(true);
            
            collection.GameObject.transform.position = context.TriggerPos;
            
            if (context.FireTransform)
            {
                collection.GameObject.transform.position += context.FireTransform.position - collection.GameObject.transform.position;
            }

            collection.GetComponent<BulletMove>().SetTarget(context.HitData.Transform);
            collection.GetComponent<BulletTrigger>().PassData(new BattleContext(context.PhysicsLayer,context.AttackerData,null));

        }

        /// <summary>
        /// 共用远程普攻
        /// </summary>
        /// <param name="context"></param>
        [Skill(PlayerSkillType.Ranged)]
        [Skill(EnemyStat.EnemyBSignal)]
        [Skill(BuildSkillType.SignalTransmitterNormalFriend)]
        private static void SharedSkill_Ranged(SkillContext context)
        {
            var collection = RecyclePool.RequestWithCollection(SharedPrefab.Ranged);
            RangedAttackTemplate(collection, context);
           

        }
        /// <summary>
        /// 共用远程群体
        /// </summary>
        /// <param name="context"></param>
        [Skill(EnemyStat.EnemyBGroup)]
        [Skill(BuildSkillType.SignalTransmitter1Friend)]
        private static void SharedSkill_RangedGroup(SkillContext context)
        {
            var collection = RecyclePool.RequestWithCollection(SharedPrefab.RangedGroup);
            RangedAttackTemplate(collection, context);

        }
        /// <summary>
        /// 远程超长版
        /// </summary>
        /// <param name="context"></param>
        [Skill(BuildSkillType.SignalTransmitter2Friend)]
        private static void SharedSkill_RangedLongest(SkillContext context)
        {
            var collection = RecyclePool.RequestWithCollection(SharedPrefab.RangedLongest);
            RangedAttackTemplate(collection, context);

        }
        /// <summary>
        /// 远程快速攻击
        /// </summary>
        /// <param name="context"></param>
        [Skill(BuildSkillType.SignalTransmitter3Friend)]
        private static void SharedSkill_RangedFast(SkillContext context)
        {
            var collection = RecyclePool.RequestWithCollection(SharedPrefab.RangedFast);
            RangedAttackTemplate(collection, context);
        }

        #endregion


    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SkillAttribute : Attribute
    {
        public Enum BuffType;
        public EnumIdentifier Key;
        public SkillAttribute(object type)
        {
            BuffType = (Enum)type;
            Key = EnumIdentifier.WrapReflection(BuffType);
        }

    }
}
