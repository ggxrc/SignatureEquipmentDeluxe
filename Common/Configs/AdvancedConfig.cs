using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;
using Progression.Common.Systems;

namespace Progression.Common.Configs
{
    /// <summary>
    /// Advanced and Technical Settings
    /// For experienced users and debugging
    /// </summary>
    [BackgroundColor(40, 40, 50)]
    public class AdvancedConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        // ==================== BLACKLISTS ====================
        
        [Header("$Mods.Progression.Config.AdvancedConfig.BlacklistsHeader")]
        
        [BackgroundColor(60, 50, 40)]
        public HashSet<ItemDefinition> GlobalItemBlacklist { get; set; } = new HashSet<ItemDefinition>();

        [BackgroundColor(60, 50, 40)]
        public HashSet<NPCDefinition> NPCBlacklist { get; set; } = new HashSet<NPCDefinition>();

        [BackgroundColor(60, 50, 40)]
        public List<ItemProjectileReference> SpecialProjectileMapping { get; set; } = new List<ItemProjectileReference>();
        
        // ==================== INDIVIDUAL LEVEL LIMITS ====================
        
        [Header("$Mods.Progression.Config.AdvancedConfig.IndividualLimitsHeader")]
        
        [BackgroundColor(50, 60, 50)]
        public Dictionary<ItemDefinition, int> IndividualMaxLevel { get; set; } = new Dictionary<ItemDefinition, int>();

        // ==================== PERFORMANCE & NETCODE ====================
        
        [Header("$Mods.Progression.Config.AdvancedConfig.PerformanceHeader")]
        
        [BackgroundColor(40, 40, 50)]
        [DefaultValue(true)]
        public bool EnableMultiplayerSync { get; set; } = true;

        [BackgroundColor(40, 40, 50)]
        [DefaultValue(true)]
        public bool EnableProjectileStatCaching { get; set; } = true;

        // ==================== DEBUG ====================
        
        [Header("$Mods.Progression.Config.AdvancedConfig.DebugHeader")]
        
        [BackgroundColor(40, 40, 50)]
        [DefaultValue(false)]
        public bool DebugMode { get; set; } = false;
    }
}
