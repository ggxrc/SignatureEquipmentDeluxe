using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// Sistema de efeitos visuais para zonas radioativas
    /// </summary>
    public class RadioactiveZoneVisuals : ModSystem
    {
        private int visualTimer = 0;
        
        public override void PostUpdateDusts()
        {
            visualTimer++;
            
            if (visualTimer % 5 != 0) return;
            
            var zones = LeveledEnemySystem.radioactiveZones;
            if (zones == null || zones.Count == 0) return;
            
            foreach (var zone in zones)
            {
                // Spawn partículas verdes ao redor da zona
                SpawnRadioactiveParticles(zone);
            }
        }
        
        /// <summary>
        /// Spawn partículas verdes ao redor da zona radioativa
        /// </summary>
        private void SpawnRadioactiveParticles(LeveledEnemySystem.RadioactiveZone zone)
        {
            var config = ModContent.GetInstance<Configs.ServerConfig>();
            if (!config.EnableLeveledEnemies)
                return;
            
            float radius = zone.Radius;
            Vector2 center = zone.Position;
            
            // Spawn 3-5 partículas aleatórias ao redor do perímetro (+50%)
            int particleCount = Main.rand.Next(3, 6);
            
            for (int i = 0; i < particleCount; i++)
            {
                // Posição aleatória ao redor do círculo
                float angle = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                float distance = radius * Main.rand.NextFloat(0.8f, 1.0f);
                
                Vector2 particlePos = center + new Vector2(
                    (float)System.Math.Cos(angle) * distance,
                    (float)System.Math.Sin(angle) * distance
                );
                
                // Spawn dust verde (CursedTorch)
                Dust dust = Dust.NewDustPerfect(particlePos, DustID.CursedTorch, Vector2.Zero, 0, default, 1.5f);
                dust.noGravity = true;
                dust.velocity = Vector2.Zero;
                dust.fadeIn = 0.8f;
            }
            
            // Spawn partículas no centro a cada 30 frames
            if (visualTimer % 30 == 0)
            {
                for (int i = 0; i < 8; i++) // Aumentado de 5 para 8 (+50%)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(2f, 2f);
                    Dust centerDust = Dust.NewDustPerfect(center, DustID.CursedTorch, velocity, 0, default, 1.2f);
                    centerDust.noGravity = true;
                    centerDust.fadeIn = 1f;
                }
            }
        }
        
        /// <summary>
        /// Desenha círculos e marcadores das zonas radioativas
        /// </summary>
        public override void PostDrawTiles()
        {
            var zones = LeveledEnemySystem.radioactiveZones;
            if (zones == null || zones.Count == 0) return;
            
            // Não desenha texto aqui, apenas efeitos de tile
            // O texto será desenhado em PostDrawInterface
        }
        
        /// <summary>
        /// Desenha texto "RADIOACTIVE ZONE" no centro da zona
        /// </summary>
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            var zones = LeveledEnemySystem.radioactiveZones;
            if (zones == null || zones.Count == 0) return;
            
            foreach (var zone in zones)
            {
                Vector2 screenPos = zone.Position - Main.screenPosition;
                
                // Só desenha se estiver na tela
                if (screenPos.X < -100 || screenPos.X > Main.screenWidth + 100 ||
                    screenPos.Y < -100 || screenPos.Y > Main.screenHeight + 100)
                    continue;
                
                // Texto piscante
                float alpha = 0.5f + (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 2f) * 0.3f;
                Color textColor = Color.Lime * alpha;
                
                string text = "☢ RADIOACTIVE ZONE ☢";
                Vector2 textSize = Terraria.GameContent.FontAssets.MouseText.Value.MeasureString(text);
                Vector2 textPos = screenPos - textSize / 2f;
                
                // Desenha sombra preta
                Terraria.Utils.DrawBorderString(
                    spriteBatch,
                    text,
                    textPos,
                    Color.Black * alpha,
                    1f
                );
                
                // Desenha texto verde
                Terraria.Utils.DrawBorderString(
                    spriteBatch,
                    text,
                    textPos,
                    textColor,
                    1f
                );
                
                // Desenha tempo restante abaixo
                int minutesLeft = zone.TimeLeft / 3600; // 60 FPS * 60 segundos
                int secondsLeft = (zone.TimeLeft / 60) % 60;
                string timeText = $"{minutesLeft:D2}:{secondsLeft:D2}";
                
                Vector2 timeSize = Terraria.GameContent.FontAssets.MouseText.Value.MeasureString(timeText);
                Vector2 timePos = textPos + new Vector2(textSize.X / 2f - timeSize.X / 2f, textSize.Y + 5);
                
                Terraria.Utils.DrawBorderString(
                    spriteBatch,
                    timeText,
                    timePos,
                    Color.Yellow * alpha,
                    0.8f
                );
            }
        }
    }
}
