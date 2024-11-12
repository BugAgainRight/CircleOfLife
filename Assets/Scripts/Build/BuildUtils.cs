using System.Linq;
using CircleOfLife.Battle;
using CircleOfLife.Buff;

namespace CircleOfLife.Build
{
    public static class BuildUtils
    {
        public static bool CurrentEnable = false;
        
        public static void EnableAllBuilding()
        {
            CurrentEnable = true;
            foreach (var stat in BuffManager.GetAllStats().Where(x => x.BattleEntity is BuildBase))
            {
                ((BuildBase)stat.BattleEntity).Switch = true;
            }
        }
        
        public static void DisableAllBuilding()
        {
            CurrentEnable = false;
            foreach (var stat in BuffManager.GetAllStats().Where(x => x.BattleEntity is BuildBase))
            {
                ((BuildBase)stat.BattleEntity).Switch = false;
            }
        }
    }
}
