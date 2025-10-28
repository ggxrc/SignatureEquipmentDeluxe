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
        
        // ==================== VISUAL EFFECTS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ClientConfig.VisualEffectsHeader")]
        
        // === AURAS ===
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(true)]
        public bool EnableAuraEffects { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(AuraStyle.Magic)]
        public AuraStyle AuraStyle { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [Range(0.1f, 2f)]
        [Increment(0.1f)]
        [DefaultValue(0.5f)]
        public float AuraIntensityMultiplier { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(25)]
        [Range(1, 200)]
        public int AuraLevel_Weak { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(50)]
        [Range(1, 200)]
        public int AuraLevel_Medium { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(75)]
        [Range(1, 200)]
        public int AuraLevel_Strong { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(100)]
        [Range(1, 200)]
        public int AuraLevel_Intense { get; set; }
        
        // === PROJECTILE HIT EFFECTS ===
        
        [BackgroundColor(40, 40, 60)]
        [DefaultValue(true)]
        public bool EnableProjectileHitEffects { get; set; }
        
        [BackgroundColor(40, 40, 60)]
        [DefaultValue(10)]
        [Range(1, 200)]
        public int HitEffectsMinLevel { get; set; }
        
        [BackgroundColor(40, 40, 60)]
        [DefaultValue(1f)]
        [Range(0.5f, 3f)]
        [Increment(0.1f)]
        public float HitEffectsIntensity { get; set; }
        
        // === COMBO VISUALS ===
        
        [BackgroundColor(60, 40, 60)]
        [Range(5, 50)]
        [DefaultValue(5)]
        public int ComboVisualInterval { get; set; }
        
        [BackgroundColor(60, 40, 60)]
        [DefaultValue(true)]
        public bool EnableComboSounds { get; set; }
        
        [BackgroundColor(60, 40, 60)]
        [DefaultValue(true)]
        public bool EnableComboParticles { get; set; }
        
        // === KILL STREAK VISUALS ===
        
        [BackgroundColor(60, 50, 40)]
        [Range(5, 50)]
        [DefaultValue(10)]
        public int KillStreakVisualInterval { get; set; }
        
        [BackgroundColor(60, 50, 40)]
        [DefaultValue(true)]
        public bool ShowStreakEndNotification { get; set; }
        
        [BackgroundColor(60, 50, 40)]
        [DefaultValue(true)]
        public bool EnableKillStreakSounds { get; set; }
        
        [BackgroundColor(60, 50, 40)]
        [DefaultValue(true)]
        public bool EnableKillStreakParticles { get; set; }
        
        [BackgroundColor(60, 50, 40)]
        [DefaultValue(true)]
        public bool ShowKillStreakHUD { get; set; }
        
        [BackgroundColor(60, 50, 40)]
        [DefaultValue(true)]
        public bool ShowKillStreakTimer { get; set; }
        
        [BackgroundColor(60, 50, 40)]
        [DefaultValue(typeof(Vector2), "0, 0")]
        public Vector2 KillStreakHUDOffset { get; set; }
        
        // === INVENTORY GLOW ===
        
        [BackgroundColor(50, 50, 40)]
        [DefaultValue(true)]
        public bool EnableInventoryGlow { get; set; }
        
        [BackgroundColor(50, 50, 40)]
        [DefaultValue(50)]
        [Range(1, 200)]
        public int InventoryGlowMinLevel { get; set; }
        
        // === EVENT VISUALS ===
        
        [BackgroundColor(40, 60, 50)]
        [DefaultValue(true)]
        public bool ShowEventVisuals { get; set; }
        
        [BackgroundColor(40, 60, 50)]
        [DefaultValue(true)]
        public bool ShowEventStacking { get; set; }
        
        // === MILESTONES ===
        
        [BackgroundColor(60, 50, 60)]
        [DefaultValue(true)]
        public bool ShowMilestoneEffects { get; set; }

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
