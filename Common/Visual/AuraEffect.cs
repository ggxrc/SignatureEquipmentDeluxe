using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SignatureEquipmentDeluxe.Common.Systems;

namespace SignatureEquipmentDeluxe.Common.Visual
{
    /// <summary>
    /// Sistema de auras visuais para equipamentos de alto nível
    /// </summary>
    public class AuraEffect
    {
        private static int particleSpawnCounter = 0;
        
        /// <summary>
        /// Atualiza e renderiza auras ao redor do player baseado nos itens equipados
        /// </summary>
        public static void UpdatePlayerAura(Player player)
        {
            var clientConfig = ModContent.GetInstance<Configs.ClientConfig>();
            
            if (!clientConfig.EnableAuraEffects || !clientConfig.EnableParticleEffects)
                return;
            
            // Limita spawn de partículas
            particleSpawnCounter++;
            int spawnRate = Math.Max(1, 60 / clientConfig.MaxParticlesPerSecond);
            
            if (particleSpawnCounter < spawnRate)
                return;
            
            particleSpawnCounter = 0;
            
            // Verifica item principal (arma equipada)
            Item heldItem = player.HeldItem;
            if (heldItem == null || heldItem.IsAir)
                return;
            
            var globalItem = heldItem.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
            if (globalItem == null || globalItem.Level < clientConfig.AuraLevel_Weak)
                return;
            
            // Determina intensidade baseada no nível
            AuraIntensity intensity = GetAuraIntensity(globalItem.Level, clientConfig);
            
            if (intensity == AuraIntensity.None)
                return;
            
            // Verifica se player está idle
            var signaturePlayer = player.GetModPlayer<Players.SignaturePlayer>();
            bool isIdle = signaturePlayer != null && signaturePlayer.IsIdleForAura;
            
            // Spawna partículas de aura
            SpawnAuraParticles(player, clientConfig.AuraStyle, intensity, clientConfig.ReducedAnimations, isIdle);
        }
        
        /// <summary>
        /// Determina a intensidade da aura baseado no nível
        /// </summary>
        private static AuraIntensity GetAuraIntensity(int level, Configs.ClientConfig config)
        {
            if (level >= config.AuraLevel_Intense)
                return AuraIntensity.Intense;
            if (level >= config.AuraLevel_Strong)
                return AuraIntensity.Strong;
            if (level >= config.AuraLevel_Medium)
                return AuraIntensity.Medium;
            if (level >= config.AuraLevel_Weak)
                return AuraIntensity.Weak;
            
            return AuraIntensity.None;
        }
        
        /// <summary>
        /// Spawna partículas de aura ao redor do player
        /// </summary>
        private static void SpawnAuraParticles(Player player, AuraStyle style, AuraIntensity intensity, bool reduced, bool isIdle)
        {
            var clientConfig = ModContent.GetInstance<Configs.ClientConfig>();
            
            // Número de partículas REDUZIDO - mais balanceado
            int baseParticleCount = intensity switch
            {
                AuraIntensity.Weak => reduced ? 1 : 2,
                AuraIntensity.Medium => reduced ? 2 : 3,
                AuraIntensity.Strong => reduced ? 2 : 4,
                AuraIntensity.Intense => reduced ? 3 : 5,
                _ => 0
            };
            
            // Aplica multiplicador de intensidade da config
            baseParticleCount = (int)(baseParticleCount * clientConfig.AuraIntensityMultiplier);
            baseParticleCount = Math.Max(1, baseParticleCount); // Mínimo de 1
            
            // Quando idle, spawna 3x mais partículas e com efeito especial
            int particleCount = isIdle ? baseParticleCount * 3 : baseParticleCount;
            
            for (int i = 0; i < particleCount; i++)
            {
                // Aura mais próxima e densa
                float angle = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                float baseDistance = isIdle ? 40f : 20f; // Mais próximo para ser mais visível
                float distance = baseDistance + Main.rand.NextFloat(0f, isIdle ? 50f : 30f);
                
                Vector2 offset = new Vector2(
                    (float)Math.Cos(angle) * distance,
                    (float)Math.Sin(angle) * distance
                );
                
                Vector2 position = player.Center + offset;
                
                // Velocidade mais caótica - não mais orbital perfeito
                Vector2 velocity;
                if (isIdle)
                {
                    // Idle: movimento suave em espiral convergente
                    float spiralSpeed = 0.8f; // Mais rápido
                    velocity = new Vector2(
                        (float)Math.Cos(angle + MathHelper.PiOver2) * spiralSpeed - offset.X * 0.015f,
                        (float)Math.Sin(angle + MathHelper.PiOver2) * spiralSpeed - offset.Y * 0.015f
                    );
                }
                else
                {
                    velocity = style == AuraStyle.Electric 
                        ? new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f)) // Electric totalmente caótico e mais rápido
                        : new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2.5f, 1.5f)); // Outros estilos mais dinâmicos
                }
                
                // Cria dust baseado no estilo
                int dustType = GetDustTypeForStyle(style);
                Color dustColor = GetColorForStyle(style);
                float scale = GetScaleForIntensity(intensity);
                
                // Aplica multiplicador de intensidade
                scale *= clientConfig.AuraIntensityMultiplier;
                
                // Partículas com tamanho balanceado
                dustColor = Color.Lerp(dustColor, Color.White, 0.15f); // Levemente iluminadas
                
                // Idle: partículas maiores e mais brilhantes
                if (isIdle)
                {
                    scale *= 1.3f; // Adicional quando idle (era 1.4f)
                    dustColor = Color.Lerp(dustColor, Color.White, 0.4f); // Brilhante (era 0.5f)
                }
                
                Dust dust = Dust.NewDustPerfect(position, dustType, velocity, 0, dustColor, scale);
                dust.noGravity = true;
                dust.fadeIn = isIdle ? 1.5f : 1.2f; // Fade mais intenso
                
                // Efeito especial para Rainbow - mais saturado
                if (style == AuraStyle.Rainbow)
                {
                    dust.color = Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.5f + Main.rand.NextFloat(0f, 0.3f)) % 1f, 1f, isIdle ? 0.9f : 0.7f);
                    dust.scale *= 1.2f; // Rainbow ainda maior
                }
                
                // Efeito especial para Electric - mais errático e brilhante
                if (style == AuraStyle.Electric)
                {
                    dust.scale *= Main.rand.NextFloat(1f, 1.6f); // Variação maior
                    dust.fadeIn *= 1.3f; // Mais brilho
                }
            }
        }
        
        /// <summary>
        /// Retorna o tipo de dust apropriado para cada estilo
        /// </summary>
        private static int GetDustTypeForStyle(AuraStyle style)
        {
            return style switch
            {
                AuraStyle.Magic => DustID.MagicMirror,
                AuraStyle.Fire => DustID.Torch,
                AuraStyle.Electric => DustID.Electric,
                AuraStyle.Rainbow => DustID.RainbowMk2,
                AuraStyle.Shadow => DustID.Shadowflame,
                AuraStyle.Holy => DustID.GoldCoin,
                _ => DustID.MagicMirror
            };
        }
        
        /// <summary>
        /// Retorna a cor base para cada estilo
        /// </summary>
        private static Color GetColorForStyle(AuraStyle style)
        {
            return style switch
            {
                AuraStyle.Magic => new Color(150, 100, 255),
                AuraStyle.Fire => new Color(255, 100, 50),
                AuraStyle.Electric => new Color(255, 255, 100),
                AuraStyle.Rainbow => Color.White,
                AuraStyle.Shadow => new Color(150, 50, 200),
                AuraStyle.Holy => new Color(255, 240, 150),
                _ => Color.White
            };
        }
        
        /// <summary>
        /// Retorna a escala das partículas baseado na intensidade
        /// </summary>
        private static float GetScaleForIntensity(AuraIntensity intensity)
        {
            // Escalas REDUZIDAS - mais balanceadas
            return intensity switch
            {
                AuraIntensity.Weak => 0.8f,      // Era 1.2f
                AuraIntensity.Medium => 1.0f,    // Era 1.6f
                AuraIntensity.Strong => 1.3f,    // Era 2.2f
                AuraIntensity.Intense => 1.6f,   // Era 2.8f
                _ => 1.0f
            };
        }
    }
    
    /// <summary>
    /// Intensidades de aura
    /// </summary>
    public enum AuraIntensity
    {
        None,
        Weak,
        Medium,
        Strong,
        Intense
    }
}
