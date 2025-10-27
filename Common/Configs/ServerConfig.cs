using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;
using SignatureEquipmentDeluxe.Common.Systems;

namespace SignatureEquipmentDeluxe.Common.Configs
{
    [BackgroundColor(30, 30, 40)]
    public class ServerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        // ==================== EXPERIÊNCIA ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.ExperienceHeader")]
        
        [BackgroundColor(50, 50, 60)]
        [DefaultValue(1f)]
        [Range(0f, 10f)]
        [Increment(0.1f)]
        public float GlobalExpMultiplier { get; set; }
        
        [BackgroundColor(50, 50, 60)]
        [DefaultValue(5f)]
        [Range(0f, 1000f)]
        [Increment(1f)]
        public float BaseXPPerHit { get; set; }
        
        [BackgroundColor(50, 50, 60)]
        [DefaultValue(50f)]
        [Range(0f, 10000f)]
        [Increment(10f)]
        public float BaseXPPerKill { get; set; }
        
        [BackgroundColor(50, 50, 60)]
        [DefaultValue(0.01f)]
        [Range(0f, 1f)]
        [Increment(0.001f)]
        public float XPPerDamageDealt { get; set; }
        
        [BackgroundColor(50, 50, 60)]
        [DefaultValue(0.1f)]
        [Range(0f, 1f)]
        [Increment(0.01f)]
        public float XPPerEnemyMaxHP { get; set; }

        [BackgroundColor(50, 50, 60)]
        [DefaultValue(1f)]
        [Range(0f, 10f)]
        [Increment(0.1f)]
        public float WeaponExpMultiplier { get; set; }

        [BackgroundColor(50, 50, 60)]
        [DefaultValue(1f)]
        [Range(0f, 10f)]
        [Increment(0.1f)]
        public float ArmorExpMultiplier { get; set; }

        [BackgroundColor(50, 50, 60)]
        [DefaultValue(1f)]
        [Range(0f, 10f)]
        [Increment(0.1f)]
        public float AccessoryExpMultiplier { get; set; }

        [BackgroundColor(50, 50, 60)]
        [DefaultValue(1f)]
        [Range(0f, 10f)]
        [Increment(0.1f)]
        public float BossExpMultiplier { get; set; }
        
        [BackgroundColor(50, 50, 60)]
        [DefaultValue(false)]
        public bool AllowStatueXP { get; set; }
        
        [BackgroundColor(50, 50, 60)]
        [DefaultValue(1f)]
        [Range(0f, 10f)]
        [Increment(0.1f)]
        public float ArmorXPPerDamageReceived { get; set; }
        
        [BackgroundColor(50, 50, 60)]
        [DefaultValue(false)]
        public bool ArmorXPIgnoreDefense { get; set; }

        [BackgroundColor(50, 50, 60)]
        [DefaultValue(100)]
        [Range(1, 10000)]
        public int BaseExpPerLevel { get; set; }

        [BackgroundColor(50, 50, 60)]
        [DefaultValue(1.1f)]
        [Range(1f, 3f)]
        [Increment(0.05f)]
        public float ExpScalingFactor { get; set; }

        // ==================== DAMAGE STATS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.DamageStatsHeader")]
        
        [BackgroundColor(60, 40, 40)]
        public ItemStatInt Damage { get; set; } = new ItemStatInt
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1,
            PerLevelMult = 1,
            Max = 999999,
            ScalingTiers = new List<ScalingTier>
            {
                new ScalingTier { StartLevel = 1, PerLevel = 1f, PerLevelMult = 1 },
                new ScalingTier { StartLevel = 50, PerLevel = 2f, PerLevelMult = 1 },
                new ScalingTier { StartLevel = 100, PerLevel = 5f, PerLevelMult = 1 }
            }
        };

        [BackgroundColor(60, 40, 40)]
        public ItemStatInt CritChance { get; set; } = new ItemStatInt
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 0,
            PerLevelMult = 1,
            Max = 100,
            ScalingTiers = new List<ScalingTier>()
        };

        [BackgroundColor(60, 40, 40)]
        public ItemStatInt Defense { get; set; } = new ItemStatInt
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 0,
            PerLevelMult = 1,
            Max = 999999,
            ScalingTiers = new List<ScalingTier>()
        };

        // ==================== SPEED STATS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.SpeedStatsHeader")]
        
        [BackgroundColor(40, 60, 40)]
        public ItemStatFloat UseSpeed { get; set; } = new ItemStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 0f,
            PerLevelMult = 1,
            Max = 100,
            ScalingTiers = new List<ScalingTier>()
        };

        [BackgroundColor(40, 60, 40)]
        public ItemStatFloat MeleeSpeed { get; set; } = new ItemStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 0f,
            PerLevelMult = 1,
            Max = 100,
            ScalingTiers = new List<ScalingTier>()
        };

        // ==================== SIZE STATS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.SizeStatsHeader")]
        
        [BackgroundColor(40, 40, 60)]
        public ItemStatFloat MeleeSize { get; set; } = new ItemStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 0f,
            PerLevelMult = 1,
            Max = 300,
            ScalingTiers = new List<ScalingTier>()
        };

        [BackgroundColor(40, 40, 60)]
        public ProjectileStatFloat ProjectileSize { get; set; } = new ProjectileStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 0f,
            PerLevelMult = 1,
            Max = 300,
            ScalingTiers = new List<ScalingTier>()
        };

        // ==================== PROJECTILE STATS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.ProjectileStatsHeader")]
        
        [BackgroundColor(60, 40, 60)]
        public ProjectileStatFloat ProjectileSpeed { get; set; } = new ProjectileStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 0f,
            PerLevelMult = 1,
            Max = 300,
            ScalingTiers = new List<ScalingTier>()
        };

        [BackgroundColor(60, 40, 60)]
        public ProjectileStatFloat ProjectileLifeTime { get; set; } = new ProjectileStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 0f,
            PerLevelMult = 1,
            Max = 500,
            ScalingTiers = new List<ScalingTier>()
        };

        [BackgroundColor(60, 40, 60)]
        public ProjectileStatFloat ProjectilePenetration { get; set; } = new ProjectileStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 0f,
            PerLevelMult = 1,
            Max = 100,
            ScalingTiers = new List<ScalingTier>()
        };

        // ==================== UTILITY STATS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.UtilityStatsHeader")]
        
        [BackgroundColor(40, 60, 60)]
        public ItemStatFloat ManaCostReduction { get; set; } = new ItemStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 0f,
            PerLevelMult = 1,
            Max = 99,
            ScalingTiers = new List<ScalingTier>()
        };

        [BackgroundColor(40, 60, 60)]
        public ItemStatFloat AmmoConsumptionReduction { get; set; } = new ItemStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 0f,
            PerLevelMult = 1,
            Max = 99,
            ScalingTiers = new List<ScalingTier>()
        };

        [BackgroundColor(40, 60, 60)]
        public ItemStatFloat MinionSlotReduction { get; set; } = new ItemStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 0f,
            PerLevelMult = 1,
            Max = 95,
            ScalingTiers = new List<ScalingTier>()
        };

        // ==================== BLACKLISTS & WHITELISTS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.ListsHeader")]
        
        [BackgroundColor(60, 50, 40)]
        public HashSet<ItemDefinition> GlobalItemBlacklist { get; set; } = new HashSet<ItemDefinition>();

        [BackgroundColor(60, 50, 40)]
        public HashSet<NPCDefinition> NPCBlacklist { get; set; } = new HashSet<NPCDefinition>();

        [BackgroundColor(60, 50, 40)]
        public List<ItemProjectileReference> SpecialProjectileMapping { get; set; } = new List<ItemProjectileReference>();

        // ==================== PROJECTILE IMMUNITY ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.ImmunityHeader")]
        
        [BackgroundColor(50, 40, 60)]
        [DefaultValue(LocalNPCImmunityMode.Disabled)]
        public LocalNPCImmunityMode LocalImmunityMode { get; set; }

        [BackgroundColor(50, 40, 60)]
        public List<ProjectileLocalFrames> ProjectileLocalFramesOverride { get; set; } = new List<ProjectileLocalFrames>();

        // ==================== ADVANCED ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.AdvancedHeader")]
        
        [BackgroundColor(40, 40, 50)]
        [DefaultValue(true)]
        public bool EnableMultiplayerSync { get; set; }

        [BackgroundColor(40, 40, 50)]
        [DefaultValue(true)]
        public bool EnableProjectileStatCaching { get; set; }

        [BackgroundColor(40, 40, 50)]
        [DefaultValue(false)]
        public bool DebugMode { get; set; }
        
        // ==================== LEVEL LIMITS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.LevelLimitsHeader")]
        
        [BackgroundColor(50, 60, 50)]
        [DefaultValue(0)]
        [Range(0, 9999)]
        public int GlobalMaxLevel { get; set; }
        
        [BackgroundColor(50, 60, 50)]
        public Dictionary<ItemDefinition, int> IndividualMaxLevel { get; set; } = new Dictionary<ItemDefinition, int>();
    }
}
