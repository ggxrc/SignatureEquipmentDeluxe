using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// Sistema de combo que rastreia hits consecutivos e oferece bônus
    /// </summary>
    public class ComboSystem : ModPlayer
    {
        // Estado do combo
        public int ComboCount { get; private set; } = 0;
        private int comboTimer = 0;
        private const int COMBO_TIMEOUT = 180; // 3 segundos sem hit para resetar
        private const int COMBO_VISUAL_TIMER = 60; // 1 segundo para mostrar texto
        
        // Cache de configs
        private Configs.ClientConfig clientConfig;
        private Configs.ServerConfig serverConfig;
        
        public override void Initialize()
        {
            ComboCount = 0;
            comboTimer = 0;
        }
        
        public override void PostUpdate()
        {
            // Atualiza timer do combo
            if (comboTimer > 0)
            {
                comboTimer--;
                
                // Resetar combo se timeout
                if (comboTimer <= 0)
                {
                    ResetCombo();
                }
            }
        }
        
        /// <summary>
        /// Adiciona um hit ao combo
        /// </summary>
        public void AddComboHit(Vector2 hitPosition)
        {
            serverConfig ??= ModContent.GetInstance<Configs.ServerConfig>();
            clientConfig ??= ModContent.GetInstance<Configs.ClientConfig>();
            
            if (!serverConfig.EnableComboSystem)
                return;
            
            ComboCount++;
            comboTimer = COMBO_TIMEOUT;
            
            // Mostra visual do combo se atingir milestone
            if (ShouldShowComboVisual())
            {
                ShowComboVisual(hitPosition);
            }
        }
        
        /// <summary>
        /// Reseta o combo
        /// </summary>
        private void ResetCombo()
        {
            if (ComboCount > 0)
            {
                ComboCount = 0;
            }
        }
        
        /// <summary>
        /// Verifica se deve mostrar visual do combo
        /// </summary>
        private bool ShouldShowComboVisual()
        {
            clientConfig ??= ModContent.GetInstance<Configs.ClientConfig>();
            
            // Mostra a cada X hits baseado na config
            return ComboCount % clientConfig.ComboVisualInterval == 0 && ComboCount >= clientConfig.ComboVisualInterval;
        }
        
        /// <summary>
        /// Mostra texto visual do combo
        /// </summary>
        private void ShowComboVisual(Vector2 position)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            
            clientConfig ??= ModContent.GetInstance<Configs.ClientConfig>();
            
            // Determina cor baseada no nível do combo
            Color comboColor = GetComboColor();
            
            // Mostra texto de combo
            string comboText = $"{ComboCount} HIT COMBO!";
            CombatText.NewText(new Rectangle((int)position.X, (int)position.Y, 10, 10), comboColor, comboText, dramatic: true);
            
            // Som de combo
            if (clientConfig.EnableComboSounds)
            {
                float pitch = Math.Min(0.5f, ComboCount * 0.02f); // Pitch aumenta com combo
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item4 with { Pitch = pitch }, position);
            }
            
            // Partículas de celebração
            if (clientConfig.EnableComboParticles)
            {
                SpawnComboParticles(position, comboColor);
            }
        }
        
        /// <summary>
        /// Retorna cor do combo baseado no número de hits
        /// </summary>
        private Color GetComboColor()
        {
            if (ComboCount >= 50)
                return new Color(255, 50, 255); // Magenta épico
            if (ComboCount >= 30)
                return new Color(255, 100, 50); // Laranja intenso
            if (ComboCount >= 20)
                return new Color(255, 200, 50); // Amarelo forte
            if (ComboCount >= 10)
                return new Color(100, 255, 255); // Ciano
            
            return new Color(255, 255, 255); // Branco básico
        }
        
        /// <summary>
        /// Spawna partículas de celebração do combo
        /// </summary>
        private void SpawnComboParticles(Vector2 position, Color color)
        {
            int particleCount = Math.Min(15, ComboCount / 2); // Mais partículas com combo maior
            
            for (int i = 0; i < particleCount; i++)
            {
                float angle = MathHelper.TwoPi * i / particleCount;
                Vector2 velocity = new Vector2(
                    (float)Math.Cos(angle) * 3f,
                    (float)Math.Sin(angle) * 3f
                );
                
                Dust dust = Dust.NewDustPerfect(position, DustID.FireworkFountain_Yellow, velocity, 0, color, 1.5f);
                dust.noGravity = true;
                dust.fadeIn = 1.2f;
            }
        }
        
        /// <summary>
        /// Retorna o multiplicador de XP baseado no combo
        /// </summary>
        public float GetComboXPMultiplier()
        {
            serverConfig ??= ModContent.GetInstance<Configs.ServerConfig>();
            
            if (!serverConfig.EnableComboSystem || !serverConfig.EnableComboXPBonus)
                return 1f;
            
            // +2% por hit de combo, até o máximo configurado
            float bonusPercent = ComboCount * serverConfig.ComboXPBonusPerHit;
            bonusPercent = Math.Min(bonusPercent, serverConfig.ComboXPBonusMax);
            
            return 1f + (bonusPercent / 100f);
        }
        
        /// <summary>
        /// Retorna o multiplicador de dano baseado no combo
        /// </summary>
        public float GetComboDamageMultiplier()
        {
            serverConfig ??= ModContent.GetInstance<Configs.ServerConfig>();
            
            if (!serverConfig.EnableComboSystem || !serverConfig.EnableComboDamageBonus)
                return 1f;
            
            // +1% por hit de combo, até o máximo configurado
            float bonusPercent = ComboCount * serverConfig.ComboDamageBonusPerHit;
            bonusPercent = Math.Min(bonusPercent, serverConfig.ComboDamageBonusMax);
            
            return 1f + (bonusPercent / 100f);
        }
    }
}
