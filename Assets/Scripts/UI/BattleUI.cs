using System;
using CircleOfLife.Battle;
using CircleOfLife.Level;
using CircleOfLife.Units;
using Milease.Utils;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CircleOfLife
{
    public class BattleUI : MonoBehaviour
    {
        public static BattleUI Instance;
        
        public TMP_Text RoundTitle, PlayerHPText;
        public Image AnimalHPBar, PlayerHPBar;
        public RectTransform SkillFill;
        public float MaxAnimalHPWidth, MaxPlayerHPWidth;
        public GameObject SkillBurstEffect;
        public SkeletonGraphic AnimalGraphic;
        
        private void Awake()
        {
            Instance = this;
            MaxAnimalHPWidth = AnimalHPBar.rectTransform.sizeDelta.x;
            MaxPlayerHPWidth = PlayerHPBar.rectTransform.sizeDelta.x;
        }
        
        public void UpdateEnergyBar(float pro)
        {
            SkillFill.sizeDelta = new Vector2(SkillFill.sizeDelta.x, pro * 76f);
            SkillBurstEffect.SetActive(pro >= 1f);
        }

        public void UpdateRoundText(int cur, int max)
        {
            RoundTitle.text = $"回合 {cur}/{max}";
        }

        private void UpdateHPBar(Image image, BattleStats stats, float maxW)
        {
            var rect = image.rectTransform;
            var pro = (stats.Current.Hp / stats.Max.Hp);
            image.color = pro switch
            {
                < 0.3f => Color.red,
                > 0.7f => Color.green,
                _ => ColorUtils.RGB(255, 178, 0)
            };
            rect.sizeDelta = new Vector2(maxW * pro, rect.sizeDelta.y);
        }
        
        private void Update()
        {
            UpdateHPBar(AnimalHPBar, ProtectAnimal.Instance.Stats, MaxAnimalHPWidth);
            UpdateHPBar(PlayerHPBar, PlayerController.Instance.Stats, MaxPlayerHPWidth);
            PlayerHPText.text = $"{Mathf.RoundToInt(PlayerController.Instance.Stats.Current.Hp)}/{PlayerController.Instance.Stats.Max.Hp}";
        }
    }
}
