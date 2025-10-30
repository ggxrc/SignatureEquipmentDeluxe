using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SignatureEquipmentDeluxe.Common.Systems;
using SignatureEquipmentDeluxe.Common.Configs;
using Terraria.Graphics.Shaders;

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
        private const float ASCENSION_HEIGHT = 15f; // 15 tiles de altura
        private const int ASCENSION_DURATION = 120; // 2 segundos para subir e acelerar giro
        private const int DECELERATION_DURATION = 240; // 4 segundos para desacelerar
        
        private Vector2 startPosition;
        private int weaponLevel;
        private int itemType;
        private bool hasCreatedZone = false;
        private bool hasExploded = false;
        private Texture2D weaponTexture;
        private float zoneRadius;
        private int totalDuration; // Dura até o fim da zona
        private float currentRotation; // Acumulador para rotação suave
        
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
            
            // Dura 10 minutos (mesma duração da zona)
            var config = ModContent.GetInstance<ServerConfig>();
            totalDuration = 10 * 60 * 60;
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
            
            // === FASE 1: ASCENSÃO (0-120 frames / 2 segundos) ===
            if (elapsedTime < ASCENSION_DURATION)
            {
                PhaseAscension(elapsedTime);
            }
            // === FASE 2: DESACELERAÇÃO (120-360 frames / 4 segundos) ===
            else if (elapsedTime < ASCENSION_DURATION + DECELERATION_DURATION)
            {
                PhaseDeceleration(elapsedTime - ASCENSION_DURATION);
            }
            // Após desaceleração, dropar arma e sumir
            else
            {
                DropWeapon();
                Projectile.Kill();
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
            currentRotation += rotationSpeed;
            Projectile.rotation = currentRotation;
            
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
            
            // Som de energia crescente (uma vez no meio)
            if (frame == ASCENSION_DURATION / 2)
            {
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
            }
        }
        
        /// <summary>
        /// Fase 2: Desaceleração do giro até parar, com explosão no frame 90 (3.5s)
        /// </summary>
        private void PhaseDeceleration(int frame)
        {
            // Explosão no frame 90 (3.5 segundos)
            if (frame == 90)
            {
                // SOM ÉPICO
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item62, Projectile.Center);
                Terraria.Audio.SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
                
                // Flash de luz intenso
                Lighting.AddLight(Projectile.Center, 10f, 10f, 10f);
                
                // Efeito no centro: partículas em forma de estrela indicando o ponto de origem
                for (int i = 0; i < 8; i++) // 8 direções para estrela
                {
                    float angle = (i / 8f) * MathHelper.TwoPi;
                    Vector2 starPos = Projectile.Center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 20f;
                    Dust starDust = Dust.NewDustPerfect(
                        starPos,
                        DustID.CursedTorch,
                        Vector2.Zero,
                        0,
                        Color.Yellow,
                        3f
                    );
                    starDust.noGravity = true;
                    starDust.fadeIn = 2f;
                }
                
                // Cria a zona radioativa
                CreateRadioactiveZone();
            }
            
            // Expansão da explosão (120 frames)
            if (frame >= 90)
            {
                float explosionFrame = frame - 90;
                float explosionProgress = explosionFrame / 120f;
                
                if (explosionProgress <= 1f)
                {
                    float expansionRadius = explosionProgress * zoneRadius;
                    
                    // Círculo de partículas expandindo
                    int circleParticles = 100;
                    for (int i = 0; i < circleParticles; i++)
                    {
                        float angle = (i / (float)circleParticles) * MathHelper.TwoPi;
                        Vector2 circlePos = Projectile.Center + new Vector2(
                            (float)Math.Cos(angle) * expansionRadius,
                            (float)Math.Sin(angle) * expansionRadius
                        );
                        
                        Dust circleDust = Dust.NewDustPerfect(
                            circlePos,
                            DustID.CursedTorch,
                            Vector2.Zero,
                            0,
                            Color.LimeGreen,
                            2f
                        );
                        circleDust.noGravity = true;
                        circleDust.fadeIn = 1.5f;
                        
                        if (Main.rand.NextBool(3))
                        {
                            Vector2 trailPos = circlePos + Main.rand.NextVector2Circular(10f, 10f);
                            Dust trailDust = Dust.NewDustPerfect(
                                trailPos,
                                DustID.CursedTorch,
                                Vector2.Zero,
                                0,
                                Color.Green,
                                1.5f
                            );
                            trailDust.noGravity = true;
                            trailDust.fadeIn = 1f;
                        }
                    }
                    
                    if (explosionFrame % 5 == 0)
                    {
                        for (int i = 0; i < 50; i++)
                        {
                            Vector2 randomPos = Projectile.Center + Main.rand.NextVector2Circular(expansionRadius, expansionRadius);
                            Dust chaos = Dust.NewDustPerfect(
                                randomPos,
                                DustID.CursedTorch,
                                Vector2.Zero,
                                0,
                                Main.rand.NextBool() ? Color.Yellow : Color.LimeGreen,
                                Main.rand.NextFloat(1.5f, 3f)
                            );
                            chaos.noGravity = true;
                            chaos.fadeIn = Main.rand.NextFloat(1f, 2f);
                        }
                    }
                }
            }
            
            // Mantém a posição no topo
            float height = ASCENSION_HEIGHT * 16f;
            Projectile.Center = startPosition - new Vector2(0, height);
            
            // Desacelera o giro: começa em 0.15f e vai para 0 em 240 frames
            float progress = frame / (float)DECELERATION_DURATION;
            float rotationSpeed = 0.15f * (1f - progress);
            currentRotation += rotationSpeed;
            Projectile.rotation = currentRotation;
            
            // Partículas ocasionais diminuindo
            if (frame % (20 + frame / 10) == 0) // Menos partículas com o tempo
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 offset = Main.rand.NextVector2Circular(15f, 15f);
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center + offset,
                        DustID.CursedTorch,
                        new Vector2(0, Main.rand.NextFloat(0.2f, 0.8f)),
                        0,
                        Color.LimeGreen,
                        1f
                    );
                    dust.noGravity = true;
                }
            }
            
            // Luz diminuindo
            float lightIntensity = 0.8f * (1f - progress * 0.5f);
            Lighting.AddLight(Projectile.Center, 0.2f * lightIntensity, 0.8f * lightIntensity, 0.2f * lightIntensity);
        }
        private void DropWeapon()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Item.NewItem(Projectile.GetSource_FromThis(), Projectile.Center, itemType);
            }
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
                Position = Projectile.Center, // Usa posição atual (onde a arma cai após explosão)
                Radius = finalRadius * 16f, // Converte para pixels
                MaxEnemyLevel = weaponLevel,
                TimeLeft = totalDuration // 30 minutos
            };
            
            zone.InitialRadius = zone.Radius; // Salva raio inicial
            
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
