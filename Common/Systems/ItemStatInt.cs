using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    public class ItemStatInt
    {
        /// <summary>
        /// Modo de scaling: Legacy, Cumulative, CurrentTierOnly, ou AccumulativePerTier
        /// </summary>
        [DefaultValue(ScalingMode.Legacy)]
        public ScalingMode ScalingMode = ScalingMode.Legacy;

        /// <summary>
        /// Lista de tiers de scaling progressivo
        /// Cada tier define uma faixa de níveis com seus próprios valores de PerLevel e PerLevelMult
        /// </summary>
        public List<ScalingTier> ScalingTiers = new List<ScalingTier>();

        [Range(int.MinValue, int.MaxValue)]
        public int PerLevel;

        [Range(int.MinValue, int.MaxValue)]
        public int PerLevelMult;

        [Range(0, int.MaxValue)]
        public int Max;

        public Dictionary<ItemDefinition, StatHardCap> HardCap = new Dictionary<ItemDefinition, StatHardCap>();

        public HashSet<ItemDefinition> ItemBlackList = new HashSet<ItemDefinition>();
    }
}
