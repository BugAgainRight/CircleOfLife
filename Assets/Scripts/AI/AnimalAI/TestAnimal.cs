using System.Collections;
using System.Collections.Generic;
using CircleOfLife.Battle;
using CircleOfLife.Buff;

using UnityEngine;

namespace CircleOfLife
{
    public class TestAnimal : MonoBehaviour
    {
        public BattleStats.Stats Stat;
        public BattleStats Stats { get; set; }
        public FactionType FactionType => Faction;
        public FactionType Faction;
        private AnimalAIContext animalAIContext;

        private void Awake()
        {
            animalAIContext = GetComponent<AnimalAIContext>();
            animalAIContext.OnAnimalDead.AddListener(OnDead);
            animalAIContext.OnAnimalAwake.AddListener(OnAwake);
        }

        void Update()
        {

        }
        public void OnDead()
        {
            Debug.Log("Animal Dead");
            Stats.ApplyBuff(BuffUtils.ToBuff(WakeUpBuff).SetMaxLevel(1));
        }
        public void OnAwake()
        {
            //Stats.ReduceBuff(BuffUtils.ToBuff(WakeUpBuff).SetMaxLevel(1));
        }
        public static void WakeUpBuff(BattleStats stats, BuffContext context)
        {
            stats.Max.Hp += 100;
            stats.Current.Hp += stats.Max.Hp * 0.02f + 10;
        }
    }
}
