using System.Collections;
using System.Collections.Generic;
using CircleOfLife.NPCInteract;
using UnityEngine;

namespace CircleOfLife.Units
{
    public class BaseNPC : MonoBehaviour
    {
        //名字、id、速度、最大生命值、攻击力、攻击范围、攻击间隔、能量、护甲、暴击率、暴击伤害、闪避率、当前生命值、描述
        private string unitName;
        private string id;
        private float velocity;
        private float maxHp;
        private float atk;
        private float attackRange;
        private float attackInterval;
        private float energy;
        private float armor;
        private float criticalChance;
        private float criticalStrikeDamage;
        private float evasionRate;
        private float currentHp;
        private string description;
        private GameObject finallyTarget;
        private GameObject currentTarget;
        private GameObject collider2DChecker;
        public string UnitName
        {
            get { return unitName; }
            set { unitName = value; }
        }
        public string ID
        {
            get { return id; }
        }

        public float Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public float MaxHp
        {
            get { return maxHp; }
            set { maxHp = value; }
        }

        public float Atk
        {
            get { return atk; }
            set { atk = value; }
        }

        public float AttackRange
        {
            get { return attackRange; }
            set { attackRange = value; }
        }

        public float AttackInterval
        {
            get { return attackInterval; }
            set { attackInterval = value; }
        }

        public float Energy
        {
            get { return energy; }
            set { energy = value; }
        }
        public float CurrentHp
        {
            get { return currentHp; }
            set { currentHp = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public GameObject FinallyTarget
        {
            get { return finallyTarget; }
            set { finallyTarget = value; }
        }
        public GameObject CurrentTarget
        {
            get { return currentTarget; }
            set { currentTarget = value; }
        }

        public GameObject Collider2DChecker
        {
            get { return collider2DChecker; }
            set { collider2DChecker = value; }
        }
        void Awake()
        {

        }

        protected void Start()
        {
            id = UnitIDManager.CreatID(this);
            //CreateCollider2DChecker();
        }

        void OnDestroy()
        {
            UnitIDManager.DestroyID(this.id);
        }

        #region Interact
        /*private void CreateCollider2DChecker()
        {
            collider2DChecker = new GameObject("Collider2DChecker");
            collider2DChecker.transform.SetParent(this.transform);
            collider2DChecker.transform.localPosition = Vector3.zero;
            collider2DChecker.AddComponent<CapsuleCollider2D>();
            CapsuleCollider2D sonCapsuleCollider2D = collider2DChecker.GetComponent<CapsuleCollider2D>();
            sonCapsuleCollider2D.isTrigger = true;
            sonCapsuleCollider2D.offset = new Vector2(0, 0.5f);
            sonCapsuleCollider2D.size = new Vector2(5, 5);
            if (this.gameObject.tag.Equals("Player"))
            {
                collider2DChecker.AddComponent<InteractorChecker>();
            }
        }*/
        #endregion
    }
}