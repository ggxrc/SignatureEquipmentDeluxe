using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace SignatureEquipmentDeluxe.Common.UI
{
    /// <summary>
    /// HUD fixo na tela mostrando o kill streak atual
    /// </summary>
    public class KillStreakHUD : ModSystem
    {
        private static float fadeAlpha = 0f;
        private static int lastKnownStreak = 0;
        private static int fadeTimer = 0;
        private const int FADE_DURATION = 180; // 3 segundos para desaparecer após última kill
        
        public override void PostUpdateEverything()
        {
            if (Main.netMode == Terraria.ID.NetmodeID.Server)
                return;
            
            var player = Main.LocalPlayer;
            if (player == null || !player.active)
                return;
            
            var killStreakSystem = player.GetModPlayer<Systems.KillStreakSystem>();
            int currentStreak = killStreakSystem.KillStreak;
            
            // Se o streak mudou, reseta o fade
            if (currentStreak != lastKnownStreak)
            {
                lastKnownStreak = currentStreak;
                if (currentStreak > 0)
                {
                    fadeTimer = FADE_DURATION;
                }
            }
            
            // Atualiza fade
            if (fadeTimer > 0)
            {
                fadeTimer--;
                fadeAlpha = MathHelper.Lerp(fadeAlpha, 1f, 0.15f);
            }
            else if (currentStreak == 0)
            {
                fadeAlpha = MathHelper.Lerp(fadeAlpha, 0f, 0.1f);
            }
        }
        
        public override void ModifyInterfaceLayers(System.Collections.Generic.List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "SignatureEquipmentDeluxe: Kill Streak HUD",
                    delegate
                    {
                        DrawKillStreakHUD();
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
        
        private void DrawKillStreakHUD()
        {
            if (Main.netMode == Terraria.ID.NetmodeID.Server)
                return;
            
            var clientConfig = ModContent.GetInstance<Configs.ClientConfig>();
            if (!clientConfig.ShowKillStreakHUD)
                return;
            
            var player = Main.LocalPlayer;
            if (player == null || !player.active)
                return;
            
            var killStreakSystem = player.GetModPlayer<Systems.KillStreakSystem>();
            int streak = killStreakSystem.KillStreak;
            
            // Não desenha se não há streak ou está completamente transparente
            if (streak == 0 && fadeAlpha < 0.01f)
                return;
            
            // Posição na tela (canto superior esquerdo, longe do mapa)
            Vector2 basePosition = new Vector2(20, 80);
            
            // Ajusta posição baseado na config
            basePosition += clientConfig.KillStreakHUDOffset;
            
            SpriteBatch spriteBatch = Main.spriteBatch;
            
            // Pega cor do streak
            Color streakColor = GetStreakColor(streak);
            Color textColor = streakColor * fadeAlpha;
            
            // Efeito de pulso
            float pulseScale = 1f;
            if (killStreakSystem.IsPulsing())
            {
                float pulseFactor = killStreakSystem.GetPulseFactor();
                pulseScale = 1f + (pulseFactor * 0.15f); // Pulso de até 15%
            }
            
            // Desenha título do streak
            string title = GetStreakTitle(streak);
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            Vector2 titleSize = font.MeasureString(title);
            Vector2 titlePosition = basePosition;
            
            // Sombra do título
            Utils.DrawBorderString(
                spriteBatch,
                title,
                titlePosition,
                textColor,
                pulseScale * 0.8f
            );
            
            // Desenha contador de kills
            string killText = $"{streak} KILLS";
            DynamicSpriteFont deathFont = FontAssets.DeathText.Value;
            Vector2 killSize = deathFont.MeasureString(killText);
            Vector2 killPosition = basePosition + new Vector2(0, titleSize.Y * 0.8f * pulseScale + 5);
            
            // Desenha texto de kills com borda
            Utils.DrawBorderString(
                spriteBatch,
                killText,
                killPosition,
                textColor * 1.2f, // Mais brilhante
                pulseScale * 1.2f
            );
            
            // Desenha barra de tempo restante
            if (clientConfig.ShowKillStreakTimer)
            {
                float timeRemaining = killStreakSystem.GetTimeRemainingNormalized();
                DrawStreakTimer(basePosition + new Vector2(0, (titleSize.Y * 0.8f + killSize.Y * 1.2f) * pulseScale + 15), timeRemaining, streakColor * fadeAlpha);
            }
        }
        
        private void DrawStreakTimer(Vector2 position, float progress, Color color)
        {
            const int barWidth = 200;
            const int barHeight = 8;
            
            Rectangle bgRect = new Rectangle((int)position.X, (int)position.Y, barWidth, barHeight);
            Rectangle fillRect = new Rectangle((int)position.X, (int)position.Y, (int)(barWidth * progress), barHeight);
            
            // Fundo da barra
            Main.spriteBatch.Draw(
                TextureAssets.MagicPixel.Value,
                bgRect,
                Color.Black * 0.8f
            );
            
            // Preenchimento da barra
            Main.spriteBatch.Draw(
                TextureAssets.MagicPixel.Value,
                fillRect,
                color
            );
            
            // Borda da barra
            DrawRectangleBorder(bgRect, Color.White * (color.A / 255f), 1);
        }
        
        private void DrawRectangleBorder(Rectangle rect, Color color, int borderWidth)
        {
            // Top
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, rect.Width, borderWidth), color);
            // Bottom
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y + rect.Height - borderWidth, rect.Width, borderWidth), color);
            // Left
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, borderWidth, rect.Height), color);
            // Right
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X + rect.Width - borderWidth, rect.Y, borderWidth, rect.Height), color);
        }
        
        private Color GetStreakColor(int streak)
        {
            if (streak >= 100)
                return new Color(255, 0, 255); // Magenta lendário
            if (streak >= 50)
                return new Color(255, 50, 50); // Vermelho épico
            if (streak >= 30)
                return new Color(255, 150, 0); // Laranja impressionante
            if (streak >= 20)
                return new Color(255, 255, 0); // Amarelo ótimo
            if (streak >= 10)
                return new Color(100, 255, 100); // Verde bom
            
            return new Color(255, 255, 255); // Branco básico
        }
        
        private string GetStreakTitle(int streak)
        {
            return streak switch
            {
                >= 100 => "LEGENDARY",
                >= 50 => "GODLIKE",
                >= 30 => "UNSTOPPABLE",
                >= 20 => "DOMINATING",
                >= 10 => "KILLING SPREE",
                >= 1 => "KILL STREAK",
                _ => ""
            };
        }
    }
}
