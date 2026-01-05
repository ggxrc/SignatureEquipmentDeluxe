using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Terraria.ModLoader.Config;

namespace Progression.Common.Configs
{
    /// <summary>
    /// Progression Settings
    /// How players gain power over time
    /// </summary>
    [BackgroundColor(50, 40, 60)]
    public class ProgressionConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        // ==================== GLOBAL MULTIPLIERS ====================

        [Header("$Mods.Progression.Config.ProgressionConfig.GlobalMultipliersHeader")]
        [BackgroundColor(60, 50, 70)]
        [Range(0f, 5f)]
        [DefaultValue(1f)]
        [Increment(0.25f)]
        public float GlobalExpMultiplier { get; set; } = 1f;

        [BackgroundColor(60, 50, 70)]
        [Range(1f, 3f)]
        [DefaultValue(1f)]
        [Increment(0.25f)]
        public float GlobalExpMultiplierExtra { get; set; } = 1f;

        // ==================== CATEGORY MULTIPLIERS ====================

        [Header("$Mods.Progression.Config.ProgressionConfig.CategoryMultipliersHeader")]
        [BackgroundColor(70, 50, 60)]
        [Range(0f, 5f)]
        [DefaultValue(1f)]
        [Increment(0.25f)]
        public float WeaponExpMultiplier { get; set; } = 1f;

        [BackgroundColor(70, 50, 60)]
        [Range(0f, 5f)]
        [DefaultValue(1f)]
        [Increment(0.25f)]
        public float ArmorExpMultiplier { get; set; } = 1f;

        // ==================== WEAPON XP SOURCES ====================

        [Header("$Mods.Progression.Config.ProgressionConfig.WeaponXPSourcesHeader")]
        [BackgroundColor(60, 40, 40)]
        [Range(0, 1000000)]
        [DefaultValue(5)]
        public int WeaponBaseXPPerHit { get; set; } = 5;

        [BackgroundColor(60, 40, 40)]
        [Range(0, 1000000)]
        [DefaultValue(50)]
        public int WeaponBaseXPPerKill { get; set; } = 50;

        [BackgroundColor(60, 40, 40)]
        [DefaultValue(1f)]
        [Range(1f, 10f)]
        [Increment(1f)]
        public float WeaponXPPerDamageDealt { get; set; } = 1f;

        [BackgroundColor(60, 40, 40)]
        [DefaultValue(1f)]
        [Range(1f, 10f)]
        [Increment(1f)]
        public float WeaponXPPerEnemyMaxHP { get; set; } = 1f;

        // ==================== ARMOR XP SOURCES ====================

        [Header("$Mods.Progression.Config.ProgressionConfig.ArmorXPSourcesHeader")]
        [BackgroundColor(40, 40, 60)]
        [Range(0, 1000000)]
        [DefaultValue(10)]
        public int ArmorXPPerDamageReceived { get; set; } = 10;

        [BackgroundColor(40, 40, 60)]
        [DefaultValue(false)]
        public bool ArmorXPIgnoreDefense { get; set; } = false;

        [BackgroundColor(40, 40, 60)]
        [Range(0, 1000000)]
        [DefaultValue(1)]
        public int ArmorXPPerDamageBlocked { get; set; } = 1;

        [BackgroundColor(40, 40, 60)]
        [Range(0, 1000000)]
        [DefaultValue(0)]
        public int ArmorXPPerDodge { get; set; } = 0;

        // ==================== SPECIAL CONDITIONS ====================

        [Header("$Mods.Progression.Config.ProgressionConfig.SpecialConditionsHeader")]
        [BackgroundColor(50, 50, 60)]
        [DefaultValue(false)]
        public bool AllowStatueXP { get; set; } = false;

        // ==================== LEVEL COST CURVE ====================

        [Header("$Mods.Progression.Config.ProgressionConfig.LevelCostCurveHeader")]
        [BackgroundColor(60, 60, 50)]
        [Range(1, 1000000)]
        [DefaultValue(1000)]
        public int StartPrice { get; set; } = 1000;

        [BackgroundColor(60, 60, 50)]
        [Range(0, 1000000)]
        [DefaultValue(0)]
        public int AditionalPrice { get; set; } = 0;

        [BackgroundColor(60, 60, 50)]
        [Range(1f, 2.5f)]
        [DefaultValue(1f)]
        [Increment(0.01f)]
        public float MultiPrice { get; set; } = 1f;

        // ==================== KILL STREAK SYSTEM ====================

        [Header("$Mods.Progression.Config.ProgressionConfig.KillStreakHeader")]
        [BackgroundColor(60, 50, 40)]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool EnableKillStreakSystem { get; set; } = true;

        [BackgroundColor(60, 50, 40)]
        [DefaultValue(true)]
        public bool EnableStreakXPBonus { get; set; } = true;

        [BackgroundColor(60, 50, 40)]
        [Range(1f, 5f)]
        [Increment(1f)]
        [DefaultValue(1f)]
        public float StreakXPBonusPerKill { get; set; } = 1f;

        [BackgroundColor(60, 50, 40)]
        [Range(0, 1000)]
        [DefaultValue(50)]
        public int StreakXPBonusMax { get; set; } = 50;
    }
}
