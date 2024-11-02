using CircleOfLife.Battle;
using System;

namespace CircleOfLife.Buff
{
    public static class BuffUtils
    {
        public static BuffContext ToBuff(BuffHandleFunction handler, float duration = -1f)
        {
            return new BuffContext()
            {
                BuffHandler = handler,
                Duration = -1f
            };
        }
    }
}
