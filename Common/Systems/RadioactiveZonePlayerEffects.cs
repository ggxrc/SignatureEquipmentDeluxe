using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// Efeitos visuais e filtros para jogador em zona radioativa
    /// </summary>
    public class RadioactiveZonePlayerEffects : ModPlayer
    {
        private bool wasInZoneLastFrame = false;
        private float screenDistortionIntensity = 0f;
        private int transitionTimer = 0;
        private const int TRANSITION_DURATION = 60; // 1 segundo
        
        // Sistema de debuff de fraqueza
        private int zoneTimer = 0; // Tempo dentro da zona (em frames)
        private const int WEAKNESS_THRESHOLD = 3600; // 60 segundos (60 frames/s * 60s)
        
        public override void PostUpdate()
        {
            bool isInZoneNow = LeveledEnemySystem.IsInRadioactiveZone(Player.Center, out int maxLevel);
            
            // Rastreia tempo dentro da zona
            if (isInZoneNow)
            {
                zoneTimer++;
                
                // Aplica debuff de fraqueza após 1 minuto
                if (zoneTimer >= WEAKNESS_THRESHOLD)
                {
                    Player.AddBuff(Terraria.ID.BuffID.Weak, 120); // 2 segundos de buff (renovado constantemente)
                    
                    // Aviso visual a cada 10 segundos
                    if (zoneTimer == WEAKNESS_THRESHOLD || (zoneTimer - WEAKNESS_THRESHOLD) % 600 == 0)
                    {
                        if (Main.netMode != Terraria.ID.NetmodeID.Server)
                        {
                            CombatText.NewText(Player.Hitbox, Color.Purple, "Weakened by radiation!", true);
                        }
                    }
                }
            }
            else
            {
                // Reseta timer ao sair da zona
                zoneTimer = 0;
            }
            
            // Detecta entrada na zona
            if (isInZoneNow && !wasInZoneLastFrame)
            {
                OnEnterZone(maxLevel);
            }
            // Detecta saída da zona
            else if (!isInZoneNow && wasInZoneLastFrame)
            {
                OnExitZone();
            }
            
            // Atualiza transição
            if (transitionTimer > 0)
            {
                transitionTimer--;
                float progress = 1f - (transitionTimer / (float)TRANSITION_DURATION);
                
                if (isInZoneNow)
                {
                    // Entrando: 0 -> 1
                    screenDistortionIntensity = progress;
                }
                else
                {
                    // Saindo: 1 -> 0
                    screenDistortionIntensity = 1f - progress;
                }
            }
            else if (isInZoneNow)
            {
                // Mantém efeito máximo enquanto estiver na zona
                screenDistortionIntensity = 1f;
            }
            else
            {
                // Sem efeito fora da zona
                screenDistortionIntensity = 0f;
            }
            
            wasInZoneLastFrame = isInZoneNow;
        }
        
        /// <summary>
        /// Chamado quando o jogador entra em uma zona radioativa
        /// </summary>
        private void OnEnterZone(int zoneLevel)
        {
            transitionTimer = TRANSITION_DURATION;
            
            // Efeito visual DRAMÁTICO
            // Explosão de partículas ao redor do jogador
            for (int i = 0; i < 40; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(8f, 8f);
                Dust dust = Dust.NewDustPerfect(
                    Player.Center,
                    Terraria.ID.DustID.CursedTorch,
                    velocity,
                    0,
                    new Color(100, 255, 100),
                    2f
                );
                dust.noGravity = true;
            }
            
            // Onda de choque verde
            for (int i = 0; i < 30; i++)
            {
                float angle = MathHelper.TwoPi * i / 30f;
                Vector2 velocity = new Vector2(
                    (float)Math.Cos(angle) * 12f,
                    (float)Math.Sin(angle) * 12f
                );
                Dust shockwave = Dust.NewDustPerfect(
                    Player.Center,
                    Terraria.ID.DustID.Electric,
                    velocity,
                    0,
                    Color.Lime,
                    2.5f
                );
                shockwave.noGravity = true;
            }
            
            // Som sinistro
            Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.Roar with { 
                Volume = 0.7f,
                Pitch = -0.3f
            }, Player.Center);
            
            // Mensagem de aviso
            if (Main.netMode != Terraria.ID.NetmodeID.Server)
            {
                Main.NewText($"You entered a radioactive zone! (Max Level: {zoneLevel})", 
                    new Color(100, 255, 100));
                
                // Texto flutuante vermelho
                CombatText.NewText(Player.Hitbox, Color.Red, "DANGER!", true, true);
            }
            
            // Screen shake
            if (Main.LocalPlayer == Player)
            {
                Main.screenPosition += Main.rand.NextVector2Circular(10f, 10f);
            }
        }
        
        /// <summary>
        /// Chamado quando o jogador sai de uma zona radioativa
        /// </summary>
        private void OnExitZone()
        {
            transitionTimer = TRANSITION_DURATION;
            
            // Efeito visual suave de saída
            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4f, 4f);
                Dust dust = Dust.NewDustPerfect(
                    Player.Center,
                    Terraria.ID.DustID.Smoke,
                    velocity,
                    100,
                    Color.Gray,
                    1.5f
                );
                dust.noGravity = true;
            }
            
            // Som de alívio
            Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.Item4, Player.Center);
            
            // Mensagem
            if (Main.netMode != Terraria.ID.NetmodeID.Server)
            {
                Main.NewText("You left the radioactive zone.", Color.LightGreen);
            }
        }
        
        /// <summary>
        /// Desenha overlay verde na tela com pulsação suave
        /// </summary>
        public override void PostUpdateMiscEffects()
        {
            if (screenDistortionIntensity > 0f)
            {
                // Pulsação suave de brilho (60 frames = 1 segundo)
                float pulse = (float)Math.Sin(Main.GameUpdateCount * 0.05f) * 0.15f + 0.85f;
                
                // Aplica lighting verde pulsante ao jogador
                Lighting.AddLight(Player.Center, 
                    0.1f * screenDistortionIntensity * pulse, 
                    0.5f * screenDistortionIntensity * pulse, 
                    0.1f * screenDistortionIntensity * pulse);
            }
        }
    }
}
