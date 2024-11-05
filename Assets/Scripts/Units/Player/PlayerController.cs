using CircleOfLife.Key;
using UnityEngine;

namespace CircleOfLife.Units
{
    [RequireComponent(typeof(Rigidbody2D))]

    [RequireComponent(typeof(CircleCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        // Start is called before the first frame update
        private Vector2 direction;
        private CircleCollider2D capsuleCollider2D;
        private new Rigidbody2D rigidbody2D;
        private GameObject firePoint;
        public float PlayerSpeed = 10;
        private Vector2 Direction;
        private GameObject bullet;

        void Awake()
        {
            capsuleCollider2D = GetComponent<CircleCollider2D>();
            rigidbody2D = GetComponent<Rigidbody2D>();
            //bullet = Resources.Load<GameObject>(UnitIDManager.BulletDict["BaseBullet"]);
            //bullet.tag = "PlayerBullet";
            CreateFirePoint();
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
            if (Input.GetKey(KeyboardSet.KeyboardDict[KeyEnum.Up])) { direction += Vector2.up; }
            if (Input.GetKey(KeyboardSet.KeyboardDict[KeyEnum.Down])) { direction += Vector2.down; }
            if (Input.GetKey(KeyboardSet.KeyboardDict[KeyEnum.Left])) { direction += Vector2.left; }
            if (Input.GetKey(KeyboardSet.KeyboardDict[KeyEnum.Right])) { direction += Vector2.right; }
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
            if (Input.GetKeyDown(KeyboardSet.KeyboardDict[KeyEnum.Interact])) OpenSomething();
            if (Input.GetKeyDown(KeyboardSet.KeyboardDict[KeyEnum.Attack])) Attack();
            if (Input.GetKeyDown(KeyboardSet.KeyboardDict[KeyEnum.Fire])) Fire();
            if (Input.GetKeyDown(KeyboardSet.KeyboardDict[KeyEnum.Skill1])) Skill();
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
