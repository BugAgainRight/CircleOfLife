using System;
using CircleOfLife.Battle;
using CircleOfLife.Buff;
using CircleOfLife.Key;
using CircleOfLife.Level;
using CircleOfLife.NPCInteract;
using Milease.Enums;
using Milease.Utils;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Rendering;

namespace CircleOfLife.Units
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class PlayerController : MonoBehaviour, IBattleEntity
    {
        public static PlayerController Instance;

        public SkeletonAnimation SkeletonAnimation;

        private Vector2 direction, lstDirection;
        private new Rigidbody2D rigidbody2D;

        public BattleStats.Stats Stat;
        public Volume RunningProcess;
        public Transform SkillOffset;

        public BattleStats Stats { get; set; }

        public FactionType FactionType => FactionType.Friend;
        //private PlayerAIContext playerAIContext;

        private bool running = false;
        private bool lstRunning = false;

        [HideInInspector]
        public Action<BattleContext> HurtAction;

        void Awake()
        {
            Instance = this;

            Stats = Stat.Build(gameObject, context =>
            {
                HurtAction?.Invoke(context);
                if (context.HitData.Current.Hp <= 0f)
                {
                    gameObject.SetActive(false);
                    // 游戏失败
                    LevelManager.Instance.Fail("玩家倒下了");
                }
            }, true);
            Stats.Max.Velocity = 10;
            Stats.Reset();
            rigidbody2D = GetComponent<Rigidbody2D>();
            //playerAIContext = GetComponent<PlayerAIContext>();

        }

        void Start()
        {
            if (rigidbody2D.gravityScale != 0)
            {
                rigidbody2D.gravityScale = 0;
                rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }

        void Update()
        {
            PlayerMove();
            KeyBoardMonitor();
            //playerAIContext.PlayerBattaleStats = Stats;
        }

        public void ResetState()
        {
            running = false;
            lstDirection = Vector2.zero;
            RunningProcess.MileaseTo(nameof(RunningProcess.weight), 0f, 0.5f,
                0f, EaseFunction.Circ, EaseType.Out).Play();
            SkeletonAnimation.state.SetAnimation(0, "idel", true);
        }

        void FixedUpdate()
        {
            transform.position += (Vector3)direction * (Stats.Current.Velocity * Time.fixedDeltaTime);
        }

        #region KeyBoardMonitor
        public void PlayerMove()
        {
            direction = Vector2.zero;
            if (KeyEnum.Up.IsPressing()) { direction += Vector2.up; }
            if (KeyEnum.Down.IsPressing()) { direction += Vector2.down; }
            if (KeyEnum.Left.IsPressing()) { direction += Vector2.left; }
            if (KeyEnum.Right.IsPressing()) { direction += Vector2.right; }
            direction *= (running ? 3f : 1f);

            if (!direction.Equals(Vector2.zero))
            {
                transform.localScale = new Vector3(direction.x < 0 ? 1 : -1, 1, 1) * 0.4f;
            }

            if (lstDirection != direction)
            {
                lstDirection = direction;
                SkeletonAnimation.state.SetAnimation(0,
                    (direction.Equals(Vector2.zero)) ? "idel" : (running ? "run" : "walk"), true);
            }

            if (lstRunning != running)
            {
                lstRunning = running;
                RunningProcess.MileaseTo(nameof(RunningProcess.weight), running ? 1f : 0f, 0.5f,
                    0f, EaseFunction.Circ, EaseType.Out).Play();
            }
        }

        public void KeyBoardMonitor()
        {
            //if (KeyEnum.Interact.IsKeyDown()) OpenSomething();
            running = KeyEnum.Running.IsPressing();
            if (KeyEnum.Interact.IsKeyDown())
            {
                if (InteractorManager.GetInteractableTarget())
                {
                    Debug.Log("Interact:" + InteractorManager.GetInteractableTarget().name);
                }
                else
                {
                    Debug.Log("No InteractableTarget");
                }
            }
            /**if (KeyEnum.Attack.IsKeyDown()) Attack();
            if (KeyEnum.Fire.IsKeyDown()) Fire();
            if (KeyEnum.Skill1.IsKeyDown()) Skilll();
            if (KeyEnum.Skill2.IsKeyDown()) Skill2();
            if (KeyEnum.Skill3.IsKeyDown()) Skill3();
            if (KeyEnum.Skill4.IsKeyDown()) Skill4();
            if (KeyEnum.Skill5.IsKeyDown()) Skill5();
            if (KeyEnum.Skill6.IsKeyDown()) Skill6();
            if (KeyEnum.Skill7.IsKeyDown()) Skill7();**/
        }


        #endregion



        /**#region Attack

        public void Attack()
        {
            playerAIContext.UseMeleeAttack();
        }
        public void Fire()
        {
            playerAIContext.UseRangeAttack();
        }
        //触发技能
        public void Skilll()
        {
            playerAIContext.UseSkill1();
        }

        public void Skill2()
        {
            playerAIContext.UseSkill2();
        }
        public void Skill3()
        {
            playerAIContext.UseSkill3();
        }
        public void Skill4()
        {
            playerAIContext.UseSkill(PlayerSkillType.FighterBraver);
        }

        public void Skill5()
        {
            playerAIContext.UseSkill(PlayerSkillType.Thorn);
        }
        public void Skill6()
        {
            playerAIContext.UseSkill(PlayerSkillType.Lurk);
        }
        public void Skill7()
        {
            playerAIContext.UseSkill(PlayerSkillType.Encouragement);
        }
        #endregion**/
    }
}
