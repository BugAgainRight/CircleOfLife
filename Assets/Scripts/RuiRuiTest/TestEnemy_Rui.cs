using CircleOfLife.Battle;
using CircleOfLife.Buff;
using CircleOfLife.Units;
using Milutools.Recycle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class TestEnemy_Rui : MonoBehaviour,IBattleEntity
    {
        public BattleStats.Stats Stat;
        public BattleStats Stats { get; set; }

        public FactionType FactionType => Faction;
        public FactionType Faction;

        private void Awake()
        {
            Stats = Stat.Build(gameObject, (context) => { if (context.HitData.Current.Hp <= 0) Destroy(gameObject); });
        }

        public NPCData GetData()
        {
            var result= new NPCData();
           
            return result;
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
