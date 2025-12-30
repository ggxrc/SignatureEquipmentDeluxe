using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;
using SignatureEquipmentDeluxe.Common.Systems;

namespace SignatureEquipmentDeluxe.Common.Configs
{
    /// <summary>
    /// Configurações de Progressão
    /// Como os jogadores ganham poder ao longo do tempo
    /// </summary>
    [BackgroundColor(50, 40, 60)]
    public class ProgressionConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        // ==================== GLOBAL MULTIPLIERS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ProgressionConfig.GlobalMultipliersHeader")]
        
        [BackgroundColor(60, 50, 70)]
        [Range(0f, 5f)]
        [DefaultValue(1f)]
        [Increment(0.1f)]
        public float GlobalExpMultiplier { get; set; } = 1f;
        
        [BackgroundColor(60, 50, 70)]
        [DefaultValue(1f)]
        [Increment(0.1f)]
        public float GlobalExpMultiplierExtra { get; set; } = 1f;
        
        // ==================== CATEGORY MULTIPLIERS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ProgressionConfig.CategoryMultipliersHeader")]
        
        [BackgroundColor(70, 50, 60)]
        [Range(0f, 5f)]
        [DefaultValue(1f)]
        [Increment(0.1f)]
        public float WeaponExpMultiplier { get; set; } = 1f;
        
        [BackgroundColor(70, 50, 60)]
        [Range(0f, 5f)]
        [DefaultValue(1f)]
        [Increment(0.1f)]
        public float ArmorExpMultiplier { get; set; } = 1f;
        
        // ==================== WEAPON XP SOURCES ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ProgressionConfig.WeaponXPSourcesHeader")]
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(5f)]
        [Increment(1f)]
        public float WeaponBaseXPPerHit { get; set; } = 5f;
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(50f)]
        [Increment(10f)]
        public float WeaponBaseXPPerKill { get; set; } = 50f;
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(0.01f)]
        [Increment(0.01f)]
        public float WeaponXPPerDamageDealt { get; set; } = 0.01f;
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(0.1f)]
        [Increment(0.1f)]
        public float WeaponXPPerEnemyMaxHP { get; set; } = 0.1f;
        
        // ==================== ARMOR XP SOURCES ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ProgressionConfig.ArmorXPSourcesHeader")]
        
        [BackgroundColor(40, 40, 60)]
        [DefaultValue(1f)]
        [Increment(0.1f)]
        public float ArmorXPPerDamageReceived { get; set; } = 1f;
        
        [BackgroundColor(40, 40, 60)]
        [DefaultValue(false)]
        public bool ArmorXPIgnoreDefense { get; set; } = false;
        
        [BackgroundColor(40, 40, 60)]
        [DefaultValue(0.5f)]
        [Increment(0.1f)]
        public float ArmorXPPerDamageBlocked { get; set; } = 0.5f;
        
        [BackgroundColor(40, 40, 60)]
        [DefaultValue(10f)]
        [Increment(1f)]
        public float ArmorXPPerDodge { get; set; } = 10f;
        
        // ==================== SPECIAL CONDITIONS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ProgressionConfig.SpecialConditionsHeader")]
        
        [BackgroundColor(50, 50, 60)]
        [DefaultValue(false)]
        public bool AllowStatueXP { get; set; } = false;
        
        // ==================== LEVEL COST CURVE ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ProgressionConfig.LevelCostCurveHeader")]
        
        [BackgroundColor(60, 60, 50)]
        [DefaultValue(100)]
        public int StartPrice { get; set; } = 100;
        
        [BackgroundColor(60, 60, 50)]
        [DefaultValue(0)]
        public int AditionalPrice { get; set; } = 0;
        
        [BackgroundColor(60, 60, 50)]
        [DefaultValue(0f)]
        [Increment(0.1f)]
        public float ExtraPrice { get; set; } = 0f;
        
        [BackgroundColor(60, 60, 50)]
        [DefaultValue(1f)]
        [Increment(0.1f)]
        public float MultiPrice { get; set; } = 1f;
        
        // ==================== KILL STREAK SYSTEM ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ProgressionConfig.KillStreakHeader")]
        
        [BackgroundColor(60, 50, 40)]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool EnableKillStreakSystem { get; set; } = true;
        
        [BackgroundColor(60, 50, 40)]
        [DefaultValue(true)]
        public bool EnableStreakXPBonus { get; set; } = true;
        
        [BackgroundColor(60, 50, 40)]
        [Range(0.5f, 5f)]
        [Increment(0.5f)]
        [DefaultValue(1f)]
        public float StreakXPBonusPerKill { get; set; } = 1f;
        
        [BackgroundColor(60, 50, 40)]
        [Range(10f, 200f)]
        [Increment(10f)]
        [DefaultValue(50f)]
        public float StreakXPBonusMax { get; set; } = 50f;
    }
}
