using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SignatureEquipmentDeluxe.Common.Systems;
using SignatureEquipmentDeluxe.Common.Configs;

namespace SignatureEquipmentDeluxe.Content.Projectiles
{
    /// <summary>
    /// Projectile de animação épica de ascensão da arma
    /// - Levita suavemente para cima girando devagar
    /// - Acelera o giro gradualmente
    /// - Explode em partículas verdes do tamanho da zona
    /// - Dura até o fim da zona (30 minutos)
    /// </summary>
    public class WeaponAscensionProjectile : ModProjectile
    {
        private const float ASCENSION_HEIGHT = 80f; // Altura máxima em tiles (reduzido de 200 para 80)
        private const int ASCENSION_DURATION = 300; // 5 segundos para subir
        private const int EXPLOSION_DURATION = 120; // 2 segundos de explosão
        
        private Vector2 startPosition;
        private int weaponLevel;
        private int itemType;
        private bool hasCreatedZone = false;
        private bool hasExploded = false;
        private Texture2D weaponTexture;
        private float zoneRadius;
        private int totalDuration; // Dura até o fim da zona
        
        // NÃO precisa de textura própria - usa a textura da arma
        public override string Texture => "Terraria/Images/Item_0";
        
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
            
            // Dura 30 minutos (mesma duração da zona)
            var config = ModContent.GetInstance<ServerConfig>();
            totalDuration = (config?.RadioactiveZoneDurationMinutes ?? 30) * 60 * 60;
            Projectile.timeLeft = totalDuration;
        }
        
        public override void AI()
        {
            // Inicialização
            if (Projectile.ai[0] == 0f)
            {
                startPosition = Projectile.Center;
                weaponLevel = (int)Projectile.ai[1];
                itemType = (int)Projectile.ai[2];
                
                // Calcula o raio da zona
                var config = ModContent.GetInstance<ServerConfig>();
                float baseRadius = (config?.RadioactiveZoneRadius ?? 150);
                zoneRadius = baseRadius * 2.5f * 16f; // 150% maior = 2.5x, tiles para pixels
                
                // Carrega a textura da arma
                Item tempItem = new Item();
                tempItem.SetDefaults(itemType);
                if (tempItem.type != ItemID.None)
                {
                    weaponTexture = Terraria.GameContent.TextureAssets.Item[itemType].Value;
                }
                
                Projectile.ai[0] = 1f;
            }
            
            int elapsedTime = totalDuration - Projectile.timeLeft;
            
            // === FASE 1: ASCENSÃO (0-300 frames / 5 segundos) ===
            if (elapsedTime < ASCENSION_DURATION)
            {
                PhaseAscension(elapsedTime);
            }
            // === FASE 2: EXPLOSÃO (300-420 frames / 2 segundos) ===
            else if (elapsedTime < ASCENSION_DURATION + EXPLOSION_DURATION)
            {
                PhaseExplosion(elapsedTime - ASCENSION_DURATION);
            }
            // === FASE 3: LEVITANDO (resto do tempo até fim da zona) ===
            else
            {
                PhaseHovering(elapsedTime - ASCENSION_DURATION - EXPLOSION_DURATION);
            }
        }
        
        /// <summary>
        /// Fase 1: Arma levita suavemente para cima girando devagar e acelerando
        /// </summary>
        private void PhaseAscension(int frame)
        {
            float progress = frame / (float)ASCENSION_DURATION;
            
            // Movimento SUAVE para cima (easing cubic out)
            float easedProgress = 1f - (float)Math.Pow(1f - progress, 3);
            float height = ASCENSION_HEIGHT * 16f * easedProgress;
            Projectile.Center = startPosition - new Vector2(0, height);
            
            // Rotação: começa DEVAGAR e ACELERA gradualmente
            // Velocidade inicial: 0.02 rad/frame (muito devagar)
            // Velocidade final: 0.15 rad/frame (rápido)
            float rotationSpeed = 0.02f + (progress * 0.13f);
            Projectile.rotation += rotationSpeed;
            
            // Partículas verdes caindo suavemente
            if (frame % 5 == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 offset = Main.rand.NextVector2Circular(30f, 30f);
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center + offset,
                        DustID.CursedTorch,
                        new Vector2(0, Main.rand.NextFloat(0.5f, 1.5f)), // Cai devagar
                        0,
                        Color.LimeGreen,
                        1.2f
                    );
                    dust.noGravity = true;
                }
            }
            
            // Trail de partículas
            if (frame % 3 == 0)
            {
                Dust trail = Dust.NewDustPerfect(
                    Projectile.Center,
                    DustID.CursedTorch,
                    Vector2.Zero,
                    0,
                    Color.Green,
                    1.5f
                );
                trail.noGravity = true;
                trail.fadeIn = 1f;
            }
            
            // Luz verde suave crescente
            float lightIntensity = progress * 0.8f;
            Lighting.AddLight(Projectile.Center, 0.2f * lightIntensity, 0.8f * lightIntensity, 0.2f * lightIntensity);
            
            // Som de energia crescente (a cada 2 segundos)
            if (frame % 120 == 0 && frame > 0)
            {
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
            }
        }
        
        /// <summary>
        /// Fase 2: EXPLOSÃO MASSIVA de partículas verdes do tamanho da zona
        /// </summary>
        private void PhaseExplosion(int frame)
        {
            // Cria a zona no primeiro frame da explosão
            if (!hasCreatedZone)
            {
                CreateRadioactiveZone();
                hasCreatedZone = true;
            }
            
            // Explosão inicial (frame 0)
            if (frame == 0 && !hasExploded)
            {
                hasExploded = true;
                
                // SOM ÉPICO
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item62, Projectile.Center);
                Terraria.Audio.SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
                
                // Flash de luz
                Lighting.AddLight(Projectile.Center, 5f, 5f, 5f);
                
                // EXPLOSÃO MASSIVA: Partículas expandem até o tamanho da zona
                int explosionParticles = 200;
                for (int i = 0; i < explosionParticles; i++)
                {
                    // Velocidade radial que alcança o raio da zona em ~2 segundos
                    float targetSpeed = zoneRadius / (EXPLOSION_DURATION * 0.8f); // 80% do tempo de explosão
                    Vector2 velocity = Main.rand.NextVector2CircularEdge(targetSpeed, targetSpeed);
                    
                    Dust explosion = Dust.NewDustPerfect(
                        Projectile.Center,
                        DustID.CursedTorch,
                        velocity,
                        0,
                        Color.Lime,
                        Main.rand.NextFloat(2f, 3.5f)
                    );
                    explosion.noGravity = true;
                    explosion.fadeIn = 1.5f;
                }
            }
            
            // Ondas de expansão contínuas
            float progress = frame / (float)EXPLOSION_DURATION;
            float expansionRadius = progress * zoneRadius;
            
            // Spawn onda de partículas expandindo
            if (frame % 8 == 0)
            {
                int waveParticles = 60;
                for (int i = 0; i < waveParticles; i++)
                {
                    float angle = (i / (float)waveParticles) * MathHelper.TwoPi;
                    Vector2 pos = Projectile.Center + new Vector2(
                        (float)Math.Cos(angle) * expansionRadius,
                        (float)Math.Sin(angle) * expansionRadius
                    );
                    
                    Dust wave = Dust.NewDustPerfect(pos, DustID.CursedTorch, Vector2.Zero, 0, Color.Green, 2f);
                    wave.noGravity = true;
                    wave.fadeIn = 1.5f;
                }
            }
            
            // Partículas aleatórias dentro da área em expansão
            if (frame % 3 == 0)
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 randomPos = Projectile.Center + Main.rand.NextVector2Circular(expansionRadius, expansionRadius);
                    Dust chaos = Dust.NewDustPerfect(randomPos, DustID.CursedTorch, Vector2.Zero, 0, Color.LimeGreen, 1.8f);
                    chaos.noGravity = true;
                }
            }
            
            // Arma continua girando rápido
            Projectile.rotation += 0.15f;
            
            // Luz pulsante intensa
            float lightIntensity = 2f * (1f - progress * 0.5f); // Diminui pela metade
            Lighting.AddLight(Projectile.Center, 0.5f * lightIntensity, lightIntensity, 0.5f * lightIntensity);
        }
        
        /// <summary>
        /// Fase 3: Arma levita na posição final até o fim da zona
        /// </summary>
        private void PhaseHovering(int frame)
        {
            // Mantém a posição no topo
            float height = ASCENSION_HEIGHT * 16f;
            Projectile.Center = startPosition - new Vector2(0, height);
            
            // Gira suavemente (velocidade constante)
            Projectile.rotation += 0.08f;
            
            // Partículas ocasionais
            if (frame % 20 == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 offset = Main.rand.NextVector2Circular(20f, 20f);
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center + offset,
                        DustID.CursedTorch,
                        new Vector2(0, Main.rand.NextFloat(0.3f, 1f)),
                        0,
                        Color.LimeGreen,
                        1.2f
                    );
                    dust.noGravity = true;
                }
            }
            
            // Luz verde constante
            Lighting.AddLight(Projectile.Center, 0.2f, 0.8f, 0.2f);
        }
        
        /// <summary>
        /// Cria a zona radioativa
        /// </summary>
        private void CreateRadioactiveZone()
        {
            if (hasCreatedZone) return;
            hasCreatedZone = true;
            
            float baseRadius = 150f; // 150 tiles base
            float radiusMultiplier = 2.5f;
            float finalRadius = baseRadius * radiusMultiplier;
            
            var zone = new LeveledEnemySystem.RadioactiveZone
            {
                Position = startPosition, // Usa posição inicial (chão)
                Radius = finalRadius * 16f, // Converte para pixels
                MaxEnemyLevel = weaponLevel,
                TimeLeft = totalDuration // 30 minutos
            };
            
            LeveledEnemySystem.radioactiveZones.Add(zone);
            
            // Mensagem dramática
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText($"A radioactive zone of level {weaponLevel} has been created!", Color.Red);
            }
        }
        
        public override bool PreDraw(ref Color lightColor)
        {
            // Se não tem textura carregada, não desenha nada
            if (weaponTexture == null)
                return false;
            
            Texture2D texture = weaponTexture;
            Rectangle sourceRect = texture.Frame(1, 1);
            Vector2 origin = sourceRect.Size() / 2f;
            
            // Brilho suave
            Color drawColor = Color.White;
            
            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                sourceRect,
                drawColor * (1f - Projectile.alpha / 255f),
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );
            
            return false;
        }
    }
}
