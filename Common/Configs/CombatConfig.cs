using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;
using SignatureEquipmentDeluxe.Common.Systems;

namespace SignatureEquipmentDeluxe.Common.Configs
{
    /// <summary>
    /// Configurações de Combat Tuning
    /// Controle granular de stats de combate
    /// </summary>
    [BackgroundColor(60, 40, 40)]
    public class CombatConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        // ==================== WEAPON STATS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.CombatConfig.WeaponStatsHeader")]
        
        [BackgroundColor(60, 40, 40)]
        public ItemStatInt WeaponDamage { get; set; } = new ItemStatInt
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>
            {
                new ScalingTier { StartLevel = 1, PerLevel = 1f, PerLevelMult = 1 },
                new ScalingTier { StartLevel = 50, PerLevel = 2f, PerLevelMult = 1 },
                new ScalingTier { StartLevel = 100, PerLevel = 5f, PerLevelMult = 1 }
            }
        };

        [BackgroundColor(60, 40, 40)]
        public ItemStatInt WeaponCritChance { get; set; } = new ItemStatInt
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };
        
        [BackgroundColor(60, 40, 40)]
        public ItemStatFloat WeaponUseTime { get; set; } = new ItemStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1f,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };

        [BackgroundColor(60, 40, 40)]
        public ItemStatFloat WeaponUseAnimation { get; set; } = new ItemStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1f,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };

        [BackgroundColor(60, 40, 40)]
        public ItemStatFloat WeaponMeleeSize { get; set; } = new ItemStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1f,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };
        
        [BackgroundColor(60, 40, 40)]
        public ItemStatFloat WeaponManaCostReduction { get; set; } = new ItemStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1f,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };

        [BackgroundColor(60, 40, 40)]
        public ItemStatFloat WeaponAmmoConsumptionReduction { get; set; } = new ItemStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1f,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };

        // ==================== PROJECTILE STATS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.CombatConfig.ProjectileStatsHeader")]

        [BackgroundColor(60, 40, 40)]
        public ProjectileStatFloat WeaponProjectileSize { get; set; } = new ProjectileStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1f,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };

        [BackgroundColor(60, 40, 40)]
        public ProjectileStatFloat WeaponProjectileSpeed { get; set; } = new ProjectileStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1f,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };

        [BackgroundColor(60, 40, 40)]
        public ProjectileStatFloat WeaponProjectileLifeTime { get; set; } = new ProjectileStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1f,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };

        [BackgroundColor(60, 40, 40)]
        public ProjectileStatFloat WeaponProjectilePenetration { get; set; } = new ProjectileStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1f,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };
        
        [BackgroundColor(60, 40, 40)]
        public ProjectileStatFloat WeaponAdditionalProjectileChance { get; set; } = new ProjectileStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1f,
            PerLevelMult = 2,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };

        // ==================== ARMOR STATS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.CombatConfig.ArmorStatsHeader")]
        
        [BackgroundColor(40, 40, 60)]
        public ItemStatInt ArmorDefense { get; set; } = new ItemStatInt
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };

        // ==================== PROJECTILE IMMUNITY ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.CombatConfig.ImmunityHeader")]
        
        [BackgroundColor(50, 40, 60)]
        [DefaultValue(LocalNPCImmunityMode.Disabled)]
        public LocalNPCImmunityMode LocalImmunityMode { get; set; } = LocalNPCImmunityMode.Disabled;

        [BackgroundColor(50, 40, 60)]
        public List<ProjectileLocalFrames> ProjectileLocalFramesOverride { get; set; } = new List<ProjectileLocalFrames>();
    }
}
