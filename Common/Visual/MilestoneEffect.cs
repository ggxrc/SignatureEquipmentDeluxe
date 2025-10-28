using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using SignatureEquipmentDeluxe.Common.Configs;

namespace SignatureEquipmentDeluxe.Common.Visual
{
    /// <summary>
    /// Handles special visual effects for milestone levels (25, 50, 75, 100, 150, 200)
    /// </summary>
    public static class MilestoneEffect
    {
        private static readonly int[] MilestoneLevels = { 25, 50, 75, 100, 150, 200 };

        /// <summary>
        /// Checks if the given level is a milestone level
        /// </summary>
        public static bool IsMilestoneLevel(int level)
        {
            foreach (int milestone in MilestoneLevels)
            {
                if (level == milestone)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Spawns a special milestone celebration effect
        /// </summary>
        public static void SpawnMilestoneEffect(Player player, int level)
        {
            var clientConfig = ModContent.GetInstance<ClientConfig>();
            
            // Check config
            if (!clientConfig.ShowMilestoneEffects)
                return;

            // Check for reduced animations
            if (clientConfig.ReducedAnimations)
            {
                // Still show but with reduced particles
                SpawnReducedMilestoneEffect(player, level);
                return;
            }

            // Full effect
            SpawnFullMilestoneEffect(player, level);
        }

        private static void SpawnFullMilestoneEffect(Player player, int level)
        {
            Vector2 position = player.Center;

            // Play special sound
            SoundEngine.PlaySound(SoundID.Item4 with { Volume = 1.5f, Pitch = 0.3f }, position);

            // Determine particle count and radius based on milestone level
            int particleCount = GetParticleCountForLevel(level);
            float maxRadius = GetRadiusForLevel(level);

            // Spawn expanding circular wave of particles
            for (int i = 0; i < particleCount; i++)
            {
                float angle = MathHelper.TwoPi * i / particleCount;
                Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * maxRadius;
                Vector2 velocity = offset / 30f; // Particles expand over ~30 frames

                // Use multiple dust types for variety
                int dustType = GetMilestoneDustType(level, i);
                Color dustColor = GetMilestoneColor(level);

                Dust dust = Dust.NewDustPerfect(position, dustType, velocity, 0, dustColor, 2f);
                dust.noGravity = true;
                dust.fadeIn = 1.4f;
            }

            // Inner explosion ring
            for (int i = 0; i < particleCount / 2; i++)
            {
                float angle = MathHelper.TwoPi * i / (particleCount / 2);
                Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * maxRadius * 0.5f;
                Vector2 velocity = offset / 40f;

                Dust dust = Dust.NewDustPerfect(position, DustID.RainbowMk2, velocity, 0, Color.White, 2.5f);
                dust.noGravity = true;
                dust.fadeIn = 1.6f;
            }

            // Sparkles rising upward
            int sparkleCount = level / 10; // More sparkles at higher levels
            for (int i = 0; i < sparkleCount; i++)
            {
                Vector2 sparklePos = position + new Vector2(Main.rand.NextFloat(-40f, 40f), Main.rand.NextFloat(-40f, 40f));
                Vector2 sparkleVel = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-3f, -1f));

                Dust sparkle = Dust.NewDustPerfect(sparklePos, DustID.GoldCoin, sparkleVel, 0, GetMilestoneColor(level), 1.5f);
                sparkle.noGravity = true;
            }
        }

        private static void SpawnReducedMilestoneEffect(Player player, int level)
        {
            Vector2 position = player.Center;

            // Play sound at lower volume
            SoundEngine.PlaySound(SoundID.Item4 with { Volume = 0.8f, Pitch = 0.3f }, position);

            // Spawn only 1/3 of particles
            int particleCount = GetParticleCountForLevel(level) / 3;
            float maxRadius = GetRadiusForLevel(level);

            for (int i = 0; i < particleCount; i++)
            {
                float angle = MathHelper.TwoPi * i / particleCount;
                Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * maxRadius;
                Vector2 velocity = offset / 30f;

                int dustType = GetMilestoneDustType(level, i);
                Color dustColor = GetMilestoneColor(level);

                Dust dust = Dust.NewDustPerfect(position, dustType, velocity, 0, dustColor, 1.5f);
                dust.noGravity = true;
            }
        }

        private static int GetParticleCountForLevel(int level)
        {
            return level switch
            {
                25 => 30,
                50 => 45,
                75 => 60,
                100 => 80,
                150 => 100,
                200 => 120,
                _ => 40
            };
        }

        private static float GetRadiusForLevel(int level)
        {
            return level switch
            {
                25 => 60f,
                50 => 80f,
                75 => 100f,
                100 => 120f,
                150 => 150f,
                200 => 180f,
                _ => 80f
            };
        }

        private static int GetMilestoneDustType(int level, int index)
        {
            // Alternate between different dust types based on level and index
            short[] dustTypes = level switch
            {
                25 => new short[] { DustID.BlueFairy, DustID.PinkFairy },
                50 => new short[] { DustID.GreenFairy, DustID.YellowStarDust },
                75 => new short[] { DustID.PurpleTorch, DustID.RainbowMk2 },
                100 => new short[] { DustID.FireworkFountain_Yellow, DustID.FireworkFountain_Red },
                150 => new short[] { DustID.Firework_Blue, DustID.Firework_Pink },
                200 => new short[] { DustID.RainbowRod, DustID.PlatinumCoin },
                _ => new short[] { DustID.BlueFairy, DustID.PinkFairy }
            };

            return dustTypes[index % dustTypes.Length];
        }

        private static Color GetMilestoneColor(int level)
        {
            return level switch
            {
                25 => new Color(100, 200, 255), // Light blue
                50 => new Color(100, 255, 100), // Green
                75 => new Color(200, 100, 255), // Purple
                100 => new Color(255, 200, 50), // Gold
                150 => new Color(255, 100, 200), // Pink
                200 => new Color(255, 255, 255), // White/Rainbow
                _ => Color.White
            };
        }

        /// <summary>
        /// Spawns milestone text above the player
        /// </summary>
        public static void SpawnMilestoneText(Player player, int level)
        {
            var clientConfig = ModContent.GetInstance<ClientConfig>();
            if (!clientConfig.ShowMilestoneEffects)
                return;

            // Use CombatText for large milestone announcement
            string text = $"MILESTONE! Level {level}";
            Color color = GetMilestoneColor(level);

            CombatText.NewText(
                new Rectangle((int)player.Center.X - 50, (int)player.Center.Y - 100, 100, 50),
                color,
                text,
                dramatic: true,
                dot: false
            );
        }
    }
}
