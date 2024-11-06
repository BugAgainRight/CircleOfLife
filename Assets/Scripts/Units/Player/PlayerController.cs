using CircleOfLife.Battle;
using CircleOfLife.Buff;
using CircleOfLife.Key;
using UnityEngine;

namespace CircleOfLife.Units
{
    [RequireComponent(typeof(Rigidbody2D))]

    [RequireComponent(typeof(CircleCollider2D))]
    public class PlayerController : MonoBehaviour, IBattleEntity
    {
        //TODO 参考敌人的AI实现操作逻辑
        // Start is called before the first frame update
        private Vector2 direction;
        private CircleCollider2D capsuleCollider2D;
        private new Rigidbody2D rigidbody2D;
        private GameObject firePoint;
        public float PlayerSpeed = 10;
        private Vector2 Direction;
        private GameObject bullet;

        public BattleStats.Stats Stat;

        public BattleStats Stats { get; set; }

        public FactionType FactionType { get; } = FactionType.Friend;

        void Awake()
        {

            Stats = Stat.Build(gameObject, test =>
            {
                if (test.HitData.Current.Hp <= 0f)
                {
                    Destroy(gameObject);
                }
            });
            Stats.Max.Velocity = 10;
            Stats.Reset();
            capsuleCollider2D = GetComponent<CircleCollider2D>();
            rigidbody2D = GetComponent<Rigidbody2D>();
            //bullet = Resources.Load<GameObject>(UnitIDManager.BulletDict["BaseBullet"]);
            //bullet.tag = "PlayerBullet";
            CreateFirePoint();
        }
        void Start()
        {
            PlayerSpeed = Stats.Current.Velocity;
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
        }

        void FixedUpdate()
        {
            transform.position = new Vector2(transform.position.x + direction.x * PlayerSpeed * Time.deltaTime, transform.position.y + direction.y * PlayerSpeed * Time.deltaTime);
        }

        public void CreateFirePoint()
        {
            if (firePoint == null)
            {
                firePoint = Instantiate(new GameObject("FirePoint"), this.transform.position, Quaternion.identity, this.transform);
            }
        }
        #region KeyBoardMonitor
        public void PlayerMove()
        {
            direction = Vector2.zero;
            if (KeyEnum.Up.IsPressing()) { direction += Vector2.up; }
            if (KeyEnum.Down.IsPressing()) { direction += Vector2.down; }
            if (KeyEnum.Left.IsPressing()) { direction += Vector2.left; }
            if (KeyEnum.Right.IsPressing()) { direction += Vector2.right; }
            Direction = new Vector2(direction.x, direction.y);
            if (direction.x < 0)
            {
                this.transform.localScale = new Vector3(1, 1, 1) * 0.4f;
            }
            else if (direction.x > 0)
            {
                this.transform.localScale = new Vector3(-1, 1, 1) * 0.4f;
            }
        }

        public void KeyBoardMonitor()
        {
            if (KeyEnum.Interact.IsKeyDown()) OpenSomething();
            if (KeyEnum.Attack.IsKeyDown()) Attack();
            if (KeyEnum.Fire.IsKeyDown()) Fire();
            if (KeyEnum.Skill1.IsKeyDown()) Skill();
        }


        #endregion

        public void OpenSomething()
        {
            Debug.Log("Open Something");
            FriendlyNPCInteract(this.gameObject);
        }

        public void FriendlyNPCInteract(GameObject target)
        {
            //显示操作提示
            //intetactableIconList.Add(Instantiate(icon, target.transform.position, Quaternion.identity, target.transform));
        }


        #region Attack
        public void Hurt(IAttack attacker)
        {
            Debug.Log("Hurt!");
        }

        public void Attack()
        {
            Debug.Log("Attack!");
        }

        public void Fire()
        {
            /*if (firePoint == null)
            {
                Debug.Log(this.name + ":can't find firePoint");
            }
            else
            {
                Instantiate(bullet, firePoint.transform.position, Quaternion.identity);
            }*/
            Debug.Log("Fire!");
        }

        //触发技能
        public void Skill()
        {
            Debug.Log("Click Skill!");
        }
        #endregion
    }
}
