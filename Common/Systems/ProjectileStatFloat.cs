using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    public class ProjectileStatFloat
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

        [Range(-2.5f, 2.5f)]
        [Increment(0.05f)]
        public float PerLevel;

        [Range(int.MinValue, int.MaxValue)]
        public int PerLevelMult;

        [Range(0, int.MaxValue)]
        public int Max;

        public Dictionary<ItemDefinition, StatHardCap> ItemHardCap = new Dictionary<ItemDefinition, StatHardCap>();

        public Dictionary<ProjectileDefinition, StatHardCap> ProjectileHardCap = new Dictionary<ProjectileDefinition, StatHardCap>();

        public HashSet<ItemDefinition> ItemBlackList = new HashSet<ItemDefinition>();

        public HashSet<ProjectileDefinition> ProjectileBlackList = new HashSet<ProjectileDefinition>();
    }
}
