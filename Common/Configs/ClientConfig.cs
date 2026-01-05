using System.ComponentModel;
using Terraria.ModLoader.Config;
using Progression.Common.Systems;
using Microsoft.Xna.Framework;

namespace Progression.Common.Configs
{
    /// <summary>
    /// Configurações Visuais e de Interface
    /// Personalize como as informações são exibidas
    /// </summary>    [BackgroundColor(25, 35, 50)]
    public class ClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        // ==================== TOOLTIP DISPLAY ====================
        
        [Header("$Mods.Progression.Config.ClientConfig.TooltipDisplayHeader")]        [BackgroundColor(70, 90, 120)]
        [DefaultValue(true)]
        public bool ShowItemLevel { get; set; }        [BackgroundColor(65, 85, 115)]
        [DefaultValue(true)]
        public bool ShowItemExperience { get; set; }        [BackgroundColor(60, 80, 110)]
        [DefaultValue(true)]
        public bool ShowItemStats { get; set; }        [BackgroundColor(55, 75, 105)]
        [DefaultValue(true)]
        public bool ShowDetailedStats { get; set; }        [BackgroundColor(50, 70, 100)]
        [DefaultValue(true)]
        public bool ShowStatChanges { get; set; }        [BackgroundColor(45, 65, 95)]
        [DefaultValue(true)]
        public bool ShowCappedStats { get; set; }

        // ==================== ITEM OUTLINE ====================
        
        [Header("$Mods.Progression.Config.ClientConfig.OutlineHeader")]        [BackgroundColor(90, 70, 110)]
        [DefaultValue(true)]
        public bool EnableOutline { get; set; }        [BackgroundColor(85, 65, 105)]
        [DefaultValue(OutlineMode.Scale)]
        public OutlineMode OutlineMode { get; set; }        [BackgroundColor(80, 60, 100)]
        [DefaultValue(1f)]
        [Range(0.5f, 3f)]
        [Increment(0.1f)]
        public float OutlineThickness { get; set; }        [BackgroundColor(75, 55, 95)]
        [DefaultValue(0.8f)]
        [Range(0f, 1f)]
        [Increment(0.05f)]
        public float OutlineOpacity { get; set; }

        // ==================== OUTLINE COLORS BY LEVEL ====================
        
        [Header("$Mods.Progression.Config.ClientConfig.OutlineColorsHeader")]        [BackgroundColor(85, 85, 85)]
        [DefaultValue(typeof(Color), "150, 150, 150, 255")]
        public Color OutlineColor_Level1_25 { get; set; }        [BackgroundColor(60, 100, 60)]
        [DefaultValue(typeof(Color), "100, 200, 100, 255")]
        public Color OutlineColor_Level26_50 { get; set; }        [BackgroundColor(50, 75, 120)]
        [DefaultValue(typeof(Color), "100, 150, 255, 255")]
        public Color OutlineColor_Level51_75 { get; set; }        [BackgroundColor(100, 50, 120)]
        [DefaultValue(typeof(Color), "200, 100, 255, 255")]
        public Color OutlineColor_Level76_100 { get; set; }        [BackgroundColor(120, 90, 50)]
        [DefaultValue(typeof(Color), "255, 200, 100, 255")]
        public Color OutlineColor_Level101Plus { get; set; }

        // ==================== TOOLTIP CUSTOMIZATION ====================
        
        [Header("$Mods.Progression.Config.ClientConfig.TooltipCustomHeader")]        [BackgroundColor(50, 80, 70)]
        [DefaultValue(true)]
        public bool ShowTooltipSeparator { get; set; }        [BackgroundColor(45, 75, 65)]
        [DefaultValue(true)]
        public bool ShowNextLevelInfo { get; set; }        [BackgroundColor(40, 70, 60)]
        [DefaultValue(true)]
        public bool ShowProgressBar { get; set; }        [BackgroundColor(120, 100, 20)]
        [DefaultValue(typeof(Color), "255, 215, 0, 255")]
        public Color TooltipLevelColor { get; set; }        [BackgroundColor(50, 120, 50)]
        [DefaultValue(typeof(Color), "100, 255, 100, 255")]
        public Color TooltipExpColor { get; set; }        [BackgroundColor(70, 90, 120)]
        [DefaultValue(typeof(Color), "150, 200, 255, 255")]
        public Color TooltipStatColor { get; set; }

        // ==================== NOTIFICATIONS ====================
        
        [Header("$Mods.Progression.Config.ClientConfig.NotificationsHeader")]        [BackgroundColor(120, 80, 40)]
        [DefaultValue(true)]
        public bool ShowLevelUpNotification { get; set; }        [BackgroundColor(115, 75, 35)]
        [DefaultValue(true)]
        public bool ShowExpGainNotification { get; set; }        [BackgroundColor(110, 70, 30)]
        [DefaultValue(3f)]
        [Range(1f, 10f)]
        [Increment(0.5f)]
        public float NotificationDuration { get; set; }        [BackgroundColor(105, 65, 25)]
        [DefaultValue(1f)]
        [Range(0.5f, 2f)]
        [Increment(0.1f)]
        public float NotificationScale { get; set; }

        // ==================== PERFORMANCE ====================
        
        [Header("$Mods.Progression.Config.ClientConfig.PerformanceHeader")]        [BackgroundColor(60, 70, 70)]
        [DefaultValue(true)]
        public bool EnableParticleEffects { get; set; }        [BackgroundColor(55, 65, 65)]
        [DefaultValue(true)]
        public bool EnableGlowEffects { get; set; }        [BackgroundColor(50, 60, 60)]
        [DefaultValue(60)]
        [Range(30, 120)]
        public int MaxParticlesPerSecond { get; set; }        [BackgroundColor(45, 55, 55)]
        [DefaultValue(false)]
        public bool ReducedAnimations { get; set; }
        
        // ==================== AURA EFFECTS ====================
        
        [Header("$Mods.Progression.Config.ClientConfig.AuraEffectsHeader")]        [BackgroundColor(100, 60, 100)]
        [DefaultValue(true)]
        public bool EnableAuraEffects { get; set; }        [BackgroundColor(95, 55, 95)]
        [DefaultValue(AuraStyle.Magic)]
        public AuraStyle AuraStyle { get; set; }        [BackgroundColor(90, 50, 90)]
        [Range(0.1f, 2f)]
        [Increment(0.1f)]
        [DefaultValue(0.5f)]
        public float AuraIntensityMultiplier { get; set; }        [BackgroundColor(85, 45, 85)]
        [DefaultValue(25)]
        [Range(1, 200)]
        public int AuraLevel_Weak { get; set; }        [BackgroundColor(80, 40, 80)]
        [DefaultValue(50)]
        [Range(1, 200)]
        public int AuraLevel_Medium { get; set; }        [BackgroundColor(75, 35, 75)]
        [DefaultValue(75)]
        [Range(1, 200)]
        public int AuraLevel_Strong { get; set; }        [BackgroundColor(70, 30, 70)]
        [DefaultValue(100)]
        [Range(1, 200)]
        public int AuraLevel_Intense { get; set; }
        
        // === PROJECTILE HIT EFFECTS ===        [BackgroundColor(60, 60, 90)]
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
        public bool ShowStreakEndNotification { get; set; }        [BackgroundColor(100, 80, 50)]
        [DefaultValue(true)]
        public bool EnableKillStreakSounds { get; set; }        [BackgroundColor(95, 75, 45)]
        [DefaultValue(true)]
        public bool EnableKillStreakParticles { get; set; }        [BackgroundColor(90, 70, 40)]
        [DefaultValue(true)]
        public bool ShowKillStreakHUD { get; set; }        [BackgroundColor(85, 65, 35)]
        [DefaultValue(true)]
        public bool ShowKillStreakTimer { get; set; }        [BackgroundColor(80, 60, 30)]
        [DefaultValue(typeof(Vector2), "0, 0")]
        public Vector2 KillStreakHUDOffset { get; set; }
        
        // === INVENTORY GLOW ===        [BackgroundColor(90, 90, 60)]
        [DefaultValue(true)]
        public bool EnableInventoryGlow { get; set; }        [BackgroundColor(85, 85, 55)]
        [DefaultValue(50)]
        [Range(1, 200)]
        public int InventoryGlowMinLevel { get; set; }
        
        // === EVENT VISUALS ===        [BackgroundColor(60, 90, 70)]
        [DefaultValue(true)]
        public bool ShowEventVisuals { get; set; }        [BackgroundColor(55, 85, 65)]
        [DefaultValue(true)]
        public bool ShowEventStacking { get; set; }
        
        // === MILESTONES ===        [BackgroundColor(100, 80, 100)]
        [DefaultValue(true)]
        public bool ShowMilestoneEffects { get; set; }

        // ==================== ADVANCED ====================
        
        [Header("$Mods.Progression.Config.ClientConfig.AdvancedHeader")]        [BackgroundColor(70, 50, 50)]
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


