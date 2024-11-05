using CircleOfLife.Battle;
using Milutools.Recycle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CircleOfLife
{
    public enum BulletTriggerType
    {
        Normal, Boom
    }

    public enum BulletMoveType
    {
        Straight, Curve
    }

    public static class BulletManagement
    {
        private static Dictionary<BulletTriggerType, Action<BattleContext>> allBulletTriggers = new();

        private static Dictionary<BulletMoveType, Action<BulletMoveContext>> allBulletMoves = new();

        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            ///注册所有子弹触发方法
            foreach (var a in RuiRuiTool.AttributeMethodUtility.GetAllActionDatas<BulletTriggerAttribute>())
            {
                BulletTriggerAttribute b = (BulletTriggerAttribute)a.attribute;
                allBulletTriggers.Add((b.BuffType), (Action<BattleContext>)a.method.CreateDelegate(typeof(Action<BattleContext>)));
            }
            ///注册所有子弹移动方法
            foreach (var a in RuiRuiTool.AttributeMethodUtility.GetAllActionDatas<BulletMoveAttribute>())
            {
                BulletMoveAttribute b = (BulletMoveAttribute)a.attribute;
                allBulletMoves.Add((b.MoveType), (Action<BulletMoveContext>)a.method.CreateDelegate(typeof(Action<BulletMoveContext>)));
            }

        }

        /// <summary>
        /// 会触发Damage时 其参数 battleContext 必须填入数据：AttackerData HitData（受击者）HitTran
        /// </summary>
        /// <param name="battleContext"></param>
        public static Action<BattleContext> GetBulletTrigger(BulletTriggerType type)
        {
            if (allBulletTriggers.ContainsKey(type)) return allBulletTriggers[type];
            else
            {
                Debug.LogError($"{type} 对应的触发函数未实现！");
                return null;
            }
        }


        public static Action<BulletMoveContext> GetBulletMove(BulletMoveType type)
        {
            if (allBulletMoves.ContainsKey(type)) return allBulletMoves[type];
            else
            {
                Debug.LogError($"{type} 对应的移动函数未实现！");
                return null;
            }
        }




        #region Trigger


        [BulletTrigger(BulletTriggerType.Normal)]
        public static void NormalTrigger(BattleContext context)
        {
            DamageManagement.Instance.Damage(context);
        }



        [BulletTrigger(BulletTriggerType.Boom)]
        public static void BoomTrigger(BattleContext context)
        {
            var colls = Physics2D.OverlapCircleAll(
                context.AttackerData.Transform.position, context.BoomRadius, context.DamageableLayer);
            var idamages = colls.ToList().Where(x => x.GetComponent<IBattleEntity>() != null);
            foreach (var idamage in idamages)
            {
                BattleContext midContext = context;
                midContext.HitData = idamage.GetComponent<IBattleEntity>().Stats;

                DamageManagement.Instance.Damage(midContext);
            }

        }

        #endregion



        #region Move
        [BulletMove(BulletMoveType.Straight)]
        public static void StraightMove(BulletMoveContext context)
        {
            if (context.LaunchTime + context.LifeTime < Time.time)
            {
                RecyclePool.ReturnToPool(context.Transform.gameObject);
                return;
            }

            context.Transform.position = Vector2.MoveTowards(
                context.Transform.position,
                (Vector2)context.Transform.position + context.Direction * context.Speed,
                context.Speed * Time.fixedDeltaTime
                );
        }

        [BulletMove(BulletMoveType.Curve)]
        public static void CurveMove(BulletMoveContext context)
        {
            ///更新曲线参数
            if (Vector2.Distance(context.Transform.position, context.StartPos) <= 0.005f)
            {
                float height = context.MaxHeight;

                Vector2 direction;
                direction = (context.TargetPos - context.StartPos).normalized;
                ///保存数据
                context.XDirection = direction;

                ///垂直direction的向量，用于坐标系转化使用
                Vector2 perpendicularDir;
                perpendicularDir = new Vector2(-direction.y, direction.x);
                ///保存数据
                context.YDirection = perpendicularDir;

                ///距离
                float distance = Vector2.Distance(context.StartPos, context.TargetPos);

                ///计划移动时间
                float moveTime;
                moveTime = distance / context.Speed;

                ///新的重力加速度
                context.Gravity = (2 * height) / (moveTime * moveTime);

                ///数值竖直方向初始速度
                float verticalSpeed = (2 * height) / moveTime / 2;

                context.SpeedVector = direction * context.Speed + perpendicularDir * verticalSpeed;

            }
            ///到达目标位置
            if (Vector2.Distance(context.Transform.position, context.TargetPos) <= 0.005f)
            {
                return;
            }


            context.Transform.position += (Vector3)context.SpeedVector * Time.fixedDeltaTime;
            ///更新速度
            context.SpeedVector += context.YDirection * -context.Gravity * Time.fixedDeltaTime;


        }

        #endregion



    }


    [AttributeUsage(AttributeTargets.Method)]
    public class BulletTriggerAttribute : Attribute
    {
        public BulletTriggerType BuffType;
        public BulletTriggerAttribute(BulletTriggerType type)
        {
            BuffType = type;
        }

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class BulletMoveAttribute : Attribute
    {
        public BulletMoveType MoveType;
        public BulletMoveAttribute(BulletMoveType type)
        {
            MoveType = type;
        }

    }

}


