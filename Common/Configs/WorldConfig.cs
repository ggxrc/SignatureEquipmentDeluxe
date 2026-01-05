using System.ComponentModel;
using Progression.Common.Systems;
using Terraria.ModLoader.Config;

namespace Progression.Common.Configs
{
    /// <summary>
    /// World Systems Configuration
    /// Affects the entire world, not just the player
    /// </summary>
    [BackgroundColor(40, 80, 40)]
    public class WorldConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        // ==================== LEVELED ENEMY SYSTEM ====================

        [Header("$Mods.Progression.Config.WorldConfig.LeveledEnemyHeader")]
        [BackgroundColor(40, 80, 40)]
        [DefaultValue(true)]
        public bool EnableLeveledEnemies { get; set; } = true;

        [BackgroundColor(40, 80, 40)]
        [DefaultValue(15)]
        [Range(1, 100)]
        [Increment(5)]
        [Slider]
        public int LeveledEnemySpawnChance { get; set; } = 15;

        // ==================== WORLD PROGRESSION MODE ====================

        [Header("$Mods.Progression.Config.WorldConfig.WorldProgressionHeader")]
        [BackgroundColor(50, 80, 50)]
        [DefaultValue(WorldLevelMode.BossProgression)]
        public WorldLevelMode WorldLevelMode { get; set; } = WorldLevelMode.BossProgression;

        // ==================== LEVEL CAPS BY PHASE ====================

        [Header("$Mods.Progression.Config.WorldConfig.LevelCapsHeader")]
        [BackgroundColor(50, 80, 50)]
        [Range(1, 1000)]
        [DefaultValue(50)]
        public int PreHardmodeMaxLevel { get; set; } = 50;

        [BackgroundColor(50, 80, 50)]
        [Range(1, 1000)]
        [DefaultValue(100)]
        public int HardmodeMaxLevel { get; set; } = 100;

        [BackgroundColor(50, 80, 50)]
        [Range(1, 1000)]
        [DefaultValue(150)]
        public int PostMoonLordMaxLevel { get; set; } = 150;

        // ==================== SPAWN VARIANCE ====================

        [Header("$Mods.Progression.Config.WorldConfig.SpawnVarianceHeader")]
        [BackgroundColor(60, 80, 60)]
        [DefaultValue(2)]
        [Range(0, 10)]
        [Slider]
        public int LevelVariance { get; set; } = 2;

        // ==================== ENEMY REWARDS ====================

        [Header("$Mods.Progression.Config.WorldConfig.EnemyRewardsHeader")]
        [BackgroundColor(40, 80, 40)]
        [DefaultValue(5)]
        [Range(1, 10)]
        [Slider]
        public int LeveledEnemyXPBonusPerLevel { get; set; } = 5;

        /// <summary>
        /// Validates that phase caps are in ascending order
        /// </summary>
        public override void OnChanged()
        {
            // Ensures Hardmode >= PreHardmode
            if (HardmodeMaxLevel < PreHardmodeMaxLevel)
            {
                HardmodeMaxLevel = PreHardmodeMaxLevel;
            }

            // Ensures PostMoonLord >= Hardmode
            if (PostMoonLordMaxLevel < HardmodeMaxLevel)
            {
                PostMoonLordMaxLevel = HardmodeMaxLevel;
            }
        }
    }
}
