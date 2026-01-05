using System.Collections.Generic;
using System.ComponentModel;
using Progression.Common.Systems;
using Terraria.ModLoader.Config;

namespace Progression.Common.Configs
{
    /// <summary>
    /// Scaling Settings
    /// Granular control of stat scaling
    /// </summary>
    [BackgroundColor(60, 40, 40)]
    public class ScalingConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        // ==================== LEVEL CAPS ====================

        [Header("$Mods.Progression.Config.GameplayConfig.LevelCapsHeader")]
        [BackgroundColor(100, 70, 70)]
        [DefaultValue(WeaponLevelCapMode.WorldLevel)]
        public WeaponLevelCapMode WeaponCapMode { get; set; } = WeaponLevelCapMode.WorldLevel;

        [BackgroundColor(100, 70, 70)]
        [DefaultValue(100)]
        [Range(1, 1000)]
        public int WeaponIndependentMaxLevel { get; set; } = 100;

        [BackgroundColor(70, 70, 100)]
        [DefaultValue(100)]
        [Range(0, 1000)]
        public int ArmorMaxLevel { get; set; } = 100;

        // ==================== WEAPON STATS ====================

        [Header("$Mods.Progression.Config.GameplayConfig.WeaponStatsHeader")]
        [BackgroundColor(110, 60, 60)]
        [DefaultValue(true)]
        public bool DamageIncrement { get; set; } = true;

        [BackgroundColor(105, 55, 55)]
        [DefaultValue(true)]
        public bool CritIncrement { get; set; } = true;

        [BackgroundColor(100, 50, 50)]
        [DefaultValue(true)]
        public bool UseTimeIncrement { get; set; } = true;

        [BackgroundColor(95, 45, 45)]
        [DefaultValue(true)]
        public bool UseAnimationIncrement { get; set; } = true;

        // ==================== DAMAGE TYPES ====================

        [Header("$Mods.Progression.Config.GameplayConfig.DamageTypesHeader")]
        [BackgroundColor(70, 50, 50)]
        [DefaultValue(true)]
        public bool IncreaseBaseDamage { get; set; } = true;

        [BackgroundColor(70, 50, 50)]
        [DefaultValue(false)]
        public bool IncreaseFlatDamage { get; set; } = false;

        [BackgroundColor(70, 50, 50)]
        [DefaultValue(false)]
        public bool IncreaseMultDamage { get; set; } = false;

        // ==================== PROJECTILE TOGGLES ====================

        [Header("$Mods.Progression.Config.GameplayConfig.ProjectileTogglesHeader")]
        [BackgroundColor(50, 60, 70)]
        [DefaultValue(true)]
        public bool ProjectileSizeIncrement { get; set; } = true;

        [BackgroundColor(50, 60, 70)]
        [DefaultValue(true)]
        public bool ProjectileSpeedIncrement { get; set; } = true;

        [BackgroundColor(50, 60, 70)]
        [DefaultValue(true)]
        public bool ProjectilePenetrationIncrement { get; set; } = true;

        [BackgroundColor(50, 60, 70)]
        [DefaultValue(true)]
        public bool ProjectileLifeTimeIncrement { get; set; } = true;

        [BackgroundColor(50, 60, 70)]
        [DefaultValue(true)]
        public bool AdditionalProjectileChanceIncrement { get; set; } = true;

        [BackgroundColor(50, 60, 70)]
        [DefaultValue(true)]
        public bool MeleeWeaponSizeIncrement { get; set; } = true;

        // ==================== RESOURCE TOGGLES ====================

        [Header("$Mods.Progression.Config.GameplayConfig.ResourceTogglesHeader")]
        [BackgroundColor(50, 70, 60)]
        [DefaultValue(true)]
        public bool NotUseAmmoChanceIncrement { get; set; } = true;

        [BackgroundColor(50, 70, 60)]
        [DefaultValue(true)]
        public bool ManaCostReductionIncrement { get; set; } = true;

        // ==================== STATUS INCREASE TYPE ====================

        [Header("$Mods.Progression.Config.ScalingConfig.StatusIncreaseTypeHeader")]
        [BackgroundColor(80, 60, 40)]
        [DefaultValue(StatusIncreaseType.Raw)]
        public StatusIncreaseType StatusIncreaseType { get; set; } = StatusIncreaseType.Raw;

        // ==================== WEAPON SCALING ====================

        [Header("$Mods.Progression.Config.ScalingConfig.WeaponStatsHeader")]
        [BackgroundColor(60, 40, 40)]
        public ItemStatInt WeaponDamage { get; set; } =
            new ItemStatInt
            {
                ScalingMode = ScalingMode.Fixed,
                PerLevel = 1,
                PerLevelMult = 1,
                Max = 0,
                ScalingTiers = new List<ScalingTier>
                {
                    new ScalingTier
                    {
                        StartLevel = 1,
                        PerLevel = 0.25f,
                        PerLevelMult = 1,
                    },
                    new ScalingTier
                    {
                        StartLevel = 50,
                        PerLevel = 0.75f,
                        PerLevelMult = 1,
                    },
                    new ScalingTier
                    {
                        StartLevel = 100,
                        PerLevel = 1.5f,
                        PerLevelMult = 1,
                    },
                },
            };

        [BackgroundColor(60, 40, 40)]
        public ItemStatInt WeaponCritChance { get; set; } =
            new ItemStatInt
            {
                ScalingMode = ScalingMode.Fixed,
                PerLevel = 1,
                PerLevelMult = 1,
                Max = 0,
                ScalingTiers = new List<ScalingTier>
                {
                    new ScalingTier
                    {
                        StartLevel = 1,
                        PerLevel = 0.25f,
                        PerLevelMult = 1,
                    },
                    new ScalingTier
                    {
                        StartLevel = 50,
                        PerLevel = 0.75f,
                        PerLevelMult = 1,
                    },
                    new ScalingTier
                    {
                        StartLevel = 100,
                        PerLevel = 1.5f,
                        PerLevelMult = 1,
                    },
                },
            };

        [BackgroundColor(60, 40, 40)]
        public ItemStatFloat WeaponUseTime { get; set; } =
            new ItemStatFloat
            {
                ScalingMode = ScalingMode.Fixed,
                PerLevel = 1f,
                PerLevelMult = 1,
                Max = 0,
                ScalingTiers = new List<ScalingTier>(),
            };

        [BackgroundColor(60, 40, 40)]
        public ItemStatFloat WeaponUseAnimation { get; set; } =
            new ItemStatFloat
            {
                ScalingMode = ScalingMode.Fixed,
                PerLevel = 1f,
                PerLevelMult = 1,
                Max = 0,
                ScalingTiers = new List<ScalingTier>(),
            };

        [BackgroundColor(60, 40, 40)]
        public ItemStatFloat WeaponMeleeSize { get; set; } =
            new ItemStatFloat
            {
                ScalingMode = ScalingMode.Fixed,
                PerLevel = 1f,
                PerLevelMult = 1,
                Max = 0,
                ScalingTiers = new List<ScalingTier>(),
            };

        [BackgroundColor(60, 40, 40)]
        public ItemStatFloat WeaponManaCostReduction { get; set; } =
            new ItemStatFloat
            {
                ScalingMode = ScalingMode.Fixed,
                PerLevel = 1f,
                PerLevelMult = 1,
                Max = 0,
                ScalingTiers = new List<ScalingTier>(),
            };

        [BackgroundColor(60, 40, 40)]
        public ItemStatFloat WeaponAmmoConsumptionReduction { get; set; } =
            new ItemStatFloat
            {
                ScalingMode = ScalingMode.Fixed,
                PerLevel = 1f,
                PerLevelMult = 1,
                Max = 0,
                ScalingTiers = new List<ScalingTier>(),
            };

        // ==================== PROJECTILE STATS ====================

        [Header("$Mods.Progression.Config.ScalingConfig.ProjectileStatsHeader")]
        [BackgroundColor(60, 40, 40)]
        public ProjectileStatFloat WeaponProjectileSize { get; set; } =
            new ProjectileStatFloat
            {
                ScalingMode = ScalingMode.Fixed,
                PerLevel = 1f,
                PerLevelMult = 1,
                Max = 0,
                ScalingTiers = new List<ScalingTier>(),
            };

        [BackgroundColor(60, 40, 40)]
        public ProjectileStatFloat WeaponProjectileSpeed { get; set; } =
            new ProjectileStatFloat
            {
                ScalingMode = ScalingMode.Fixed,
                PerLevel = 1f,
                PerLevelMult = 1,
                Max = 0,
                ScalingTiers = new List<ScalingTier>(),
            };

        [BackgroundColor(60, 40, 40)]
        public ProjectileStatFloat WeaponProjectileLifeTime { get; set; } =
            new ProjectileStatFloat
            {
                ScalingMode = ScalingMode.Fixed,
                PerLevel = 1f,
                PerLevelMult = 1,
                Max = 0,
                ScalingTiers = new List<ScalingTier>(),
            };

        [BackgroundColor(60, 40, 40)]
        public ProjectileStatFloat WeaponProjectilePenetration { get; set; } =
            new ProjectileStatFloat
            {
                ScalingMode = ScalingMode.Fixed,
                PerLevel = 1f,
                PerLevelMult = 1,
                Max = 0,
                ScalingTiers = new List<ScalingTier>(),
            };

        [BackgroundColor(60, 40, 40)]
        public ProjectileStatFloat WeaponAdditionalProjectileChance { get; set; } =
            new ProjectileStatFloat
            {
                ScalingMode = ScalingMode.Fixed,
                PerLevel = 1f,
                PerLevelMult = 2,
                Max = 0,
                ScalingTiers = new List<ScalingTier>(),
            };

        // ==================== ARMOR STATS ====================

        [Header("$Mods.Progression.Config.ScalingConfig.ArmorStatsHeader")]
        [BackgroundColor(40, 40, 60)]
        public ItemStatInt ArmorDefense { get; set; } =
            new ItemStatInt
            {
                ScalingMode = ScalingMode.Fixed,
                PerLevel = 1,
                PerLevelMult = 1,
                Max = 0,
                ScalingTiers = new List<ScalingTier>(),
            };

        // ==================== PROJECTILE IMMUNITY ====================

        [Header("$Mods.Progression.Config.ScalingConfig.ImmunityHeader")]
        [BackgroundColor(50, 40, 60)]
        [DefaultValue(LocalNPCImmunityMode.Disabled)]
        public LocalNPCImmunityMode LocalImmunityMode { get; set; } = LocalNPCImmunityMode.Disabled;

        [BackgroundColor(50, 40, 60)]
        public List<ProjectileLocalFrames> ProjectileLocalFramesOverride { get; set; } =
            new List<ProjectileLocalFrames>();

        // ==================== INDIVIDUAL ITEM OVERRIDES ====================

        [Header("$Mods.Progression.Config.ScalingConfig.IndividualOverridesHeader")]
        [BackgroundColor(100, 80, 40)]
        public List<IndividualItemOverride> ItemOverrides { get; set; } =
            new List<IndividualItemOverride>();
    }
}
