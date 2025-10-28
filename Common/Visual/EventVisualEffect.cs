using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SignatureEquipmentDeluxe.Common.Configs;
using SignatureEquipmentDeluxe.Common.Systems;
using SignatureEquipmentDeluxe.Common.Players;

namespace SignatureEquipmentDeluxe.Common.Visual
{
    /// <summary>
    /// Handles visual effects for active events, including stacking multiple events
    /// </summary>
    public static class EventVisualEffect
    {
        private static int particleCounter = 0;
        private static int penaltyParticleCounter = 0;

        /// <summary>
        /// Updates event visual effects around the player
        /// </summary>
        public static void UpdateEventVisuals(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            var clientConfig = ModContent.GetInstance<ClientConfig>();
            if (!clientConfig.ShowEventVisuals || !clientConfig.EnableParticleEffects)
                return;

            var eventTracker = player.GetModPlayer<SignaturePlayer>()?.GetEventTracker();
            if (eventTracker == null)
                return;

            // Get all active events
            List<EventType> activeEvents = GetActiveEvents(eventTracker);
            
            if (activeEvents.Count == 0)
                return;

            // Spawn particles for each active event
            if (clientConfig.ShowEventStacking)
            {
                // Stacking mode: spawn particles for each event, but with reduced rate
                SpawnStackedEventParticles(player, activeEvents, clientConfig);
            }
            else
            {
                // Non-stacking mode: only show the most significant event
                EventType primaryEvent = GetPrimaryEvent(activeEvents);
                SpawnSingleEventParticles(player, primaryEvent, clientConfig);
            }
        }

        /// <summary>
        /// Updates penalty visual effects (negative visuals when anti-farm penalty is active)
        /// </summary>
        public static void UpdatePenaltyVisuals(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            var clientConfig = ModContent.GetInstance<ClientConfig>();
            if (!clientConfig.EnableParticleEffects)
                return;

            var eventTracker = player.GetModPlayer<SignaturePlayer>()?.GetEventTracker();
            if (eventTracker == null || !eventTracker.IsAntiGrindPenaltyActive)
                return;

            // Spawn penalty particles
            penaltyParticleCounter++;
            int spawnRate = clientConfig.ReducedAnimations ? 20 : 10;

            if (penaltyParticleCounter >= spawnRate)
            {
                penaltyParticleCounter = 0;
                SpawnPenaltyParticles(player, eventTracker.CurrentExpMultiplier, clientConfig);
            }
        }

        private static List<EventType> GetActiveEvents(EventTracker tracker)
        {
            List<EventType> activeEvents = new List<EventType>();

            if (tracker.IsAnyBossActive) activeEvents.Add(EventType.Boss);
            if (tracker.IsAnyMoonEventActive) activeEvents.Add(EventType.MoonEvent);
            if (tracker.IsAnyInvasionActive) activeEvents.Add(EventType.Invasion);
            if (tracker.IsWeatherEventActive) activeEvents.Add(EventType.Weather);

            return activeEvents;
        }

        private static EventType GetPrimaryEvent(List<EventType> events)
        {
            // Priority: Boss > Moon > Invasion > Weather
            if (events.Contains(EventType.Boss)) return EventType.Boss;
            if (events.Contains(EventType.MoonEvent)) return EventType.MoonEvent;
            if (events.Contains(EventType.Invasion)) return EventType.Invasion;
            return EventType.Weather;
        }

        private static void SpawnStackedEventParticles(Player player, List<EventType> events, ClientConfig config)
        {
            particleCounter++;

            // Reduced spawn rate when stacking (to prevent particle spam)
            int baseRate = config.ReducedAnimations ? 30 : 15;
            int spawnRate = baseRate * events.Count; // More events = slower spawn per event

            if (particleCounter >= spawnRate)
            {
                particleCounter = 0;

                // Spawn 1 particle per active event
                foreach (EventType eventType in events)
                {
                    SpawnEventParticle(player, eventType);
                }
            }
        }

        private static void SpawnSingleEventParticles(Player player, EventType eventType, ClientConfig config)
        {
            particleCounter++;
            int spawnRate = config.ReducedAnimations ? 20 : 10;

            if (particleCounter >= spawnRate)
            {
                particleCounter = 0;
                SpawnEventParticle(player, eventType);
            }
        }

        private static void SpawnEventParticle(Player player, EventType eventType)
        {
            // Spawn subtle particles around the player
            Vector2 offset = new Vector2(Main.rand.NextFloat(-60f, 60f), Main.rand.NextFloat(-60f, 60f));
            Vector2 position = player.Center + offset;
            Vector2 velocity = new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-1f, 0.5f));

            short dustType = GetDustTypeForEvent(eventType);
            Color color = GetColorForEvent(eventType);

            Dust dust = Dust.NewDustPerfect(position, dustType, velocity, 0, color, 1.2f);
            dust.noGravity = true;
            dust.fadeIn = 0.8f;
        }

        private static void SpawnPenaltyParticles(Player player, float multiplier, ClientConfig config)
        {
            // Determine intensity based on penalty level
            // multiplier: 0.5 (50%) = subtle, 0.25 (25%) = moderate, 0.125 (12.5%) = heavy
            int particleCount = 1;
            if (multiplier <= 0.25f) particleCount = 2;
            if (multiplier <= 0.125f) particleCount = 3;

            for (int i = 0; i < particleCount; i++)
            {
                // Spawn "broken" particles around player
                Vector2 offset = new Vector2(Main.rand.NextFloat(-50f, 50f), Main.rand.NextFloat(-50f, 50f));
                Vector2 position = player.Center + offset;
                Vector2 velocity = new Vector2(Main.rand.NextFloat(-0.3f, 0.3f), Main.rand.NextFloat(0.5f, 1.5f)); // Fall downward

                // Use smoke, ash, or crack dust
                short dustType = Main.rand.Next(3) switch
                {
                    0 => DustID.Smoke,
                    1 => DustID.Ash,
                    _ => DustID.Stone
                };

                Color color = Main.rand.Next(2) == 0 ? 
                    new Color(100, 50, 50) : // Dark red
                    new Color(80, 80, 80);   // Gray

                Dust dust = Dust.NewDustPerfect(position, dustType, velocity, 0, color, 1.0f);
                dust.noGravity = false; // Let them fall
                dust.fadeIn = 0.5f;
            }
        }

        private static short GetDustTypeForEvent(EventType eventType)
        {
            return eventType switch
            {
                EventType.Boss => DustID.Torch,           // Fire/orange
                EventType.MoonEvent => DustID.PurpleTorch, // Purple
                EventType.Invasion => DustID.YellowTorch,  // Yellow
                EventType.Weather => DustID.RainCloud,     // Cyan/water
                _ => DustID.BlueTorch
            };
        }

        private static Color GetColorForEvent(EventType eventType)
        {
            return eventType switch
            {
                EventType.Boss => new Color(255, 150, 50),      // Orange
                EventType.MoonEvent => new Color(150, 100, 255), // Purple
                EventType.Invasion => new Color(255, 255, 100),  // Yellow
                EventType.Weather => new Color(100, 200, 255),   // Cyan
                _ => Color.White
            };
        }
    }

    /// <summary>
    /// Types of events that can have visual effects
    /// </summary>
    public enum EventType
    {
        Boss,
        MoonEvent,
        Invasion,
        Weather
    }
}
