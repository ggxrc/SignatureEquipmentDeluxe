using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SignatureEquipmentDeluxe.Common.Configs
{
    /// <summary>
    /// Configurações de Runas e Maldições
    /// Sistemas especiais de customização de itens
    /// </summary>
    [BackgroundColor(60, 40, 80)]
    public class RuneConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        // ==================== RUNE SYSTEM ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.RuneConfig.RuneSystemHeader")]
        
        [BackgroundColor(60, 40, 80)]
        [DefaultValue(true)]
        public bool EnableRuneSystem { get; set; } = true;
        
        [BackgroundColor(60, 40, 80)]
        [DefaultValue(20)]
        [Range(1, 100)]
        [Slider]
        public int RuneSlot1Level { get; set; } = 20;
        
        [BackgroundColor(60, 40, 80)]
        [DefaultValue(40)]
        [Range(1, 100)]
        [Slider]
        public int RuneSlot2Level { get; set; } = 40;
        
        [BackgroundColor(60, 40, 80)]
        [DefaultValue(60)]
        [Range(1, 100)]
        [Slider]
        public int RuneSlot3Level { get; set; } = 60;
        
        [BackgroundColor(60, 40, 80)]
        [DefaultValue(80)]
        [Range(1, 100)]
        [Slider]
        public int RuneSlot4Level { get; set; } = 80;
        
        [BackgroundColor(60, 40, 80)]
        [DefaultValue(100)]
        [Range(1, 100)]
        [Slider]
        public int RuneSlot5Level { get; set; } = 100;
        
        [BackgroundColor(60, 40, 80)]
        [DefaultValue(0.25f)]
        [Range(0f, 1f)]
        [Increment(0.05f)]
        public float RuneXPPerHitMultiplier { get; set; } = 0.25f;
        
        [BackgroundColor(60, 40, 80)]
        [DefaultValue(0.5f)]
        [Range(0f, 2f)]
        [Increment(0.1f)]
        public float RuneXPPerKillMultiplier { get; set; } = 0.5f;
        
        // ==================== CURSE SYSTEM ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.RuneConfig.CurseSystemHeader")]
        
        [BackgroundColor(80, 40, 40)]
        [DefaultValue(true)]
        public bool EnableCurseSystem { get; set; } = true;
        
        [BackgroundColor(80, 40, 40)]
        [DefaultValue(0.25f)]
        [Range(0f, 1f)]
        [Increment(0.05f)]
        public float CurseXPBonusPerHit { get; set; } = 0.25f;
        
        [BackgroundColor(80, 40, 40)]
        [DefaultValue(0.5f)]
        [Range(0f, 2f)]
        [Increment(0.1f)]
        public float CurseXPBonusPerKill { get; set; } = 0.5f;
        
        [BackgroundColor(80, 40, 40)]
        [DefaultValue(0.05f)]
        [Range(0f, 0.5f)]
        [Increment(0.01f)]
        public float CurseDropChancePerCurse { get; set; } = 0.05f;
        
        [BackgroundColor(80, 40, 40)]
        [DefaultValue(0.2f)]
        [Range(0f, 1f)]
        [Increment(0.05f)]
        public float CurseRemovalLevelLossChance { get; set; } = 0.2f;
        
        [BackgroundColor(80, 40, 40)]
        [DefaultValue(0.125f)]
        [Range(0.01f, 0.5f)]
        [Increment(0.025f)]
        public float CurseRemovalLevelLossFraction { get; set; } = 0.125f;
        
        // ==================== ELEMENTAL EFFECTS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.RuneConfig.ElementalEffectsHeader")]
        
        [BackgroundColor(80, 50, 40)]
        [DefaultValue(true)]
        public bool EnableElementalTrailEffects { get; set; } = true;
        
        [BackgroundColor(80, 50, 40)]
        [DefaultValue(true)]
        public bool EnableElementalDoTEffects { get; set; } = true;
        
        [BackgroundColor(80, 50, 40)]
        [DefaultValue(1.0f)]
        [Range(0.1f, 10f)]
        [Increment(0.1f)]
        public float FireDoTDamagePerLevel { get; set; } = 1.0f;
        
        [BackgroundColor(80, 50, 40)]
        [DefaultValue(0.5f)]
        [Range(0.1f, 10f)]
        [Increment(0.1f)]
        public float IceDoTDamagePerLevel { get; set; } = 0.5f;
        
        [BackgroundColor(80, 50, 40)]
        [DefaultValue(0.75f)]
        [Range(0.1f, 10f)]
        [Increment(0.1f)]
        public float PoisonDoTDamagePerLevel { get; set; } = 0.75f;
        
        [BackgroundColor(80, 50, 40)]
        [DefaultValue(1.25f)]
        [Range(0.1f, 10f)]
        [Increment(0.1f)]
        public float LightningDoTDamagePerLevel { get; set; } = 1.25f;
        
        [BackgroundColor(80, 50, 40)]
        [DefaultValue(180)]
        [Range(30, 600)]
        [Slider]
        public int FireDoTDuration { get; set; } = 180;
        
        [BackgroundColor(80, 50, 40)]
        [DefaultValue(240)]
        [Range(30, 600)]
        [Slider]
        public int IceDoTDuration { get; set; } = 240;
        
        [BackgroundColor(80, 50, 40)]
        [DefaultValue(300)]
        [Range(30, 600)]
        [Slider]
        public int PoisonDoTDuration { get; set; } = 300;
        
        [BackgroundColor(80, 50, 40)]
        [DefaultValue(120)]
        [Range(30, 600)]
        [Slider]
        public int LightningDoTDuration { get; set; } = 120;
    }
}
