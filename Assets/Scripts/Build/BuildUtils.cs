using System.Linq;
using CircleOfLife.Battle;
using CircleOfLife.Buff;

namespace CircleOfLife.Build
{
    public static class BuildUtils
    {
        public static void EnableAllBuilding()
        {
            foreach (var stat in BuffManager.GetAllStats().Where(x => x.BattleEntity is BuildBase))
            {
                ((BuildBase)stat.BattleEntity).Switch = true;
            }
        }
        
        public static void DisableAllBuilding()
        {
            foreach (var stat in BuffManager.GetAllStats().Where(x => x.BattleEntity is BuildBase))
            {
                ((BuildBase)stat.BattleEntity).Switch = false;
            }
        }
    }
}
