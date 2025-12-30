using System.ComponentModel;
using Terraria.ModLoader.Config;
using SignatureEquipmentDeluxe.Common.Systems;

namespace SignatureEquipmentDeluxe.Common.Configs
{
    /// <summary>
    /// Configurações de Sistemas de Mundo
    /// Afetam o mundo todo, não apenas o jogador
    /// </summary>
    [BackgroundColor(40, 80, 40)]
    public class WorldConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        // ==================== LEVELED ENEMY SYSTEM ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.WorldConfig.LeveledEnemyHeader")]
        
        [BackgroundColor(40, 80, 40)]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool EnableLeveledEnemies { get; set; } = true;
        
        [BackgroundColor(40, 80, 40)]
        [DefaultValue(0.15f)]
        [Range(0f, 1f)]
        [Increment(0.05f)]
        public float LeveledEnemySpawnChance { get; set; } = 0.15f;
        
        // ==================== WORLD PROGRESSION MODE ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.WorldConfig.WorldProgressionHeader")]
        
        [BackgroundColor(50, 80, 50)]
        [DefaultValue(WorldLevelMode.BossProgression)]
        public WorldLevelMode WorldLevelMode { get; set; } = WorldLevelMode.BossProgression;
        
        // ==================== LEVEL CAPS BY PHASE ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.WorldConfig.LevelCapsHeader")]
        
        [BackgroundColor(50, 80, 50)]
        [DefaultValue(50)]
        [Range(1, 500)]
        [Slider]
        public int PreHardmodeMaxLevel { get; set; } = 50;
        
        [BackgroundColor(50, 80, 50)]
        [DefaultValue(100)]
        [Range(1, 500)]
        [Slider]
        public int HardmodeMaxLevel { get; set; } = 100;
        
        [BackgroundColor(50, 80, 50)]
        [DefaultValue(150)]
        [Range(1, 1000)]
        [Slider]
        public int PostMoonLordMaxLevel { get; set; } = 150;
        
        // ==================== SPAWN VARIANCE ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.WorldConfig.SpawnVarianceHeader")]
        
        [BackgroundColor(60, 80, 60)]
        [DefaultValue(2)]
        [Range(0, 50)]
        [Slider]
        public int LevelVariance { get; set; } = 2;
        
        // ==================== ENEMY REWARDS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.WorldConfig.EnemyRewardsHeader")]
        
        [BackgroundColor(40, 80, 40)]
        [DefaultValue(0.05f)]
        [Range(0f, 0.5f)]
        [Increment(0.01f)]
        public float LeveledEnemyXPBonusPerLevel { get; set; } = 0.05f;
    }
}
