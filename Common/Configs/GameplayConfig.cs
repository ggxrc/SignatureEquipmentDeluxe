using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SignatureEquipmentDeluxe.Common.Configs
{
    /// <summary>
    /// Configurações de Gameplay Core
    /// Afetam a experiência diária de jogo
    /// </summary>
    [BackgroundColor(40, 50, 60)]
    public class GameplayConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        // ==================== LEVEL CAPS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.GameplayConfig.LevelCapsHeader")]
        
        [BackgroundColor(50, 60, 70)]
        [DefaultValue(0)]
        [Range(0, 500)]
        [Slider]
        public int WeaponMaxLevel { get; set; } = 0;
        
        [BackgroundColor(50, 60, 70)]
        [DefaultValue(0)]
        [Range(0, 500)]
        [Slider]
        public int ArmorMaxLevel { get; set; } = 0;
        
        // ==================== STAT TOGGLES ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.GameplayConfig.StatTogglesHeader")]
        
        [BackgroundColor(60, 50, 50)]
        [DefaultValue(true)]
        public bool DamageIncrement { get; set; } = true;
        
        [BackgroundColor(60, 50, 50)]
        [DefaultValue(true)]
        public bool CritIncrement { get; set; } = true;
        
        [BackgroundColor(60, 50, 50)]
        [DefaultValue(true)]
        public bool UseTimeIncrement { get; set; } = true;
        
        [BackgroundColor(60, 50, 50)]
        [DefaultValue(true)]
        public bool UseAnimationIncrement { get; set; } = true;
        
        [BackgroundColor(60, 50, 50)]
        [DefaultValue(true)]
        public bool DefenceIncrement { get; set; } = true;
        
        // ==================== DAMAGE TYPES ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.GameplayConfig.DamageTypesHeader")]
        
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
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.GameplayConfig.ProjectileTogglesHeader")]
        
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
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.GameplayConfig.ResourceTogglesHeader")]
        
        [BackgroundColor(50, 70, 60)]
        [DefaultValue(true)]
        public bool NotUseAmmoChanceIncrement { get; set; } = true;
        
        [BackgroundColor(50, 70, 60)]
        [DefaultValue(true)]
        public bool ManaCostReductionIncrement { get; set; } = true;
        
        // ==================== ADDITIONAL PROJECTILE CONFIG ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.GameplayConfig.AdditionalProjectileHeader")]
        
        [BackgroundColor(60, 60, 70)]
        [DefaultValue(10)]
        [Range(0, 360)]
        public int AdditionalProjectileMinRad { get; set; } = 10;
        
        [BackgroundColor(60, 60, 70)]
        [DefaultValue(150)]
        [Range(0, 360)]
        public int AdditionalProjectileMaxRad { get; set; } = 150;
        
        [BackgroundColor(60, 60, 70)]
        [DefaultValue(30)]
        [Range(0, 360)]
        public int AdditionalProjectileMinRadMinion { get; set; } = 30;
        
        [BackgroundColor(60, 60, 70)]
        [DefaultValue(360)]
        [Range(0, 360)]
        public int AdditionalProjectileMaxRadMinion { get; set; } = 360;
        
        [BackgroundColor(60, 60, 70)]
        [DefaultValue(100)]
        [Range(1, 1000)]
        public int ProjectilesToMaxRad { get; set; } = 100;
    }
}
