using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Progression.Common.Configs
{
    /// <summary>
    /// Core Gameplay Settings
    /// Control equipment mechanics and progression
    /// </summary>
    public class GameplayConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        // ==================== ADDITIONAL PROJECTILE CONFIG ====================

        [Header("$Mods.Progression.Config.GameplayConfig.AdditionalProjectileHeader")]
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
