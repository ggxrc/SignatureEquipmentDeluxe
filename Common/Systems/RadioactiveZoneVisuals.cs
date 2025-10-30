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
                
                // Partículas caóticas reduzidas (menos frequência)
                if (chaosTimer % 8 == 0) // Menos frequente
                {
                    SpawnChaoticParticles(zone);
                }
            }
        }
        
        /// <summary>
        /// Spawn partículas verdes ao redor da zona radioativa
        /// Partículas aumentam gradualmente do centro para a borda
        /// Danger level aumenta a intensidade das cores
        /// </summary>
        private void SpawnRadioactiveParticles(LeveledEnemySystem.RadioactiveZone zone)
        {
            var config = ModContent.GetInstance<Configs.ServerConfig>();
            if (!config.EnableLeveledEnemies)
                return;
            
            float radius = zone.Radius;
            Vector2 center = zone.Position;
            
            // MULTIPLICADOR DE PERIGO: aumenta brilho e quantidade
            float dangerMultiplier = zone.GetParticleMultiplier();
            Color dangerColor = zone.GetDangerColor();
            
            // Spawn partículas em anéis concêntricos mais organizados
            int ringCount = 4; // Menos anéis para menos caos
            
            for (int ring = 0; ring < ringCount; ring++)
            {
                // Distância normalizada do centro (0.0 = centro, 1.0 = borda)
                float normalizedDistance = (ring + 1) / (float)ringCount;
                float ringRadius = radius * normalizedDistance;
                
                // Densidade aumenta gradualmente da borda para o centro
                // Centro: mais partículas, borda: menos
                float distanceFromCenter = 1f - normalizedDistance; // 1.0 no centro, 0.0 na borda
                float densityMultiplier = 1.0f + (distanceFromCenter * 2.0f); // 1x na borda, 3x no centro
                densityMultiplier *= dangerMultiplier; // Aplica multiplicador de perigo
                
                // Partículas por anel (reduzido para menos caos)
                int baseParticles = ring == 0 ? 12 : 8; // Menos partículas
                int particleCount = (int)(baseParticles * densityMultiplier);
                
                for (int i = 0; i < particleCount; i++)
                {
                    // Posição mais organizada ao redor do anel
                    float angle = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                    float distanceVariation = 0.85f + (0.15f * distanceFromCenter); // Menos variação
                    float distance = ringRadius * Main.rand.NextFloat(0.9f, distanceVariation);
                    
                    Vector2 particlePos = center + new Vector2(
                        (float)System.Math.Cos(angle) * distance,
                        (float)System.Math.Sin(angle) * distance
                    );
                    
                    // Tamanho aumenta no centro
                    float scale = 0.8f + (distanceFromCenter * 0.7f); // 0.8x na borda, 1.5x no centro
                    
                    // Cor baseada no danger level
                    Color particleColor = Color.Lerp(Color.Green, dangerColor, distanceFromCenter * 0.7f);
                    
                    // Velocity DIRECIONADA AO CENTRO COM FORÇA CRESCENTE
                    Vector2 directionToCenter = Vector2.Normalize(center - particlePos);
                    
                    // FORÇA DE ATRACÃO CRESCENTE: aumenta com o tempo e danger level
                    float basePullStrength = 0.3f + (distanceFromCenter * 0.7f); // Mais força na borda
                    float timeMultiplier = 1f + (zone.TimeLeft / 36000f) * 2f; // Aumenta com o tempo (era 10min = 36000 ticks)
                    float dangerMultiplierPull = 1f + (zone.DangerLevel * 0.3f); // Danger level aumenta atração
                    
                    float pullStrength = basePullStrength * timeMultiplier * dangerMultiplierPull;
                    
                    // NOS SEGUNDOS FINAIS: TODAS as partículas são puxadas com força extrema
                    if (zone.IsFinalCountdown)
                    {
                        pullStrength *= 3f; // 3x mais força nos segundos finais para TODAS as partículas
                    }
                    
                    Vector2 velocity = directionToCenter * pullStrength;
                    
                    // Spawn dust e modificar cor
                    Dust dust = Dust.NewDustPerfect(particlePos, DustID.CursedTorch, velocity, 0, default(Color), scale);
                    dust.color = particleColor; // Define cor customizada
                    dust.noGravity = true;
                    
                    dust.fadeIn = 0.5f + (distanceFromCenter * 0.5f);
                }
            }
            
            // EFEITOS CENTRAIS MAIS ORGANIZADOS
            SpawnCenterEffects(zone, center, dangerMultiplier, dangerColor);
            
            // PARTÍCULAS ALEATÓRIAS NO CAMPO DE VISÃO DO JOGADOR
            SpawnScreenParticles(zone, dangerMultiplier, dangerColor);
        }
        
        /// <summary>
        /// Efeitos visuais organizados no centro da zona
        /// Substitui a "bola de partículas" caótica por indicadores mais claros
        /// </summary>
        private void SpawnCenterEffects(LeveledEnemySystem.RadioactiveZone zone, Vector2 center, float dangerMultiplier, Color dangerColor)
        {
            // PULSAÇÃO CENTRAL: Anéis pulsantes indicando o centro perigoso
            if (visualTimer % 20 == 0)
            {
                float pulseRadius = 30f + (float)System.Math.Sin(visualTimer * 0.1f) * 10f;
                int particlesInPulse = (int)(16 * dangerMultiplier);
                
                for (int i = 0; i < particlesInPulse; i++)
                {
                    float angle = MathHelper.TwoPi * i / particlesInPulse;
                    Vector2 pos = center + new Vector2((float)System.Math.Cos(angle) * pulseRadius, (float)System.Math.Sin(angle) * pulseRadius);
                    
                    Dust pulse = Dust.NewDustPerfect(pos, DustID.CursedTorch, Vector2.Zero, 0, dangerColor, 1.2f);
                    pulse.noGravity = true;
                    pulse.fadeIn = 1.5f;
                }
            }
            
            // INDICADOR DE PERIGO: Partículas subindo do centro
            if (visualTimer % 15 == 0)
            {
                int risingParticles = (int)(8 * dangerMultiplier);
                for (int i = 0; i < risingParticles; i++)
                {
                    Vector2 startPos = center + Main.rand.NextVector2Circular(20f, 20f);
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-3f, -1f)); // Para cima
                    
                    Dust rising = Dust.NewDustPerfect(startPos, DustID.CursedTorch, velocity, 0, dangerColor, 0.8f);
                    rising.noGravity = true;
                    rising.fadeIn = 1.0f;
                }
            }
            
            // FINAL COUNTDOWN: Efeitos mais intensos no centro
            if (zone.IsFinalCountdown)
            {
                if (visualTimer % 10 == 0)
                {
                    // Anéis concêntricos se contraindo
                    for (int ring = 0; ring < 3; ring++)
                    {
                        float contractRadius = 40f - ring * 10f;
                        int particlesInRing = 12;
                        
                        for (int i = 0; i < particlesInRing; i++)
                        {
                            float angle = MathHelper.TwoPi * i / particlesInRing + visualTimer * 0.1f; // Rotação
                            Vector2 pos = center + new Vector2((float)System.Math.Cos(angle) * contractRadius, (float)System.Math.Sin(angle) * contractRadius);
                            
                            Dust contract = Dust.NewDustPerfect(pos, DustID.Electric, Vector2.Zero, 0, default(Color), 1.5f);
                            contract.color = Color.Red; // Define cor customizada
                            contract.noGravity = true;
                            contract.fadeIn = 2.0f;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Spawn partículas aleatórias no campo de visão do jogador para otimização
        /// Só spawna quando o jogador está próximo da zona
        /// </summary>
        private void SpawnScreenParticles(LeveledEnemySystem.RadioactiveZone zone, float dangerMultiplier, Color dangerColor)
        {
            // Só spawna se o jogador estiver próximo da zona (otimização)
            Vector2 playerCenter = Main.LocalPlayer.Center;
            float distanceToZone = Vector2.Distance(playerCenter, zone.Position);
            
            if (distanceToZone > zone.Radius * 1.5f) return; // Jogador muito longe
            
            // Spawn partículas aleatórias na tela do jogador
            if (visualTimer % 15 == 0) // Menos frequente que os anéis
            {
                int screenParticles = (int)(3 * dangerMultiplier);
                
                for (int i = 0; i < screenParticles; i++)
                {
                    // Posição aleatória dentro da tela do jogador, mas próxima da zona
                    Vector2 screenCenter = Main.screenPosition + new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);
                    Vector2 randomOffset = Main.rand.NextVector2Circular(Main.screenWidth / 3f, Main.screenHeight / 3f);
                    Vector2 particlePos = screenCenter + randomOffset;
                    
                    // Garante que está dentro da zona (aproximadamente)
                    float distanceFromZoneCenter = Vector2.Distance(particlePos, zone.Position);
                    if (distanceFromZoneCenter > zone.Radius * 0.8f) continue; // Fora da zona
                    
                    // Tipo de partícula baseado no tier
                    int dustType = zone.DangerLevel switch
                    {
                        1 => DustID.CursedTorch,
                        2 => DustID.GreenTorch,
                        3 => DustID.Electric,
                        4 => DustID.Shadowflame,
                        5 => DustID.InfernoFork,
                        _ => DustID.CursedTorch
                    };
                    
                    // Velocidade direcionada ao centro com variação
                    Vector2 directionToCenter = Vector2.Normalize(zone.Position - particlePos);
                    Vector2 baseVelocity = directionToCenter * 0.8f; // Componente direcionada
                    Vector2 randomVelocity = Main.rand.NextVector2Circular(0.3f, 0.3f); // Variação aleatória
                    Vector2 velocity = baseVelocity + randomVelocity;
                    
                    // Tamanho pequeno
                    float scale = Main.rand.NextFloat(0.3f, 0.8f);
                    
                    Dust screenDust = Dust.NewDustPerfect(particlePos, dustType, velocity, 0, default(Color), scale);
                    screenDust.color = dangerColor;
                    screenDust.noGravity = true;
                    screenDust.fadeIn = Main.rand.NextFloat(0.5f, 1.5f);
                    
                    // Partículas de tier alto têm mais brilho
                    if (zone.DangerLevel >= 4)
                    {
                        screenDust.scale *= 1.2f;
                        screenDust.fadeIn *= 1.5f;
                    }
                }
            }
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
            
            // Quantidade base de partículas caóticas (reduzida)
            int baseParticles = 4; // Era 8, reduzido
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
                
                // Velocidade DIRECIONADA AO CENTRO COM FORÇA CRESCENTE
                Vector2 directionToCenter = Vector2.Normalize(center - particlePos);
                
                // FORÇA DE ATRACÃO CRESCENTE: mesma lógica das outras partículas
                float distanceFromCenter = Vector2.Distance(particlePos, center) / radius;
                float basePullStrength = 0.8f + (distanceFromCenter * 1.2f); // Mais força na borda
                float timeMultiplier = 1f + (zone.TimeLeft / 36000f) * 2f; // Aumenta com o tempo
                float dangerMultiplierPull = 1f + (zone.DangerLevel * 0.3f); // Danger level aumenta atração
                
                float pullStrength = basePullStrength * timeMultiplier * dangerMultiplierPull;
                
                // NOS SEGUNDOS FINAIS: TODAS as partículas caóticas também são puxadas
                if (zone.IsFinalCountdown)
                {
                    pullStrength *= 4f; // 4x mais força nos segundos finais para TODAS as partículas caóticas
                }
                
                Vector2 baseVelocity = directionToCenter * pullStrength;
                Vector2 chaoticVelocity = Main.rand.NextVector2Circular(0.3f, 0.3f); // Caos reduzido
                Vector2 velocity = baseVelocity + chaoticVelocity;
                
                // Tamanho aleatório
                float scale = Main.rand.NextFloat(1f, 2.5f);
                
                // Cor mista entre verde e cor de perigo
                Color particleColor = Main.rand.NextBool() ? Color.LimeGreen : dangerColor;
                
                Dust chaos = Dust.NewDustPerfect(particlePos, dustType, velocity, 0, default(Color), scale);
                chaos.color = particleColor; // Define cor customizada
                chaos.noGravity = Main.rand.NextBool(3); // 66% sem gravidade, 33% com gravidade (mais variedade)
                
                chaos.fadeIn = Main.rand.NextFloat(0.5f, 1.5f);
            }
            
            // Partículas extras flutuando para cima (efeito de fumaça radioativa)
            if (visualTimer % 10 == 0)
            {
                int smokeParticles = (int)(5 * dangerMultiplier);
                for (int i = 0; i < smokeParticles; i++)
                {
                    Vector2 randomPos = center + Main.rand.NextVector2Circular(radius * 0.8f, radius * 0.8f);
                    Dust smoke = Dust.NewDustPerfect(randomPos, DustID.Smoke, new Vector2(0, -Main.rand.NextFloat(0.5f, 2f)), 0, default(Color), Main.rand.NextFloat(1.5f, 2.5f));
                    smoke.color = Color.DarkGreen; // Define cor customizada
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
                        
                        Dust apocalypse = Dust.NewDustPerfect(pos, DustID.Torch, Vector2.Zero, 0, default(Color), 2.5f);
                        apocalypse.color = Color.DarkRed; // Define cor customizada
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
                
                // Desenha tempo restante abaixo (ajustado para velocidade 5x)
                int effectiveTimeLeft = zone.TimeLeft / 5; // Converte de volta para tempo "normal"
                int minutesLeft = effectiveTimeLeft / 3600; // 60 FPS * 60 segundos
                int secondsLeft = (effectiveTimeLeft / 60) % 60;
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
