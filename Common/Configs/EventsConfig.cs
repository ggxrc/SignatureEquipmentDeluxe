using System.Collections.Generic;
using System.ComponentModel;
using Progression.Common.Systems;
using Terraria.ModLoader.Config;

namespace Progression.Common.Configs
{
    /// <summary>
    /// Event and Multiplier Settings
    /// Controls XP bonuses during in-game events
    /// </summary>
    [BackgroundColor(60, 40, 50)]
    public class EventsConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        // ==================== ANTI-FARM PENALTIES ====================

        [Header("$Mods.Progression.Config.EventsConfig.AntiFarmHeader")]
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

        // ==================== BOSS EVENTS PRE-HARDMODE ====================

        [Header("$Mods.Progression.Config.EventsConfig.BossPreHardmodeHeader")]
        [BackgroundColor(80, 40, 40)]
        public List<EventMultiplier> BossEventsPreHardmode { get; set; } =
            new List<EventMultiplier>
            {
                new EventMultiplier
                {
                    EventType = GameEventType.KingSlime,
                    Category = EventCategory.BossPreHardmode,
                    Enabled = true,
                    Tier = XPMultiplierTier.Medium,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.EyeOfCthulhu,
                    Category = EventCategory.BossPreHardmode,
                    Enabled = true,
                    Tier = XPMultiplierTier.Medium,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.EaterOfWorlds,
                    Category = EventCategory.BossPreHardmode,
                    Enabled = true,
                    Tier = XPMultiplierTier.High,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.BrainOfCthulhu,
                    Category = EventCategory.BossPreHardmode,
                    Enabled = true,
                    Tier = XPMultiplierTier.High,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.QueenBee,
                    Category = EventCategory.BossPreHardmode,
                    Enabled = true,
                    Tier = XPMultiplierTier.High,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.Skeletron,
                    Category = EventCategory.BossPreHardmode,
                    Enabled = true,
                    Tier = XPMultiplierTier.High,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.Deerclops,
                    Category = EventCategory.BossPreHardmode,
                    Enabled = true,
                    Tier = XPMultiplierTier.High,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.WallOfFlesh,
                    Category = EventCategory.BossPreHardmode,
                    Enabled = true,
                    Tier = XPMultiplierTier.VeryHigh,
                },
            };

        // ==================== BOSS EVENTS HARDMODE ====================

        [Header("$Mods.Progression.Config.EventsConfig.BossHardmodeHeader")]
        [BackgroundColor(100, 40, 40)]
        public List<EventMultiplier> BossEventsHardmode { get; set; } =
            new List<EventMultiplier>
            {
                new EventMultiplier
                {
                    EventType = GameEventType.QueenSlime,
                    Category = EventCategory.BossHardmode,
                    Enabled = true,
                    Tier = XPMultiplierTier.High,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.TheTwins,
                    Category = EventCategory.BossHardmode,
                    Enabled = true,
                    Tier = XPMultiplierTier.VeryHigh,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.TheDestroyer,
                    Category = EventCategory.BossHardmode,
                    Enabled = true,
                    Tier = XPMultiplierTier.VeryHigh,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.SkeletronPrime,
                    Category = EventCategory.BossHardmode,
                    Enabled = true,
                    Tier = XPMultiplierTier.VeryHigh,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.Plantera,
                    Category = EventCategory.BossHardmode,
                    Enabled = true,
                    Tier = XPMultiplierTier.VeryHigh,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.Golem,
                    Category = EventCategory.BossHardmode,
                    Enabled = true,
                    Tier = XPMultiplierTier.VeryHigh,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.EmpressOfLight,
                    Category = EventCategory.BossHardmode,
                    Enabled = true,
                    Tier = XPMultiplierTier.Extreme,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.DukeFishron,
                    Category = EventCategory.BossHardmode,
                    Enabled = true,
                    Tier = XPMultiplierTier.Extreme,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.LunaticCultist,
                    Category = EventCategory.BossHardmode,
                    Enabled = true,
                    Tier = XPMultiplierTier.VeryHigh,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.MoonLord,
                    Category = EventCategory.BossHardmode,
                    Enabled = true,
                    Tier = XPMultiplierTier.Extreme,
                },
            };

        // ==================== MOON EVENTS ====================

        [Header("$Mods.Progression.Config.EventsConfig.MoonEventsHeader")]
        [BackgroundColor(60, 60, 80)]
        public List<EventMultiplier> MoonEvents { get; set; } =
            new List<EventMultiplier>
            {
                new EventMultiplier
                {
                    EventType = GameEventType.BloodMoon,
                    Category = EventCategory.Moon,
                    Enabled = true,
                    Tier = XPMultiplierTier.Medium,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.FullMoon,
                    Category = EventCategory.Moon,
                    Enabled = true,
                    Tier = XPMultiplierTier.Low,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.NewMoon,
                    Category = EventCategory.Moon,
                    Enabled = false,
                    Tier = XPMultiplierTier.None,
                },
            };

        // ==================== INVASION EVENTS ====================

        [Header("$Mods.Progression.Config.EventsConfig.InvasionEventsHeader")]
        [BackgroundColor(80, 40, 60)]
        public List<EventMultiplier> InvasionEvents { get; set; } =
            new List<EventMultiplier>
            {
                new EventMultiplier
                {
                    EventType = GameEventType.GoblinArmy,
                    Category = EventCategory.Invasion,
                    Enabled = true,
                    Tier = XPMultiplierTier.Medium,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.FrostLegion,
                    Category = EventCategory.Invasion,
                    Enabled = true,
                    Tier = XPMultiplierTier.Medium,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.PirateInvasion,
                    Category = EventCategory.Invasion,
                    Enabled = true,
                    Tier = XPMultiplierTier.High,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.MartianMadness,
                    Category = EventCategory.Invasion,
                    Enabled = true,
                    Tier = XPMultiplierTier.High,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.PumpkinMoon,
                    Category = EventCategory.Invasion,
                    Enabled = true,
                    Tier = XPMultiplierTier.VeryHigh,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.FrostMoon,
                    Category = EventCategory.Invasion,
                    Enabled = true,
                    Tier = XPMultiplierTier.VeryHigh,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.SolarEclipse,
                    Category = EventCategory.Invasion,
                    Enabled = true,
                    Tier = XPMultiplierTier.High,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.LunarEvent,
                    Category = EventCategory.Invasion,
                    Enabled = true,
                    Tier = XPMultiplierTier.Extreme,
                },
            };

        // ==================== TIME EVENTS ====================

        [Header("$Mods.Progression.Config.EventsConfig.TimeEventsHeader")]
        [BackgroundColor(60, 60, 60)]
        public List<EventMultiplier> TimeEvents { get; set; } =
            new List<EventMultiplier>
            {
                new EventMultiplier
                {
                    EventType = GameEventType.Day,
                    Category = EventCategory.Time,
                    Enabled = false,
                    Tier = XPMultiplierTier.None,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.Night,
                    Category = EventCategory.Time,
                    Enabled = true,
                    Tier = XPMultiplierTier.Low,
                },
            };

        // ==================== WEATHER EVENTS ====================

        [Header("$Mods.Progression.Config.EventsConfig.WeatherEventsHeader")]
        [BackgroundColor(40, 70, 90)]
        public List<EventMultiplier> WeatherEvents { get; set; } =
            new List<EventMultiplier>
            {
                new EventMultiplier
                {
                    EventType = GameEventType.Rain,
                    Category = EventCategory.Weather,
                    Enabled = true,
                    Tier = XPMultiplierTier.Low,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.Sandstorm,
                    Category = EventCategory.Weather,
                    Enabled = true,
                    Tier = XPMultiplierTier.Low,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.Blizzard,
                    Category = EventCategory.Weather,
                    Enabled = true,
                    Tier = XPMultiplierTier.Low,
                },
            };

        // ==================== SPECIAL EVENTS ====================

        [Header("$Mods.Progression.Config.EventsConfig.SpecialEventsHeader")]
        [BackgroundColor(70, 50, 70)]
        public List<EventMultiplier> SpecialEvents { get; set; } =
            new List<EventMultiplier>
            {
                new EventMultiplier
                {
                    EventType = GameEventType.PartyEvent,
                    Category = EventCategory.Special,
                    Enabled = true,
                    Tier = XPMultiplierTier.Medium,
                },
                new EventMultiplier
                {
                    EventType = GameEventType.LanternNight,
                    Category = EventCategory.Special,
                    Enabled = true,
                    Tier = XPMultiplierTier.Medium,
                },
            };
    }
}
