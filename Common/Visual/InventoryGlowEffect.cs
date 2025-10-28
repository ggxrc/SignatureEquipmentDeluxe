using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;

namespace SignatureEquipmentDeluxe.Common.Visual
{
    /// <summary>
    /// Sistema de glow (brilho) pulsante para itens de alto nível no inventário
    /// </summary>
    public class InventoryGlowEffect
    {
        /// <summary>
        /// Desenha efeito de glow pulsante sobre o item no inventário
        /// </summary>
        public static void DrawGlow(Item item, SpriteBatch spriteBatch, Vector2 position, 
            Rectangle frame, Vector2 origin, float scale, int level, Color glowColor)
        {
            Texture2D texture = TextureAssets.Item[item.type].Value;
            
            // Calcula pulsação baseada no tempo (breathing effect) - mais rápido e agressivo
            float pulse = (float)Math.Sin(Main.GameUpdateCount * 0.08f) * 0.5f + 0.5f;
            
            // Intensidade baseada no nível
            float intensity = GetGlowIntensity(level);
            
            // Aplica pulsação e intensidade - aumentado de 0.6f para 0.9f para ser mais agressivo
            float finalOpacity = pulse * intensity * 0.9f;
            
            Color finalGlowColor = glowColor * finalOpacity;
            
            // Desenha com blend additivo para efeito de brilho
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, 
                DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            
            // Desenha múltiplas camadas para glow mais suave - mais camadas para efeito mais forte
            for (int i = 0; i < 4; i++)
            {
                float layerScale = scale * (1f + i * 0.08f);
                float layerOpacity = finalOpacity * (1f - i * 0.25f);
                Color layerColor = glowColor * layerOpacity;
                
                spriteBatch.Draw(
                    texture,
                    position,
                    frame,
                    layerColor,
                    0f,
                    origin,
                    layerScale,
                    SpriteEffects.None,
                    0f
                );
            }
            
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, 
                DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        }
        
        /// <summary>
        /// Retorna a intensidade do glow baseado no nível
        /// </summary>
        private static float GetGlowIntensity(int level)
        {
            if (level >= 150)
                return 1.0f;  // Glow máximo
            if (level >= 100)
                return 0.8f;
            if (level >= 75)
                return 0.6f;
            if (level >= 50)
                return 0.4f;
            
            return 0.2f;  // Glow mínimo
        }
    }
}
