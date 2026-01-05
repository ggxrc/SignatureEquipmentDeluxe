using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Progression.Common.Systems
{
    public class ProjectileStatFloat
    {
        /// <summary>
        /// Modo de scaling: Fixed (fixo) ou Tiered (escalonado)
        /// </summary>
        [DefaultValue(ScalingMode.Fixed)]
        public ScalingMode ScalingMode = ScalingMode.Fixed;

        [Header("$Mods.Progression.Config.ItemStat.FixedSettingsHeader")]
        
        /// <summary>
        /// Bônus base por nível (Modo Fixed)
        /// </summary>
        [Range(-2.5f, 2.5f)]
        [Increment(0.05f)]
        [DefaultValue(1f)]
        public float PerLevel { get; set; } = 1f;

        /// <summary>
        /// Multiplicador do bônus (Modo Fixed)
        /// </summary>
        [Range(int.MinValue, int.MaxValue)]
        [DefaultValue(1)]
        public int PerLevelMult { get; set; } = 1;

        [Header("$Mods.Progression.Config.ItemStat.TieredSettingsHeader")]
        
        /// <summary>
        /// Lista de tiers de scaling progressivo (Modo Tiered)
        /// Cada tier define uma faixa de níveis com seus próprios valores
        /// </summary>
        public List<ScalingTier> ScalingTiers { get; set; } = new List<ScalingTier>();

        [Header("$Mods.Progression.Config.ItemStat.GlobalSettingsHeader")]
        
        /// <summary>
        /// Valor máximo global (0 = sem limite)
        /// </summary>
        [Range(0, int.MaxValue)]
        [DefaultValue(0)]
        public int Max;

        /// <summary>
        /// Limites específicos por item
        /// </summary>
        public Dictionary<ItemDefinition, StatHardCap> ItemHardCap = new Dictionary<ItemDefinition, StatHardCap>();

        /// <summary>
        /// Limites específicos por projétil
        /// </summary>
        public Dictionary<ProjectileDefinition, StatHardCap> ProjectileHardCap = new Dictionary<ProjectileDefinition, StatHardCap>();

        /// <summary>
        /// Itens que não recebem este bônus
        /// </summary>
        public HashSet<ItemDefinition> ItemBlackList = new HashSet<ItemDefinition>();

        /// <summary>
        /// Projéteis que não recebem este bônus
        /// </summary>
        public HashSet<ProjectileDefinition> ProjectileBlackList = new HashSet<ProjectileDefinition>();
    }
}
