using UnityEngine;

namespace CircleOfLife.Utils
{
    public static class PlayerUtils
    {
        public static bool IsPlayer(this GameObject go)
            => go.CompareTag("Player");
    }
}
