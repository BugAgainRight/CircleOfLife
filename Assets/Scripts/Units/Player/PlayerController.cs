using CircleOfLife.Battle;
using CircleOfLife.Buff;
using CircleOfLife.Key;
using Milease.Enums;
using Milease.Utils;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Rendering;

namespace CircleOfLife.Units
{
    [RequireComponent(typeof(Rigidbody2D))]

    [RequireComponent(typeof(CircleCollider2D))]
    public class PlayerController : MonoBehaviour, IBattleEntity
    {
        public static PlayerController Instance;

        public SkeletonAnimation SkeletonAnimation;
        
        //TODO 参考敌人的AI实现操作逻辑
        // Start is called before the first frame update
        private Vector2 direction, lstDirection;
        private new Rigidbody2D rigidbody2D;
        
        public BattleStats.Stats Stat;
        public Volume RunningProcess;
        
        public BattleStats Stats { get; set; }

        public FactionType FactionType { get; } = FactionType.Friend;
        private PlayerAIContext playerAIContext;

        private bool running = false;
        private bool lstRunning = false;
        
        void Awake()
        {
            Instance = this;
            
            Stats = Stat.Build(gameObject, test =>
            {
                if (test.HitData.Current.Hp <= 0f)
                {
                    Destroy(gameObject);
                }
            });
            Stats.Max.Velocity = 10;
            Stats.Reset();
            rigidbody2D = GetComponent<Rigidbody2D>();
            playerAIContext = GetComponent<PlayerAIContext>();

        }
        void Start()
        {
            if (rigidbody2D.gravityScale != 0)
            {
                rigidbody2D.gravityScale = 0;
                rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            }

        }

        // Update is called once per frame
        void Update()
        {
            PlayerMove();
            KeyBoardMonitor();
            playerAIContext.PlayerBattaleStats = Stats;
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
            if (direction.x < 0)
            {
                this.transform.localScale = new Vector3(1, 1, 1) * 0.4f;
            }
            else if (direction.x > 0)
            {
                this.transform.localScale = new Vector3(-1, 1, 1) * 0.4f;
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
            if (KeyEnum.Attack.IsKeyDown()) Attack();
            if (KeyEnum.Fire.IsKeyDown()) Fire();
            if (KeyEnum.Skill1.IsKeyDown()) Skilll();
            if (KeyEnum.Skill2.IsKeyDown()) Skill2();
            if (KeyEnum.Skill3.IsKeyDown()) Skill3();
            if (KeyEnum.Skill4.IsKeyDown()) Skill4();
            if (KeyEnum.Skill5.IsKeyDown()) Skill5();
            if (KeyEnum.Skill6.IsKeyDown()) Skill6();
            if (KeyEnum.Skill7.IsKeyDown()) Skill7();
        }


        #endregion



        #region Attack

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
        #endregion
    }
}
