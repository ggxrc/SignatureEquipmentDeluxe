using System.ComponentModel;
using Terraria.ModLoader.Config;
using SignatureEquipmentDeluxe.Common.Systems;
using Microsoft.Xna.Framework;

namespace SignatureEquipmentDeluxe.Common.Configs
{
    [BackgroundColor(20, 30, 40)]
    public class ClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        // ==================== VISUAL SETTINGS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ClientConfig.VisualHeader")]
        
        [BackgroundColor(40, 50, 60)]
        [DefaultValue(true)]
        public bool ShowItemLevel { get; set; }

        [BackgroundColor(40, 50, 60)]
        [DefaultValue(true)]
        public bool ShowItemExperience { get; set; }

        [BackgroundColor(40, 50, 60)]
        [DefaultValue(true)]
        public bool ShowItemStats { get; set; }

        [BackgroundColor(40, 50, 60)]
        [DefaultValue(true)]
        public bool ShowDetailedStats { get; set; }

        [BackgroundColor(40, 50, 60)]
        [DefaultValue(true)]
        public bool ShowStatChanges { get; set; }

        [BackgroundColor(40, 50, 60)]
        [DefaultValue(true)]
        public bool ShowCappedStats { get; set; }

        // ==================== OUTLINE SETTINGS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ClientConfig.OutlineHeader")]
        
        [BackgroundColor(50, 40, 60)]
        [DefaultValue(true)]
        public bool EnableOutline { get; set; }

        [BackgroundColor(50, 40, 60)]
        [DefaultValue(OutlineMode.Scale)]
        public OutlineMode OutlineMode { get; set; }

        [BackgroundColor(50, 40, 60)]
        [DefaultValue(1f)]
        [Range(0.5f, 3f)]
        [Increment(0.1f)]
        public float OutlineThickness { get; set; }

        [BackgroundColor(50, 40, 60)]
        [DefaultValue(0.8f)]
        [Range(0f, 1f)]
        [Increment(0.05f)]
        public float OutlineOpacity { get; set; }

        // ==================== OUTLINE COLORS BY LEVEL ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ClientConfig.OutlineColorsHeader")]
        
        [BackgroundColor(60, 50, 50)]
        [DefaultValue(typeof(Color), "150, 150, 150, 255")]
        public Color OutlineColor_Level1_25 { get; set; }

        [BackgroundColor(60, 60, 50)]
        [DefaultValue(typeof(Color), "100, 200, 100, 255")]
        public Color OutlineColor_Level26_50 { get; set; }

        [BackgroundColor(50, 60, 60)]
        [DefaultValue(typeof(Color), "100, 150, 255, 255")]
        public Color OutlineColor_Level51_75 { get; set; }

        [BackgroundColor(60, 50, 60)]
        [DefaultValue(typeof(Color), "200, 100, 255, 255")]
        public Color OutlineColor_Level76_100 { get; set; }

        [BackgroundColor(70, 60, 50)]
        [DefaultValue(typeof(Color), "255, 200, 100, 255")]
        public Color OutlineColor_Level101Plus { get; set; }

        // ==================== TOOLTIP SETTINGS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ClientConfig.TooltipHeader")]
        
        [BackgroundColor(40, 60, 50)]
        [DefaultValue(true)]
        public bool ShowTooltipSeparator { get; set; }

        [BackgroundColor(40, 60, 50)]
        [DefaultValue(true)]
        public bool ShowNextLevelInfo { get; set; }

        [BackgroundColor(40, 60, 50)]
        [DefaultValue(true)]
        public bool ShowProgressBar { get; set; }

        [BackgroundColor(40, 60, 50)]
        [DefaultValue(typeof(Color), "255, 215, 0, 255")]
        public Color TooltipLevelColor { get; set; }

        [BackgroundColor(40, 60, 50)]
        [DefaultValue(typeof(Color), "100, 255, 100, 255")]
        public Color TooltipExpColor { get; set; }

        [BackgroundColor(40, 60, 50)]
        [DefaultValue(typeof(Color), "150, 200, 255, 255")]
        public Color TooltipStatColor { get; set; }

        // ==================== UI SETTINGS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ClientConfig.UIHeader")]
        
        [BackgroundColor(50, 50, 40)]
        [DefaultValue(true)]
        public bool EnableManagementUI { get; set; }

        [BackgroundColor(50, 50, 40)]
        [DefaultValue(0.9f)]
        [Range(0.5f, 1f)]
        [Increment(0.05f)]
        public float UIScale { get; set; }

        [BackgroundColor(50, 50, 40)]
        [DefaultValue(0.95f)]
        [Range(0.5f, 1f)]
        [Increment(0.05f)]
        public float UIOpacity { get; set; }

        [BackgroundColor(50, 50, 40)]
        [DefaultValue(typeof(Color), "30, 30, 50, 240")]
        public Color UIBackgroundColor { get; set; }

        [BackgroundColor(50, 50, 40)]
        [DefaultValue(typeof(Color), "100, 150, 255, 255")]
        public Color UIAccentColor { get; set; }

        // ==================== NOTIFICATIONS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ClientConfig.NotificationsHeader")]
        
        [BackgroundColor(60, 40, 50)]
        [DefaultValue(true)]
        public bool ShowLevelUpNotification { get; set; }

        [BackgroundColor(60, 40, 50)]
        [DefaultValue(true)]
        public bool ShowExpGainNotification { get; set; }

        [BackgroundColor(60, 40, 50)]
        [DefaultValue(3f)]
        [Range(1f, 10f)]
        [Increment(0.5f)]
        public float NotificationDuration { get; set; }

        [BackgroundColor(60, 40, 50)]
        [DefaultValue(1f)]
        [Range(0.5f, 2f)]
        [Increment(0.1f)]
        public float NotificationScale { get; set; }

        // ==================== PERFORMANCE ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ClientConfig.PerformanceHeader")]
        
        [BackgroundColor(40, 50, 50)]
        [DefaultValue(true)]
        public bool EnableParticleEffects { get; set; }

        [BackgroundColor(40, 50, 50)]
        [DefaultValue(true)]
        public bool EnableGlowEffects { get; set; }

        [BackgroundColor(40, 50, 50)]
        [DefaultValue(60)]
        [Range(30, 120)]
        public int MaxParticlesPerSecond { get; set; }

        [BackgroundColor(40, 50, 50)]
        [DefaultValue(false)]
        public bool ReducedAnimations { get; set; }

        // ==================== ADVANCED ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ClientConfig.AdvancedHeader")]
        
        [BackgroundColor(50, 40, 40)]
        [DefaultValue(false)]
        public bool ShowDebugInfo { get; set; }

        [BackgroundColor(50, 40, 40)]
        [DefaultValue(false)]
        public bool ShowStatCalculations { get; set; }

        [BackgroundColor(50, 40, 40)]
        [DefaultValue(false)]
        public bool LogStatChanges { get; set; }
    }
}
