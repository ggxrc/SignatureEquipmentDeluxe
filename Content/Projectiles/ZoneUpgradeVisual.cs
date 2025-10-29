using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SignatureEquipmentDeluxe.Content.Projectiles
{
    /// <summary>
    /// Visual temporário quando uma zona é upgradada
    /// Mostra a arma levitando no centro por 5 segundos com efeitos dramáticos
    /// </summary>
    public class ZoneUpgradeVisual : ModProjectile
    {
        private Texture2D weaponTexture;
        private int itemType;
        
        public override string Texture => "Terraria/Images/Item_0";
        
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300; // 5 segundos
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
        }
        
        public override void AI()
        {
            // Inicialização
            if (Projectile.ai[0] == 0f)
            {
                itemType = (int)Projectile.ai[1];
                
                // Carrega a textura da arma
                Item tempItem = new Item();
                tempItem.SetDefaults(itemType);
                if (tempItem.type != ItemID.None)
                {
                    weaponTexture = Terraria.GameContent.TextureAssets.Item[itemType].Value;
                }
                
                Projectile.ai[0] = 1f;
            }
            
            // Gira rapidamente
            Projectile.rotation += 0.2f;
            
            // Pulsa verticalmente
            float pulse = (float)System.Math.Sin(Projectile.timeLeft * 0.1f) * 5f;
            Projectile.Center = new Vector2(Projectile.Center.X, Projectile.Center.Y + pulse);
            
            // Partículas orbitando intensamente
            if (Main.rand.NextBool(2))
            {
                float angle = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                float distance = 30f + (float)System.Math.Sin(Projectile.timeLeft * 0.1f) * 10f;
                Vector2 pos = Projectile.Center + new Vector2(
                    (float)System.Math.Cos(angle) * distance,
                    (float)System.Math.Sin(angle) * distance
                );
                
                Dust orbit = Dust.NewDustPerfect(pos, DustID.Electric, Vector2.Zero, 0, Color.OrangeRed, 2f);
                orbit.noGravity = true;
            }
            
            // Luz pulsante intensa
            float lightIntensity = 0.8f + (float)System.Math.Sin(Projectile.timeLeft * 0.15f) * 0.2f;
            Lighting.AddLight(Projectile.Center, 1f * lightIntensity, 0.5f * lightIntensity, 0f);
        }
        
        public override bool PreDraw(ref Color lightColor)
        {
            if (weaponTexture == null)
                return false;
            
            Rectangle sourceRect = weaponTexture.Frame(1, 1);
            Vector2 origin = sourceRect.Size() / 2f;
            
            // Brilho intenso vermelho/laranja
            Color drawColor = Color.Lerp(Color.White, Color.OrangeRed, 0.5f);
            
            Main.EntitySpriteDraw(
                weaponTexture,
                Projectile.Center - Main.screenPosition,
                sourceRect,
                drawColor * (1f - Projectile.alpha / 255f),
                Projectile.rotation,
                origin,
                Projectile.scale * 1.2f, // Ligeiramente maior
                SpriteEffects.None,
                0
            );
            
            return false;
        }
    }
}
