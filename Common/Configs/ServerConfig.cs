using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;
using SignatureEquipmentDeluxe.Common.Systems;
using Newtonsoft.Json;

namespace SignatureEquipmentDeluxe.Common.Configs
{
    [BackgroundColor(30, 30, 40)]
    public class ServerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        // ==================== EXPERIÊNCIA GERAL ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.GeneralExperienceHeader")]
        
        [BackgroundColor(50, 50, 60)]
        [DefaultValue(1f)]
        public float GlobalExpMultiplier { get; set; }
        
        [BackgroundColor(50, 50, 60)]
        [DefaultValue(100)]
        public int BaseExpPerLevel { get; set; }

        [BackgroundColor(50, 50, 60)]
        [DefaultValue(1.1f)]
        [Increment(0.05f)]
        public float ExpScalingFactor { get; set; }
        
        [BackgroundColor(50, 50, 60)]
        [DefaultValue(false)]
        public bool AllowStatueXP { get; set; }
        
        [BackgroundColor(50, 50, 60)]
        [DefaultValue(1f)]
        public float BossExpMultiplier { get; set; }
        
        // ==================== EXPERIÊNCIA - ARMAS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.WeaponExperienceHeader")]
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(1f)]
        public float WeaponExpMultiplier { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(5f)]
        public float WeaponBaseXPPerHit { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(50f)]
        public float WeaponBaseXPPerKill { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(0.01f)]
        public float WeaponXPPerDamageDealt { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(0.1f)]
        public float WeaponXPPerEnemyMaxHP { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(0)]
        public int WeaponMaxLevel { get; set; }

        // ==================== EXPERIÊNCIA - ARMADURAS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.ArmorExperienceHeader")]
        
        [BackgroundColor(40, 40, 60)]
        [DefaultValue(1f)]
        public float ArmorExpMultiplier { get; set; }
        
        [BackgroundColor(40, 40, 60)]
        [DefaultValue(1f)]
        public float ArmorXPPerDamageReceived { get; set; }
        
        [BackgroundColor(40, 40, 60)]
        [DefaultValue(false)]
        public bool ArmorXPIgnoreDefense { get; set; }
        
        [BackgroundColor(40, 40, 60)]
        [DefaultValue(0.5f)]
        public float ArmorXPPerDamageBlocked { get; set; }
        
        [BackgroundColor(40, 40, 60)]
        [DefaultValue(10f)]
        public float ArmorXPPerDodge { get; set; }
        
        [BackgroundColor(40, 40, 60)]
        [DefaultValue(0)]
        public int ArmorMaxLevel { get; set; }

        // ==================== EXPERIÊNCIA - ACESSÓRIOS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.AccessoryExperienceHeader")]
        
        [BackgroundColor(40, 60, 40)]
        [DefaultValue(1f)]
        public float AccessoryExpMultiplier { get; set; }
        
        [BackgroundColor(40, 60, 40)]
        [DefaultValue(0.1f)]
        public float AccessoryXPPerSecondEquipped { get; set; }
        
        [BackgroundColor(40, 60, 40)]
        [DefaultValue(2f)]
        public float AccessoryXPPerHit { get; set; }
        
        [BackgroundColor(40, 60, 40)]
        [DefaultValue(5f)]
        public float AccessoryXPPerKill { get; set; }
        
        [BackgroundColor(40, 60, 40)]
        [DefaultValue(1f)]
        public float AccessoryXPPerDamageTaken { get; set; }
        
        [BackgroundColor(40, 60, 40)]
        [DefaultValue(0)]
        public int AccessoryMaxLevel { get; set; }

        // ==================== STATS - ARMAS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.WeaponStatsHeader")]
        
        [BackgroundColor(60, 40, 40)]
        public ItemStatInt WeaponDamage { get; set; } = new ItemStatInt
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>
            {
                new ScalingTier { StartLevel = 1, PerLevel = 1f, PerLevelMult = 1 },
                new ScalingTier { StartLevel = 50, PerLevel = 2f, PerLevelMult = 1 },
                new ScalingTier { StartLevel = 100, PerLevel = 5f, PerLevelMult = 1 }
            }
        };

        [BackgroundColor(60, 40, 40)]
        public ItemStatInt WeaponCritChance { get; set; } = new ItemStatInt
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };
        
        [BackgroundColor(60, 40, 40)]
        public ItemStatFloat WeaponUseSpeed { get; set; } = new ItemStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1f,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };

        [BackgroundColor(60, 40, 40)]
        public ItemStatFloat WeaponMeleeSpeed { get; set; } = new ItemStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1f,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };

        [BackgroundColor(60, 40, 40)]
        public ItemStatFloat WeaponMeleeSize { get; set; } = new ItemStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1f,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };

        [BackgroundColor(60, 40, 40)]
        public ProjectileStatFloat WeaponProjectileSize { get; set; } = new ProjectileStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1f,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };

        [BackgroundColor(60, 40, 40)]
        public ProjectileStatFloat WeaponProjectileSpeed { get; set; } = new ProjectileStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1f,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };

        [BackgroundColor(60, 40, 40)]
        public ProjectileStatFloat WeaponProjectileLifeTime { get; set; } = new ProjectileStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1f,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };

        [BackgroundColor(60, 40, 40)]
        public ProjectileStatFloat WeaponProjectilePenetration { get; set; } = new ProjectileStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1f,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };
        
        [BackgroundColor(60, 40, 40)]
        public ItemStatFloat WeaponManaCostReduction { get; set; } = new ItemStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1f,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };

        [BackgroundColor(60, 40, 40)]
        public ItemStatFloat WeaponAmmoConsumptionReduction { get; set; } = new ItemStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1f,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };

        // ==================== STATS - ARMADURAS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.ArmorStatsHeader")]
        
        [BackgroundColor(40, 40, 60)]
        public ItemStatInt ArmorDefense { get; set; } = new ItemStatInt
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };

        // ==================== STATS - ACESSÓRIOS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.AccessoryStatsHeader")]
        
        [BackgroundColor(40, 60, 40)]
        public ItemStatInt AccessoryDamage { get; set; } = new ItemStatInt
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };
        
        [BackgroundColor(40, 60, 40)]
        public ItemStatInt AccessoryDefense { get; set; } = new ItemStatInt
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };
        
        [BackgroundColor(40, 60, 40)]
        public ItemStatInt AccessoryCritChance { get; set; } = new ItemStatInt
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };

        [BackgroundColor(40, 60, 40)]
        public ItemStatFloat AccessoryMinionSlotReduction { get; set; } = new ItemStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 0.1f,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };

        // ==================== BLACKLISTS & WHITELISTS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.ListsHeader")]
        
        [BackgroundColor(60, 50, 40)]
        public HashSet<ItemDefinition> GlobalItemBlacklist { get; set; } = new HashSet<ItemDefinition>();

        [BackgroundColor(60, 50, 40)]
        public HashSet<NPCDefinition> NPCBlacklist { get; set; } = new HashSet<NPCDefinition>();

        [BackgroundColor(60, 50, 40)]
        public List<ItemProjectileReference> SpecialProjectileMapping { get; set; } = new List<ItemProjectileReference>();

        // ==================== PROJECTILE IMMUNITY ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.ImmunityHeader")]
        
        [BackgroundColor(50, 40, 60)]
        [DefaultValue(LocalNPCImmunityMode.Disabled)]
        public LocalNPCImmunityMode LocalImmunityMode { get; set; }

        [BackgroundColor(50, 40, 60)]
        public List<ProjectileLocalFrames> ProjectileLocalFramesOverride { get; set; } = new List<ProjectileLocalFrames>();

        // ==================== ADVANCED ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.AdvancedHeader")]
        
        [BackgroundColor(40, 40, 50)]
        [DefaultValue(true)]
        public bool EnableMultiplayerSync { get; set; }

        [BackgroundColor(40, 40, 50)]
        [DefaultValue(true)]
        public bool EnableProjectileStatCaching { get; set; }

        [BackgroundColor(40, 40, 50)]
        [DefaultValue(false)]
        public bool DebugMode { get; set; }
        
        // ==================== LEVEL LIMITS (INDIVIDUAL) ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.IndividualLevelLimitsHeader")]
        
        [BackgroundColor(50, 60, 50)]
        public Dictionary<ItemDefinition, int> IndividualMaxLevel { get; set; } = new Dictionary<ItemDefinition, int>();
        
        // ==================== COMPATIBILIDADE (LEGACY) ====================
        // Properties antigas mantidas apenas para compatibilidade de código
        // [JsonIgnore] esconde da UI de configuração
        
        [JsonIgnore]
        public float BaseXPPerHit
        {
            get => WeaponBaseXPPerHit;
            set => WeaponBaseXPPerHit = value;
        }
        
        [JsonIgnore]
        public float BaseXPPerKill
        {
            get => WeaponBaseXPPerKill;
            set => WeaponBaseXPPerKill = value;
        }
        
        [JsonIgnore]
        public float XPPerDamageDealt
        {
            get => WeaponXPPerDamageDealt;
            set => WeaponXPPerDamageDealt = value;
        }
        
        [JsonIgnore]
        public float XPPerEnemyMaxHP
        {
            get => WeaponXPPerEnemyMaxHP;
            set => WeaponXPPerEnemyMaxHP = value;
        }
        
        [JsonIgnore]
        public int GlobalMaxLevel
        {
            get
            {
                // Retorna o menor max level não-zero, ou 0 se todos forem 0
                int min = int.MaxValue;
                if (WeaponMaxLevel > 0 && WeaponMaxLevel < min) min = WeaponMaxLevel;
                if (ArmorMaxLevel > 0 && ArmorMaxLevel < min) min = ArmorMaxLevel;
                if (AccessoryMaxLevel > 0 && AccessoryMaxLevel < min) min = AccessoryMaxLevel;
                return min == int.MaxValue ? 0 : min;
            }
            set
            {
                // Aplica o mesmo valor para todos
                WeaponMaxLevel = value;
                ArmorMaxLevel = value;
                AccessoryMaxLevel = value;
            }
        }
        
        [JsonIgnore]
        public ItemStatInt Damage
        {
            get => WeaponDamage;
            set => WeaponDamage = value;
        }
        
        [JsonIgnore]
        public ItemStatInt CritChance
        {
            get => WeaponCritChance;
            set => WeaponCritChance = value;
        }
        
        [JsonIgnore]
        public ItemStatInt Defense
        {
            get => ArmorDefense;
            set => ArmorDefense = value;
        }
        
        [JsonIgnore]
        public ItemStatFloat UseSpeed
        {
            get => WeaponUseSpeed;
            set => WeaponUseSpeed = value;
        }
        
        [JsonIgnore]
        public ItemStatFloat MeleeSpeed
        {
            get => WeaponMeleeSpeed;
            set => WeaponMeleeSpeed = value;
        }
        
        [JsonIgnore]
        public ItemStatFloat MeleeSize
        {
            get => WeaponMeleeSize;
            set => WeaponMeleeSize = value;
        }
        
        [JsonIgnore]
        public ProjectileStatFloat ProjectileSize
        {
            get => WeaponProjectileSize;
            set => WeaponProjectileSize = value;
        }
        
        [JsonIgnore]
        public ProjectileStatFloat ProjectileSpeed
        {
            get => WeaponProjectileSpeed;
            set => WeaponProjectileSpeed = value;
        }
        
        [JsonIgnore]
        public ProjectileStatFloat ProjectileLifeTime
        {
            get => WeaponProjectileLifeTime;
            set => WeaponProjectileLifeTime = value;
        }
        
        [JsonIgnore]
        public ProjectileStatFloat ProjectilePenetration
        {
            get => WeaponProjectilePenetration;
            set => WeaponProjectilePenetration = value;
        }
        
        [JsonIgnore]
        public ItemStatFloat ManaCostReduction
        {
            get => WeaponManaCostReduction;
            set => WeaponManaCostReduction = value;
        }
        
        [JsonIgnore]
        public ItemStatFloat AmmoConsumptionReduction
        {
            get => WeaponAmmoConsumptionReduction;
            set => WeaponAmmoConsumptionReduction = value;
        }
        
        [JsonIgnore]
        public ItemStatFloat MinionSlotReduction
        {
            get => AccessoryMinionSlotReduction;
            set => AccessoryMinionSlotReduction = value;
        }
    }
}
