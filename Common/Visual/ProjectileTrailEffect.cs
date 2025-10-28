using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace SignatureEquipmentDeluxe.Common.Visual
{
    /// <summary>
    /// Sistema de efeitos visuais quando projéteis de armas de alto nível acertam alvos
    /// </summary>
    public class ProjectileHitEffect
    {
        /// <summary>
        /// Spawna efeito visual quando um projétil acerta um alvo
        /// </summary>
        public static void SpawnHitEffect(Vector2 position, Color baseColor, int itemLevel, int damage, DamageClass damageType = null)
        {
            var clientConfig = ModContent.GetInstance<Configs.ClientConfig>();
            
            if (!clientConfig.EnableProjectileHitEffects || !clientConfig.EnableParticleEffects)
                return;
            
            if (itemLevel < clientConfig.HitEffectsMinLevel)
                return;
            
            // Determina intensidade baseada no nível
            HitEffectStyle style = GetHitEffectStyle(itemLevel);
            
            // Gera cor aleatória ao invés de usar baseColor
            Color randomColor = GetRandomHitColor();
            
            // Spawna partículas de impacto
            SpawnImpactParticles(position, randomColor, style, clientConfig, damage, damageType);
        }
        
        /// <summary>
        /// Retorna uma cor aleatória para o efeito de hit
        /// </summary>
        private static Color GetRandomHitColor()
        {
            // Paleta de cores vibrantes para efeitos de hit
            Color[] colors = new Color[]
            {
                new Color(255, 100, 100),  // Vermelho
                new Color(100, 150, 255),  // Azul
                new Color(100, 255, 100),  // Verde
                new Color(255, 200, 100),  // Laranja
                new Color(255, 100, 255),  // Magenta
                new Color(100, 255, 255),  // Ciano
                new Color(255, 255, 100),  // Amarelo
                new Color(200, 100, 255),  // Roxo
            };
            
            return colors[Main.rand.Next(colors.Length)];
        }
        
        /// <summary>
        /// Determina o estilo de efeito baseado no nível
        /// </summary>
        private static HitEffectStyle GetHitEffectStyle(int level)
        {
            if (level >= 100)
                return HitEffectStyle.Explosive;
            if (level >= 50)
                return HitEffectStyle.Strong;
            
            return HitEffectStyle.Normal;
        }
        
        /// <summary>
        /// Spawna partículas de impacto
        /// </summary>
        private static void SpawnImpactParticles(Vector2 position, Color color, HitEffectStyle style, Configs.ClientConfig config, int damage, DamageClass damageType)
        {
            // Calcula quantidade de partículas baseado no estilo e intensidade
            int particleCount = style switch
            {
                HitEffectStyle.Normal => (int)(5 * config.HitEffectsIntensity),
                HitEffectStyle.Strong => (int)(10 * config.HitEffectsIntensity),
                HitEffectStyle.Explosive => (int)(15 * config.HitEffectsIntensity),
                _ => 5
            };
            
            if (config.ReducedAnimations)
                particleCount /= 2;
            
            particleCount = Math.Max(1, particleCount);
            
            // Spawna partículas em explosão radial
            for (int i = 0; i < particleCount; i++)
            {
                float angle = MathHelper.TwoPi * i / particleCount + Main.rand.NextFloat(-0.3f, 0.3f);
                float speed = style switch
                {
                    HitEffectStyle.Normal => Main.rand.NextFloat(2f, 4f),
                    HitEffectStyle.Strong => Main.rand.NextFloat(3f, 6f),
                    HitEffectStyle.Explosive => Main.rand.NextFloat(4f, 8f),
                    _ => 3f
                };
                
                Vector2 velocity = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );
                
                // Escolhe tipo de dust baseado no estilo
                int dustType = GetDustTypeForStyle(style);
                float scale = GetScaleForStyle(style);
                
                Dust dust = Dust.NewDustPerfect(position, dustType, velocity, 0, color, scale);
                dust.noGravity = true;
                dust.fadeIn = 1f;
                
                // Efeito especial para Explosive
                if (style == HitEffectStyle.Explosive)
                {
                    dust.scale *= Main.rand.NextFloat(1f, 1.5f);
                }
            }
            
            // Partículas específicas por tipo de dano
            if (damageType != null && !config.ReducedAnimations)
            {
                SpawnDamageTypeParticles(position, damageType, style);
            }
            
            // Efeito adicional para estilos mais fortes
            if (style == HitEffectStyle.Strong || style == HitEffectStyle.Explosive)
            {
                // Onda de choque central - 50% menor
                for (int i = 0; i < 3; i++)
                {
                    Dust shockwave = Dust.NewDustPerfect(position, DustID.RainbowMk2, Vector2.Zero, 0, color, 1f);
                    shockwave.noGravity = true;
                    shockwave.fadeIn = 1f;
                }
            }
            
            // Efeito de explosão massiva para Explosive
            if (style == HitEffectStyle.Explosive && !config.ReducedAnimations)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 sparkVel = new Vector2(
                        Main.rand.NextFloat(-3f, 3f),
                        Main.rand.NextFloat(-3f, 3f)
                    );
                    
                    Dust spark = Dust.NewDustPerfect(position, DustID.FireworkFountain_Yellow, sparkVel, 0, color, 0.75f);
                    spark.noGravity = true;
                }
            }
        }
        
        /// <summary>
        /// Spawna partículas específicas baseadas no tipo de dano da arma
        /// </summary>
        private static void SpawnDamageTypeParticles(Vector2 position, DamageClass damageType, HitEffectStyle style)
        {
            int particleCount = style switch
            {
                HitEffectStyle.Normal => 2,
                HitEffectStyle.Strong => 3,
                HitEffectStyle.Explosive => 5,
                _ => 2
            };
            
            // Melee: Sparks metálicos
            if (damageType == DamageClass.Melee || damageType == DamageClass.MeleeNoSpeed)
            {
                for (int i = 0; i < particleCount; i++)
                {
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f));
                    Dust dust = Dust.NewDustPerfect(position, DustID.Iron, velocity, 0, default, 1.2f);
                    dust.noGravity = false; // Sparks caem
                    dust.fadeIn = 0.8f;
                }
            }
            // Ranged: Casings de balas / flechas
            else if (damageType == DamageClass.Ranged)
            {
                for (int i = 0; i < particleCount; i++)
                {
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-3f, -1f));
                    Dust dust = Dust.NewDustPerfect(position, DustID.Copper, velocity, 0, new Color(255, 200, 100), 0.8f);
                    dust.noGravity = false;
                }
            }
            // Magic: Estrelas mágicas
            else if (damageType == DamageClass.Magic)
            {
                for (int i = 0; i < particleCount; i++)
                {
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f));
                    Dust dust = Dust.NewDustPerfect(position, DustID.MagicMirror, velocity, 0, new Color(150, 100, 255), 1f);
                    dust.noGravity = true;
                    dust.fadeIn = 1.2f;
                }
            }
            // Summon: Partículas etéreas
            else if (damageType == DamageClass.Summon || damageType == DamageClass.SummonMeleeSpeed)
            {
                for (int i = 0; i < particleCount; i++)
                {
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
                    Dust dust = Dust.NewDustPerfect(position, DustID.Shadowflame, velocity, 0, new Color(200, 100, 255), 1f);
                    dust.noGravity = true;
                    dust.fadeIn = 1f;
                }
            }
            // Throwing: Pequenas estilhaços
            else if (damageType == DamageClass.Throwing)
            {
                for (int i = 0; i < particleCount; i++)
                {
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f));
                    Dust dust = Dust.NewDustPerfect(position, DustID.Glass, velocity, 0, new Color(150, 255, 150), 0.8f);
                    dust.noGravity = false;
                }
            }
        }
        
        private static int GetDustTypeForStyle(HitEffectStyle style)
        {
            return style switch
            {
                HitEffectStyle.Normal => DustID.MagicMirror,
                HitEffectStyle.Strong => DustID.Firework_Blue,
                HitEffectStyle.Explosive => DustID.FireworkFountain_Red,
                _ => DustID.MagicMirror
            };
        }
        
        private static float GetScaleForStyle(HitEffectStyle style)
        {
            // Reduzido em 50% (era 1.2f, 1.5f, 2f)
            return style switch
            {
                HitEffectStyle.Normal => 0.6f,
                HitEffectStyle.Strong => 0.75f,
                HitEffectStyle.Explosive => 1f,
                _ => 0.5f
            };
        }
    }
    
    /// <summary>
    /// Estilos de efeito de hit
    /// </summary>
    public enum HitEffectStyle
    {
        Normal,     // Efeito básico (level 10-49)
        Strong,     // Efeito forte (level 50-99)
        Explosive   // Efeito explosivo (level 100+)
    }
}
