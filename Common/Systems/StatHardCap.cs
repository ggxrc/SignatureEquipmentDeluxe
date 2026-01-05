using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Progression.Common.Systems
{
    public class StatHardCap
    {
        [Range(int.MinValue, int.MaxValue)]
        public int Max;

        [DefaultValue(1)]
        [Range(int.MinValue, int.MaxValue)]
        public int Sign;
    }
}
