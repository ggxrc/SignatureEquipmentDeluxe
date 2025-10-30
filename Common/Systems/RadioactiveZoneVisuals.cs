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
        private int chaosTimer = 0; // Timer separado para partículas caóticas
        
        public override void PostUpdateDusts()
        {
            visualTimer++;
            chaosTimer++;
            
            if (visualTimer % 5 != 0) return;
            
            var zones = LeveledEnemySystem.radioactiveZones;
            if (zones == null || zones.Count == 0) return;
            
            foreach (var zone in zones)
            {
                // Spawn partículas verdes ao redor da zona
                SpawnRadioactiveParticles(zone);
                
                // NOVO: Spawn partículas caóticas extras
                if (chaosTimer % 3 == 0) // Mais frequente
                {
                    SpawnChaoticParticles(zone);
                }
            }
        }
        
        /// <summary>
        /// Spawn partículas verdes ao redor da zona radioativa
        /// Partículas aumentam de 50% na borda até 600% no centro
        /// Danger level aumenta partículas em 50% por tier
        /// </summary>
        private void SpawnRadioactiveParticles(LeveledEnemySystem.RadioactiveZone zone)
        {
            var config = ModContent.GetInstance<Configs.ServerConfig>();
            if (!config.EnableLeveledEnemies)
                return;
            
            float radius = zone.Radius;
            Vector2 center = zone.Position;
            
            // MULTIPLICADOR DE PERIGO: +50% partículas por tier
            float dangerMultiplier = zone.GetParticleMultiplier();
            Color dangerColor = zone.GetDangerColor();
            
            // Spawn partículas em anéis do centro até a borda
            // Borda: pouquíssimas partículas
            // Centro: CAÓTICO - partículas ultra densas e agressivas
            int ringCount = 5; // 5 anéis de partículas
            
            for (int ring = 0; ring < ringCount; ring++)
            {
                // Distância normalizada do centro (0.0 = centro, 1.0 = borda)
                float normalizedDistance = (ring + 1) / (float)ringCount;
                float ringRadius = radius * normalizedDistance;
                
                // Densidade de partículas baseada na distância do centro
                // MEGA AUMENTO: 50% na borda até 600% no centro!
                // Centro (0.0): 6.0x multiplicador base + 5.5x bonus = 11.5x total (aprox 600% comparado ao original)
                // Meio (0.5): 6.0x + 2.75x = 8.75x
                // Borda (1.0): 6.0x + 0x = 6.0x (aprox 300% do original, mas vamos ajustar para 50%)
                
                // Calculamos o multiplicador exponencial para mais caos no centro
                float distanceFromCenter = 1f - normalizedDistance; // 1.0 no centro, 0.0 na borda
                float exponentialFactor = distanceFromCenter * distanceFromCenter; // Exponencial para mais contraste
                
                // Base mais alta para borda (50% do original 2.0 = 1.0, mas queremos 50% MAIOR = 3.0)
                float baseDensity = 3.0f; // 50% maior que original
                // Bonus massivo no centro (até 600%)
                float centerBonus = 33f * exponentialFactor; // Exponencial para mega caos no centro
                float densityMultiplier = (baseDensity + centerBonus) * dangerMultiplier; // APLICA DANGER MULTIPLIER
                
                // Base: 4-8 partículas por anel (dobrado)
                int baseParticles = Main.rand.Next(4, 9);
                int particleCount = (int)(baseParticles * densityMultiplier);
                
                for (int i = 0; i < particleCount; i++)
                {
                    // Posição aleatória ao redor do anel
                    float angle = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                    // Mais variação perto do centro (mais caos)
                    float distanceVariation = 0.9f + (0.2f * (1f - normalizedDistance)); // Mais caos no centro
                    float distance = ringRadius * Main.rand.NextFloat(0.9f, distanceVariation);
                    
                    Vector2 particlePos = center + new Vector2(
                        (float)System.Math.Cos(angle) * distance,
                        (float)System.Math.Sin(angle) * distance
                    );
                    
                    // Partículas MUITO mais brilhantes e maiores perto do centro
                    float scale = 1.0f + (1f - normalizedDistance) * 1.5f; // Era 0.8f, agora 1.5f
                    
                    // Usa cor de perigo baseado no danger level
                    Color particleColor = Color.Lerp(Color.Green, dangerColor, 0.6f);
                    
                    // Spawn dust verde (CursedTorch)
                    Dust dust = Dust.NewDustPerfect(particlePos, DustID.CursedTorch, Vector2.Zero, 0, particleColor, scale);
                    dust.noGravity = true;
                    
                    // FINAL COUNTDOWN: Partículas atraídas para o centro
                    if (zone.IsFinalCountdown)
                    {
                        Vector2 directionToCenter = Vector2.Normalize(center - particlePos);
                        dust.velocity = directionToCenter * 3f; // Velocidade de atração
                        dust.scale *= 1.5f; // Maiores durante countdown
                    }
                    else
                    {
                        dust.velocity = Vector2.Zero;
                    }
                    
                    dust.fadeIn = 0.8f + (1f - normalizedDistance) * 0.5f;
                }
            }
            
            // Spawn EXPLOSÃO de partículas no centro a cada 30 frames
            if (visualTimer % 30 == 0)
            {
                // +50% = 60 partículas, multiplicado por danger
                int centerParticles = (int)(60 * dangerMultiplier);
                
                // FINAL COUNTDOWN: Mais partículas
                if (zone.IsFinalCountdown)
                {
                    centerParticles = (int)(centerParticles * 2f);
                }
                
                for (int i = 0; i < centerParticles; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(3f, 3f); // Velocidade aumentada
                    
                    // FINAL COUNTDOWN: Atração ao centro ao invés de expansão
                    if (zone.IsFinalCountdown)
                    {
                        velocity = Main.rand.NextVector2CircularEdge(2f, 2f); // Direção aleatória para centro
                        velocity = -velocity; // Inverter para ir para dentro
                    }
                    
                    Dust centerDust = Dust.NewDustPerfect(center, DustID.CursedTorch, velocity, 0, dangerColor, 2.2f); // Usa cor de perigo
                    centerDust.noGravity = true;
                    centerDust.fadeIn = 2.0f; // FadeIn aumentado
                    
                    // FINAL COUNTDOWN: Maiores
                    if (zone.IsFinalCountdown)
                    {
                        centerDust.scale *= 1.5f;
                    }
                }
            }
            
            // EFEITOS ESPECIAIS POR DANGER LEVEL
            SpawnDangerEffects(zone, center, radius);
        }
        
        /// <summary>
        /// Spawn partículas caóticas aleatórias pela zona inteira
        /// NÃO mexe no anel visual, apenas adiciona mais caos
        /// </summary>
        private void SpawnChaoticParticles(LeveledEnemySystem.RadioactiveZone zone)
        {
            Vector2 center = zone.Position;
            float radius = zone.Radius;
            float dangerMultiplier = zone.GetParticleMultiplier();
            Color dangerColor = zone.GetDangerColor();
            
            // Quantidade base de partículas caóticas (multiplicado por danger)
            int baseParticles = 8; // Base: 8 partículas por frame
            int particleCount = (int)(baseParticles * dangerMultiplier);
            
            for (int i = 0; i < particleCount; i++)
            {
                // Posição completamente aleatória dentro do raio
                Vector2 randomOffset = Main.rand.NextVector2Circular(radius, radius);
                Vector2 particlePos = center + randomOffset;
                
                // Tipo de partícula aleatória para mais variedade
                int dustType = Main.rand.Next(new int[] {
                    DustID.CursedTorch,  // Verde padrão
                    DustID.GreenTorch,   // Verde mais claro
                    DustID.Poisoned,     // Verde venenoso
                    DustID.Electric      // Amarelo elétrico
                });
                
                // Velocidade aleatória (partículas mais lentas = mais caóticas)
                Vector2 velocity = Main.rand.NextVector2Circular(1.5f, 1.5f);
                
                // Tamanho aleatório
                float scale = Main.rand.NextFloat(1f, 2.5f);
                
                // Cor mista entre verde e cor de perigo
                Color particleColor = Main.rand.NextBool() ? Color.LimeGreen : dangerColor;
                
                Dust chaos = Dust.NewDustPerfect(particlePos, dustType, velocity, 0, particleColor, scale);
                chaos.noGravity = Main.rand.NextBool(3); // 66% sem gravidade, 33% com gravidade (mais variedade)
                
                // FINAL COUNTDOWN: Atração para o centro
                if (zone.IsFinalCountdown)
                {
                    Vector2 directionToCenter = Vector2.Normalize(center - particlePos);
                    chaos.velocity = directionToCenter * 2f; // Atração mais lenta para caos
                    chaos.noGravity = true; // Todas sem gravidade durante countdown
                    chaos.scale *= 1.2f; // Maiores
                }
                
                chaos.fadeIn = Main.rand.NextFloat(0.5f, 1.5f);
            }
            
            // Partículas extras flutuando para cima (efeito de fumaça radioativa)
            if (visualTimer % 10 == 0)
            {
                int smokeParticles = (int)(5 * dangerMultiplier);
                for (int i = 0; i < smokeParticles; i++)
                {
                    Vector2 randomPos = center + Main.rand.NextVector2Circular(radius * 0.8f, radius * 0.8f);
                    Dust smoke = Dust.NewDustPerfect(randomPos, DustID.Smoke, new Vector2(0, -Main.rand.NextFloat(0.5f, 2f)), 0, Color.DarkGreen, Main.rand.NextFloat(1.5f, 2.5f));
                    smoke.noGravity = true;
                    smoke.alpha = 100;
                }
            }
        }
        
        /// <summary>
        /// Efeitos visuais especiais baseados no nível de perigo
        /// </summary>
        private void SpawnDangerEffects(LeveledEnemySystem.RadioactiveZone zone, Vector2 center, float radius)
        {
            // Tier 3+: Relâmpagos verdes ocasionais
            if (zone.DangerLevel >= 3 && visualTimer % 90 == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 lightningStart = center + Main.rand.NextVector2Circular(radius * 0.8f, radius * 0.8f);
                    Vector2 lightningEnd = lightningStart + Main.rand.NextVector2Circular(100f, 100f);
                    
                    // Linha de partículas simulando relâmpago
                    int steps = 10;
                    for (int j = 0; j < steps; j++)
                    {
                        Vector2 pos = Vector2.Lerp(lightningStart, lightningEnd, j / (float)steps);
                        Dust lightning = Dust.NewDustPerfect(pos, DustID.Electric, Vector2.Zero, 0, Color.LimeGreen, 2f);
                        lightning.noGravity = true;
                    }
                }
            }
            
            // Tier 4+: Ondas de choque pulsantes
            if (zone.DangerLevel >= 4 && visualTimer % 60 == 0)
            {
                for (int ring = 0; ring < 3; ring++)
                {
                    float shockwaveRadius = radius * (0.3f + ring * 0.3f);
                    int particlesInRing = 30;
                    
                    for (int i = 0; i < particlesInRing; i++)
                    {
                        float angle = (i / (float)particlesInRing) * MathHelper.TwoPi;
                        Vector2 pos = center + new Vector2(
                            (float)System.Math.Cos(angle) * shockwaveRadius,
                            (float)System.Math.Sin(angle) * shockwaveRadius
                        );
                        
                        Dust shockwave = Dust.NewDustPerfect(pos, DustID.Torch, Vector2.Zero, 0, Color.Orange, 1.5f);
                        shockwave.noGravity = true;
                    }
                }
            }
            
            // Tier 5: APOCALIPSE - Aura vermelha pulsante massiva
            if (zone.DangerLevel >= 5)
            {
                if (visualTimer % 10 == 0)
                {
                    // Anel vermelho massivo
                    int apocalypseParticles = 50;
                    for (int i = 0; i < apocalypseParticles; i++)
                    {
                        float angle = (i / (float)apocalypseParticles) * MathHelper.TwoPi;
                        float pulseRadius = radius * (0.9f + (float)System.Math.Sin(visualTimer * 0.05f) * 0.1f);
                        Vector2 pos = center + new Vector2(
                            (float)System.Math.Cos(angle) * pulseRadius,
                            (float)System.Math.Sin(angle) * pulseRadius
                        );
                        
                        Dust apocalypse = Dust.NewDustPerfect(pos, DustID.Torch, Vector2.Zero, 0, Color.DarkRed, 2.5f);
                        apocalypse.noGravity = true;
                        apocalypse.fadeIn = 2.5f;
                    }
                }
            }
            
            // SPAWNS DE FOGO BASEADOS NO TIER
            SpawnFireEffects(zone, center, radius);
        }
        
        /// <summary>
        /// Spawn efeitos de fogo baseados no tier
        /// </summary>
        private void SpawnFireEffects(LeveledEnemySystem.RadioactiveZone zone, Vector2 center, float radius)
        {
            // Tier 3: Cursed fire at epicenter
            if (zone.DangerLevel >= 3 && visualTimer % 120 == 0) // A cada 2 segundos
            {
                // Spawn cursed flame projectile no epicentro
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(
                        Main.LocalPlayer.GetSource_FromThis(),
                        center,
                        Vector2.Zero,
                        ProjectileID.CursedFlameHostile, // Cursed flame hostile
                        20, // Damage
                        0f,
                        Main.myPlayer
                    );
                }
            }
            
            // Tier 4: Shadow flame before epicenter
            if (zone.DangerLevel >= 4 && visualTimer % 100 == 0) // A cada ~1.67 segundos
            {
                float flameRadius = radius * 0.7f; // Antes do epicentro
                for (int i = 0; i < 4; i++) // 4 projéteis
                {
                    float angle = (i / 4f) * MathHelper.TwoPi + Main.rand.NextFloat(-0.2f, 0.2f);
                    Vector2 pos = center + new Vector2((float)System.Math.Cos(angle) * flameRadius, (float)System.Math.Sin(angle) * flameRadius);
                    
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(
                            Main.LocalPlayer.GetSource_FromThis(),
                            pos,
                            Vector2.Zero,
                            ProjectileID.ShadowFlame, // Shadow flame
                            25, // Damage
                            0f,
                            Main.myPlayer
                        );
                    }
                }
            }
            
            // Tier 5: Two fire rings before epicenter
            if (zone.DangerLevel >= 5 && visualTimer % 80 == 0) // A cada ~1.33 segundos
            {
                float innerRadius = radius * 0.5f;
                float outerRadius = radius * 0.8f;
                
                // Inner ring: Cursed fire
                for (int i = 0; i < 6; i++)
                {
                    float angle = (i / 6f) * MathHelper.TwoPi;
                    Vector2 pos = center + new Vector2((float)System.Math.Cos(angle) * innerRadius, (float)System.Math.Sin(angle) * innerRadius);
                    
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(
                            Main.LocalPlayer.GetSource_FromThis(),
                            pos,
                            Vector2.Zero,
                            ProjectileID.CursedFlameHostile,
                            30, // Damage
                            0f,
                            Main.myPlayer
                        );
                    }
                }
                
                // Outer ring: Shadow flame
                for (int i = 0; i < 8; i++)
                {
                    float angle = (i / 8f) * MathHelper.TwoPi + MathHelper.Pi / 8f; // Offset
                    Vector2 pos = center + new Vector2((float)System.Math.Cos(angle) * outerRadius, (float)System.Math.Sin(angle) * outerRadius);
                    
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(
                            Main.LocalPlayer.GetSource_FromThis(),
                            pos,
                            Vector2.Zero,
                            ProjectileID.ShadowFlame,
                            35, // Damage
                            0f,
                            Main.myPlayer
                        );
                    }
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
                
                // MOVIDO PARA CIMA: -120 pixels do centro da zona
                Vector2 textPos = screenPos - textSize / 2f - new Vector2(0, 120);
                
                // Desenha sombra preta
                Terraria.Utils.DrawBorderString(
                    spriteBatch,
                    text,
                    textPos,
                    Color.Black * alpha,
                    1.2f  // Aumentado de 1f para 1.2f (texto maior)
                );
                
                // Desenha texto verde
                Terraria.Utils.DrawBorderString(
                    spriteBatch,
                    text,
                    textPos,
                    textColor,
                    1.2f  // Aumentado de 1f para 1.2f (texto maior)
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
