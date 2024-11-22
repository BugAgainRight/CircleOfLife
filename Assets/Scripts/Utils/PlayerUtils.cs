using System;
using UnityEngine;

namespace CircleOfLife.Utils
{
    public static class PlayerUtils
    {
        public static bool IsPlayer(this GameObject go)
            => go.CompareTag("Player");

        public static string GetSkillName(this PlayerSkillType skillType)
            => skillType switch
            {
                PlayerSkillType.Melee => "近战攻击",
                PlayerSkillType.Ranged => "远程攻击",
                PlayerSkillType.Whack => "重击",
                PlayerSkillType.Slash => "挥砍",
                PlayerSkillType.FighterBraver => "愈战愈勇",
                PlayerSkillType.Heal => "疗伤",
                PlayerSkillType.Thorn => "荆棘",
                PlayerSkillType.Lurk => "潜伏",
                PlayerSkillType.Encouragement => "鼓舞",
                _ => throw new ArgumentOutOfRangeException(nameof(skillType), skillType, null)
            };
    }
}
