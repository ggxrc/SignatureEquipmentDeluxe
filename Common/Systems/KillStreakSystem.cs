using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// Sistema de kill streak que rastreia kills consecutivos e mostra notificações
    /// </summary>
    public class KillStreakSystem : ModPlayer
    {
        // Estado do kill streak
        public int KillStreak { get; private set; } = 0;
        private int streakTimer = 0;
        private const int STREAK_TIMEOUT = 300; // 5 segundos sem kill para resetar
        
        // Visual pulsante
        private int pulseTimer = 0;
        private int lastDisplayedStreak = 0;
        
        // Cache de configs
        private Configs.ClientConfig clientConfig;
        private Configs.ServerConfig serverConfig;
        
        public override void Initialize()
        {
            KillStreak = 0;
            streakTimer = 0;
            pulseTimer = 0;
            lastDisplayedStreak = 0;
        }
        
        public override void PostUpdate()
        {
            // Atualiza timer do streak
            if (streakTimer > 0)
            {
                streakTimer--;
                
                // Resetar streak se timeout
                if (streakTimer <= 0 && KillStreak > 0)
                {
                    ResetStreak();
                }
            }
            
            // Atualiza timer de pulso
            if (pulseTimer > 0)
            {
                pulseTimer--;
            }
        }
        
        /// <summary>
        /// Adiciona um kill ao streak
        /// </summary>
        public void AddKill(Vector2 killPosition, int targetMaxLife)
        {
            serverConfig ??= ModContent.GetInstance<Configs.ServerConfig>();
            clientConfig ??= ModContent.GetInstance<Configs.ClientConfig>();
            
            if (!serverConfig.EnableKillStreakSystem)
                return;
            
            KillStreak++;
            streakTimer = STREAK_TIMEOUT;
            
            // Mostra visual se atingir milestone
            if (ShouldShowStreakVisual())
            {
                ShowStreakVisual(killPosition);
                lastDisplayedStreak = KillStreak;
                pulseTimer = 180; // 3 segundos de pulso
            }
        }
        
        /// <summary>
        /// Reseta o streak
        /// </summary>
        private void ResetStreak()
        {
            if (KillStreak > 0)
            {
                clientConfig ??= ModContent.GetInstance<Configs.ClientConfig>();
                
                // Notifica fim do streak se foi significativo
                if (KillStreak >= clientConfig.KillStreakVisualInterval && clientConfig.ShowStreakEndNotification)
                {
                    string endText = $"Kill Streak Ended: {KillStreak}";
                    CombatText.NewText(new Rectangle((int)Player.Center.X, (int)Player.Center.Y - 50, 10, 10), 
                        new Color(150, 150, 150), endText);
                }
                
                KillStreak = 0;
                pulseTimer = 0;
                lastDisplayedStreak = 0;
            }
        }
        
        /// <summary>
        /// Verifica se deve mostrar visual do streak
        /// </summary>
        private bool ShouldShowStreakVisual()
        {
            clientConfig ??= ModContent.GetInstance<Configs.ClientConfig>();
            
            // Mostra a cada X kills baseado na config
            return KillStreak % clientConfig.KillStreakVisualInterval == 0 && 
                   KillStreak >= clientConfig.KillStreakVisualInterval;
        }
        
        /// <summary>
        /// Mostra texto visual do kill streak com efeito pulsante
        /// </summary>
        private void ShowStreakVisual(Vector2 position)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            
            clientConfig ??= ModContent.GetInstance<Configs.ClientConfig>();
            
            // Determina cor e texto baseado no streak
            Color streakColor = GetStreakColor();
            string streakText = GetStreakText();
            
            // Mostra texto com efeito dramático
            CombatText.NewText(new Rectangle((int)position.X, (int)position.Y - 30, 10, 10), 
                streakColor, streakText, dramatic: true);
            
            // Som de streak
            if (clientConfig.EnableKillStreakSounds)
            {
                float pitch = Math.Min(1f, KillStreak * 0.03f); // Pitch aumenta com streak
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item4 with { Pitch = pitch, Volume = 0.8f }, position);
            }
            
            // Partículas de celebração
            if (clientConfig.EnableKillStreakParticles)
            {
                SpawnStreakParticles(position, streakColor);
            }
        }
        
        /// <summary>
        /// Retorna cor do streak baseado no número de kills
        /// </summary>
        private Color GetStreakColor()
        {
            if (KillStreak >= 100)
                return new Color(255, 0, 255); // Magenta lendário
            if (KillStreak >= 50)
                return new Color(255, 50, 50); // Vermelho épico
            if (KillStreak >= 30)
                return new Color(255, 150, 0); // Laranja impressionante
            if (KillStreak >= 20)
                return new Color(255, 255, 0); // Amarelo ótimo
            if (KillStreak >= 10)
                return new Color(100, 255, 100); // Verde bom
            
            return new Color(255, 255, 255); // Branco básico
        }
        
        /// <summary>
        /// Retorna texto do streak com título apropriado
        /// </summary>
        private string GetStreakText()
        {
            string title = KillStreak switch
            {
                >= 100 => "LEGENDARY",
                >= 50 => "GODLIKE",
                >= 30 => "UNSTOPPABLE",
                >= 20 => "DOMINATING",
                >= 10 => "KILLING SPREE",
                _ => "KILL STREAK"
            };
            
            return $"{title}! {KillStreak} KILLS";
        }
        
        /// <summary>
        /// Spawna partículas de celebração do kill streak
        /// </summary>
        private void SpawnStreakParticles(Vector2 position, Color color)
        {
            int particleCount = Math.Min(20, KillStreak / 3); // Mais partículas com streak maior
            
            for (int i = 0; i < particleCount; i++)
            {
                float angle = MathHelper.TwoPi * i / particleCount;
                Vector2 velocity = new Vector2(
                    (float)Math.Cos(angle) * 4f,
                    (float)Math.Sin(angle) * 4f - 2f // Bias para cima
                );
                
                int dustType = KillStreak >= 50 ? DustID.FireworkFountain_Red : DustID.FireworkFountain_Yellow;
                Dust dust = Dust.NewDustPerfect(position, dustType, velocity, 0, color, 1.8f);
                dust.noGravity = true;
                dust.fadeIn = 1.5f;
            }
        }
        
        /// <summary>
        /// Retorna o multiplicador de XP baseado no kill streak
        /// </summary>
        public float GetStreakXPMultiplier()
        {
            serverConfig ??= ModContent.GetInstance<Configs.ServerConfig>();
            
            if (!serverConfig.EnableKillStreakSystem || !serverConfig.EnableStreakXPBonus)
                return 1f;
            
            // +1% por kill de streak, até o máximo configurado
            float bonusPercent = KillStreak * serverConfig.StreakXPBonusPerKill;
            bonusPercent = Math.Min(bonusPercent, serverConfig.StreakXPBonusMax);
            
            return 1f + (bonusPercent / 100f);
        }
        
        /// <summary>
        /// Verifica se o streak está ativo e pulsando
        /// </summary>
        public bool IsPulsing()
        {
            return pulseTimer > 0 && KillStreak >= lastDisplayedStreak;
        }
        
        /// <summary>
        /// Retorna o fator de pulso atual (0-1)
        /// </summary>
        public float GetPulseFactor()
        {
            if (!IsPulsing())
                return 0f;
            
            // Pulso sinusoidal
            return (float)Math.Sin(pulseTimer * 0.15f) * 0.5f + 0.5f;
        }
        
        /// <summary>
        /// Retorna o tempo restante do streak normalizado (0-1)
        /// </summary>
        public float GetTimeRemainingNormalized()
        {
            if (KillStreak == 0)
                return 0f;
            
            return (float)streakTimer / STREAK_TIMEOUT;
        }
    }
}
