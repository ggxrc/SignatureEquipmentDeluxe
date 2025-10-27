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

        // ==================== EVENTOS (MULTIPLICADORES DE XP) ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.EventPenaltyHeader")]
        
        [BackgroundColor(90, 40, 40)]
        [DefaultValue(true)]
        public bool EnableBossPenalty { get; set; } = true;
        
        [BackgroundColor(90, 40, 40)]
        [DefaultValue(true)]
        public bool EnableInvasionPenalty { get; set; } = true;
        
        [BackgroundColor(90, 40, 40)]
        [DefaultValue(false)]
        public bool EnableMoonPenalty { get; set; } = false;
        
        [BackgroundColor(90, 40, 40)]
        [DefaultValue(false)]
        public bool EnableWeatherPenalty { get; set; } = false;
        
        [BackgroundColor(90, 40, 40)]
        [DefaultValue(false)]
        public bool EnableTimePenalty { get; set; } = false;
        
        [BackgroundColor(90, 40, 40)]
        [DefaultValue(false)]
        public bool EnableSpecialPenalty { get; set; } = false;
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.BossEventsPreHardmodeHeader")]
        
        [BackgroundColor(80, 40, 40)]
        public List<EventMultiplier> BossEventsPreHardmode { get; set; } = new List<EventMultiplier>
        {
            new EventMultiplier { EventType = GameEventType.KingSlime, Category = EventCategory.BossPreHardmode, Enabled = true, Tier = XPMultiplierTier.Medium },
            new EventMultiplier { EventType = GameEventType.EyeOfCthulhu, Category = EventCategory.BossPreHardmode, Enabled = true, Tier = XPMultiplierTier.Medium },
            new EventMultiplier { EventType = GameEventType.EaterOfWorlds, Category = EventCategory.BossPreHardmode, Enabled = true, Tier = XPMultiplierTier.High },
            new EventMultiplier { EventType = GameEventType.BrainOfCthulhu, Category = EventCategory.BossPreHardmode, Enabled = true, Tier = XPMultiplierTier.High },
            new EventMultiplier { EventType = GameEventType.QueenBee, Category = EventCategory.BossPreHardmode, Enabled = true, Tier = XPMultiplierTier.High },
            new EventMultiplier { EventType = GameEventType.Skeletron, Category = EventCategory.BossPreHardmode, Enabled = true, Tier = XPMultiplierTier.High },
            new EventMultiplier { EventType = GameEventType.Deerclops, Category = EventCategory.BossPreHardmode, Enabled = true, Tier = XPMultiplierTier.High },
            new EventMultiplier { EventType = GameEventType.WallOfFlesh, Category = EventCategory.BossPreHardmode, Enabled = true, Tier = XPMultiplierTier.VeryHigh }
        };
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.BossEventsHardmodeHeader")]
        
        [BackgroundColor(100, 40, 40)]
        public List<EventMultiplier> BossEventsHardmode { get; set; } = new List<EventMultiplier>
        {
            new EventMultiplier { EventType = GameEventType.QueenSlime, Category = EventCategory.BossHardmode, Enabled = true, Tier = XPMultiplierTier.High },
            new EventMultiplier { EventType = GameEventType.TheTwins, Category = EventCategory.BossHardmode, Enabled = true, Tier = XPMultiplierTier.VeryHigh },
            new EventMultiplier { EventType = GameEventType.TheDestroyer, Category = EventCategory.BossHardmode, Enabled = true, Tier = XPMultiplierTier.VeryHigh },
            new EventMultiplier { EventType = GameEventType.SkeletronPrime, Category = EventCategory.BossHardmode, Enabled = true, Tier = XPMultiplierTier.VeryHigh },
            new EventMultiplier { EventType = GameEventType.Plantera, Category = EventCategory.BossHardmode, Enabled = true, Tier = XPMultiplierTier.VeryHigh },
            new EventMultiplier { EventType = GameEventType.Golem, Category = EventCategory.BossHardmode, Enabled = true, Tier = XPMultiplierTier.VeryHigh },
            new EventMultiplier { EventType = GameEventType.EmpressOfLight, Category = EventCategory.BossHardmode, Enabled = true, Tier = XPMultiplierTier.Extreme },
            new EventMultiplier { EventType = GameEventType.DukeFishron, Category = EventCategory.BossHardmode, Enabled = true, Tier = XPMultiplierTier.Extreme },
            new EventMultiplier { EventType = GameEventType.LunaticCultist, Category = EventCategory.BossHardmode, Enabled = true, Tier = XPMultiplierTier.VeryHigh },
            new EventMultiplier { EventType = GameEventType.MoonLord, Category = EventCategory.BossHardmode, Enabled = true, Tier = XPMultiplierTier.Extreme }
        };
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.MoonEventsHeader")]
        
        [BackgroundColor(60, 60, 80)]
        public List<EventMultiplier> MoonEvents { get; set; } = new List<EventMultiplier>
        {
            new EventMultiplier { EventType = GameEventType.BloodMoon, Category = EventCategory.Moon, Enabled = true, Tier = XPMultiplierTier.Medium },
            new EventMultiplier { EventType = GameEventType.FullMoon, Category = EventCategory.Moon, Enabled = true, Tier = XPMultiplierTier.Low },
            new EventMultiplier { EventType = GameEventType.NewMoon, Category = EventCategory.Moon, Enabled = false, Tier = XPMultiplierTier.None }
        };
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.InvasionEventsHeader")]
        
        [BackgroundColor(80, 40, 60)]
        public List<EventMultiplier> InvasionEvents { get; set; } = new List<EventMultiplier>
        {
            new EventMultiplier { EventType = GameEventType.GoblinArmy, Category = EventCategory.Invasion, Enabled = true, Tier = XPMultiplierTier.Medium },
            new EventMultiplier { EventType = GameEventType.FrostLegion, Category = EventCategory.Invasion, Enabled = true, Tier = XPMultiplierTier.Medium },
            new EventMultiplier { EventType = GameEventType.PirateInvasion, Category = EventCategory.Invasion, Enabled = true, Tier = XPMultiplierTier.High },
            new EventMultiplier { EventType = GameEventType.MartianMadness, Category = EventCategory.Invasion, Enabled = true, Tier = XPMultiplierTier.High },
            new EventMultiplier { EventType = GameEventType.PumpkinMoon, Category = EventCategory.Invasion, Enabled = true, Tier = XPMultiplierTier.VeryHigh },
            new EventMultiplier { EventType = GameEventType.FrostMoon, Category = EventCategory.Invasion, Enabled = true, Tier = XPMultiplierTier.VeryHigh },
            new EventMultiplier { EventType = GameEventType.SolarEclipse, Category = EventCategory.Invasion, Enabled = true, Tier = XPMultiplierTier.High },
            new EventMultiplier { EventType = GameEventType.LunarEvent, Category = EventCategory.Invasion, Enabled = true, Tier = XPMultiplierTier.Extreme }
        };
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.TimeEventsHeader")]
        
        [BackgroundColor(60, 60, 60)]
        public List<EventMultiplier> TimeEvents { get; set; } = new List<EventMultiplier>
        {
            new EventMultiplier { EventType = GameEventType.Day, Category = EventCategory.Time, Enabled = false, Tier = XPMultiplierTier.None },
            new EventMultiplier { EventType = GameEventType.Night, Category = EventCategory.Time, Enabled = true, Tier = XPMultiplierTier.Low }
        };
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.WeatherEventsHeader")]
        
        [BackgroundColor(40, 70, 90)]
        public List<EventMultiplier> WeatherEvents { get; set; } = new List<EventMultiplier>
        {
            new EventMultiplier { EventType = GameEventType.Rain, Category = EventCategory.Weather, Enabled = true, Tier = XPMultiplierTier.Low },
            new EventMultiplier { EventType = GameEventType.Sandstorm, Category = EventCategory.Weather, Enabled = true, Tier = XPMultiplierTier.Low },
            new EventMultiplier { EventType = GameEventType.Blizzard, Category = EventCategory.Weather, Enabled = true, Tier = XPMultiplierTier.Low }
        };
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.SpecialEventsHeader")]
        
        [BackgroundColor(70, 50, 70)]
        public List<EventMultiplier> SpecialEvents { get; set; } = new List<EventMultiplier>
        {
            new EventMultiplier { EventType = GameEventType.PartyEvent, Category = EventCategory.Special, Enabled = true, Tier = XPMultiplierTier.Medium },
            new EventMultiplier { EventType = GameEventType.LanternNight, Category = EventCategory.Special, Enabled = true, Tier = XPMultiplierTier.Medium }
        };
        
        // ==================== EXPERIÊNCIA GERAL ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.GeneralExperienceHeader")]
        
        [BackgroundColor(50, 50, 60)]
        [Range(0f, 2.5f)]
        [DefaultValue(1f)]
        [Increment(0.1f)]
        public float GlobalExpMultiplier { get; set; }
        
        [BackgroundColor(50, 50, 60)]
        [DefaultValue(1f)]
        [Increment(0.1f)]
        public float GlobalExpMultiplierExtra { get; set; }
        
        [BackgroundColor(50, 50, 60)]
        [DefaultValue(false)]
        public bool AllowStatueXP { get; set; }
        
        // ==================== PREÇOS DE LEVEL (EXP PRICE) ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.ExpPriceHeader")]
        
        [BackgroundColor(50, 50, 60)]
        [DefaultValue(100)]
        public int StartPrice { get; set; }
        
        [BackgroundColor(50, 50, 60)]
        [DefaultValue(0)]
        public int AditionalPrice { get; set; }
        
        [BackgroundColor(50, 50, 60)]
        [DefaultValue(0f)]
        [Increment(0.1f)]
        public float ExtraPrice { get; set; }
        
        [BackgroundColor(50, 50, 60)]
        [DefaultValue(1f)]
        [Increment(0.1f)]
        public float MultiPrice { get; set; }
        
        // ==================== EXPERIÊNCIA - ARMAS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.WeaponExperienceHeader")]
        
        [BackgroundColor(60, 40, 40)]
        [Range(0f, 2.5f)]
        [DefaultValue(1f)]
        [Increment(0.1f)]
        public float WeaponExpMultiplier { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(5f)]
        [Increment(1f)]
        public float WeaponBaseXPPerHit { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(50f)]
        [Increment(10f)]
        public float WeaponBaseXPPerKill { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(0.01f)]
        [Increment(0.01f)]
        public float WeaponXPPerDamageDealt { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(0.1f)]
        [Increment(0.1f)]
        public float WeaponXPPerEnemyMaxHP { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(0)]
        public int WeaponMaxLevel { get; set; }

        // ==================== EXPERIÊNCIA - ARMADURAS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.ArmorExperienceHeader")]
        
        [BackgroundColor(40, 40, 60)]
        [Range(0f, 2.5f)]
        [DefaultValue(1f)]
        [Increment(0.1f)]
        public float ArmorExpMultiplier { get; set; }
        
        [BackgroundColor(40, 40, 60)]
        [DefaultValue(1f)]
        [Increment(0.1f)]
        public float ArmorXPPerDamageReceived { get; set; }
        
        [BackgroundColor(40, 40, 60)]
        [DefaultValue(false)]
        public bool ArmorXPIgnoreDefense { get; set; }
        
        [BackgroundColor(40, 40, 60)]
        [DefaultValue(0.5f)]
        [Increment(0.1f)]
        public float ArmorXPPerDamageBlocked { get; set; }
        
        [BackgroundColor(40, 40, 60)]
        [DefaultValue(10f)]
        [Increment(1f)]
        public float ArmorXPPerDodge { get; set; }
        
        [BackgroundColor(40, 40, 60)]
        [DefaultValue(0)]
        public int ArmorMaxLevel { get; set; }

        // ==================== STATS - ARMAS ====================
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.WeaponStatsHeader")]
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(true)]
        public bool DamageIncrement { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(true)]
        public bool IncreaseBaseDamage { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(false)]
        public bool IncreaseFlatDamage { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(false)]
        public bool IncreaseMultDamage { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(true)]
        public bool CritIncrement { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(true)]
        public bool UseTimeIncrement { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(true)]
        public bool UseAnimationIncrement { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(true)]
        public bool MeleeWeaponSizeIncrement { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(true)]
        public bool NotUseAmmoChanceIncrement { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(true)]
        public bool ManaCostReductionIncrement { get; set; }
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.AdditionalProjectilesHeader")]
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(true)]
        public bool AdditionalProjectileChanceIncrement { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(10)]
        [Range(0, 360)]
        public int AdditionalProjectileMinRad { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(150)]
        [Range(0, 360)]
        public int AdditionalProjectileMaxRad { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(30)]
        [Range(0, 360)]
        public int AdditionalProjectileMinRadMinion { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(360)]
        [Range(0, 360)]
        public int AdditionalProjectileMaxRadMinion { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(100)]
        [Range(1, 1000)]
        public int ProjectilesToMaxRad { get; set; }
        
        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.ProjectileIncrementsHeader")]
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(true)]
        public bool ProjectileSizeIncrement { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(true)]
        public bool ProjectileSpeedIncrement { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(true)]
        public bool ProjectilePenetrationIncrement { get; set; }
        
        [BackgroundColor(60, 40, 40)]
        [DefaultValue(true)]
        public bool ProjectileLifeTimeIncrement { get; set; }
        
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
        public ItemStatFloat WeaponUseTime { get; set; } = new ItemStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1f,
            PerLevelMult = 1,
            Max = 0,
            ScalingTiers = new List<ScalingTier>()
        };

        [BackgroundColor(60, 40, 40)]
        public ItemStatFloat WeaponUseAnimation { get; set; } = new ItemStatFloat
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

        [Header("$Mods.SignatureEquipmentDeluxe.Config.ServerConfig.ProjectileStatsHeader")]
        
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
        public ProjectileStatFloat WeaponAdditionalProjectileChance { get; set; } = new ProjectileStatFloat
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1f,
            PerLevelMult = 2,
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
        [DefaultValue(true)]
        public bool DefenceIncrement { get; set; }
        
        [BackgroundColor(40, 40, 60)]
        public ItemStatInt ArmorDefense { get; set; } = new ItemStatInt
        {
            ScalingMode = ScalingMode.Legacy,
            PerLevel = 1,
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
                return min == int.MaxValue ? 0 : min;
            }
            set
            {
                // Aplica o mesmo valor para todos
                WeaponMaxLevel = value;
                ArmorMaxLevel = value;
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
            get => WeaponUseTime;
            set => WeaponUseTime = value;
        }
        
        [JsonIgnore]
        public ItemStatFloat UseTime
        {
            get => WeaponUseTime;
            set => WeaponUseTime = value;
        }
        
        [JsonIgnore]
        public ItemStatFloat UseAnimation
        {
            get => WeaponUseAnimation;
            set => WeaponUseAnimation = value;
        }
        
        [JsonIgnore]
        public ItemStatFloat MeleeSpeed
        {
            get => WeaponUseTime; // Agora melee também usa UseTime
            set => WeaponUseTime = value;
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
    }
}
