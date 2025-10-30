using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// Sistema que aplica debuffs punitivos no centro das zonas radioativas
    /// Quanto mais perto do centro, mais severos os efeitos
    /// </summary>
    public class RadioactiveZoneDebuffs : ModPlayer
    {
        private int debuffCheckTimer = 0;
        
        public override void PostUpdate()
        {
            debuffCheckTimer++;
            
            if (debuffCheckTimer < 30) return; // Verifica a cada 0.5s
            debuffCheckTimer = 0;
            
            // Verifica se está em zona radioativa
            if (LeveledEnemySystem.IsInRadioactiveZone(Player.Center, out int maxLevel, out Vector2 zoneCenter, out float zoneRadius))
            {
                float distanceFromCenter = Vector2.Distance(Player.Center, zoneCenter);
                float normalizedDistance = MathHelper.Clamp(distanceFromCenter / zoneRadius, 0f, 1f);
                
                // Inverso: 1.0 no centro, 0.0 na borda
                float centerIntensity = 1f - normalizedDistance;
                
                ApplyCenterDebuffs(centerIntensity, maxLevel);
            }
        }
        
        /// <summary>
        /// Aplica debuffs baseados no tier da zona e proximidade do centro
        /// </summary>
        private void ApplyCenterDebuffs(float intensity, int zoneLevel)
        {
            // Debuffs baseados no tier da zona
            switch (zoneLevel)
            {
                case 1: // Tier 1: debuffs fraquíssimos, calmos
                    if (intensity >= 0.5f) // Apenas no centro
                    {
                        Player.AddBuff(BuffID.Weak, 5 * 60); // Fraquíssimo
                    }
                    break;
                    
                case 2: // Tier 2: adiciona ichor
                    if (intensity >= 0.4f)
                    {
                        Player.AddBuff(BuffID.Ichor, 5 * 60);
                        Player.AddBuff(BuffID.Weak, 5 * 60);
                    }
                    break;
                    
                case 3: // Tier 3: adiciona fogo amaldiçoado no epicentro
                    if (intensity >= 0.3f)
                    {
                        Player.AddBuff(BuffID.Ichor, 5 * 60);
                        Player.AddBuff(BuffID.Weak, 5 * 60);
                        Player.AddBuff(BuffID.Slow, 5 * 60);
                    }
                    if (intensity >= 0.9f) // Epicentro
                    {
                        Player.AddBuff(BuffID.CursedInferno, 5 * 60);
                    }
                    break;
                    
                case 4: // Tier 4: adiciona fogo sombrio antes do epicentro
                    if (intensity >= 0.3f)
                    {
                        Player.AddBuff(BuffID.Ichor, 5 * 60);
                        Player.AddBuff(BuffID.Weak, 5 * 60);
                        Player.AddBuff(BuffID.Slow, 5 * 60);
                        Player.AddBuff(BuffID.Bleeding, 5 * 60);
                    }
                    if (intensity >= 0.7f) // Antes do epicentro
                    {
                        Player.AddBuff(BuffID.ShadowFlame, 5 * 60);
                    }
                    break;
                    
                case 5: // Tier 5: caos e morte, dois anéis de fogo antes do epicentro
                    if (intensity >= 0.2f)
                    {
                        Player.AddBuff(BuffID.Ichor, 5 * 60);
                        Player.AddBuff(BuffID.Weak, 5 * 60);
                        Player.AddBuff(BuffID.Slow, 5 * 60);
                        Player.AddBuff(BuffID.Bleeding, 5 * 60);
                        Player.AddBuff(BuffID.Darkness, 5 * 60);
                        Player.lifeRegen -= 5;
                        Player.moveSpeed *= 0.9f;
                    }
                    if (intensity >= 0.6f) // Dois anéis
                    {
                        Player.AddBuff(BuffID.ShadowFlame, 5 * 60);
                        Player.AddBuff(BuffID.CursedInferno, 5 * 60);
                    }
                    if (intensity >= 0.9f) // Epicentro
                    {
                        Player.Hurt(Terraria.DataStructures.PlayerDeathReason.ByCustomReason(Terraria.Localization.NetworkText.FromLiteral("was consumed by radioactive chaos")), 10, 0, false, false, -1, false);
                    }
                    break;
            }
            
            // Efeitos visuais baseados na intensidade
            if (intensity >= 0.5f)
            {
                if (Main.rand.NextBool(10))
                {
                    Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.CursedTorch);
                    dust.noGravity = true;
                }
            }
        }
    }
    
    /// <summary>
    /// Sistema simples de screen shake
    /// </summary>
    public class ScreenShakePlayer : ModPlayer
    {
        private float shakeIntensity = 0f;
        private int shakeDuration = 0;
        
        public void AddShake(int duration, float intensity)
        {
            shakeDuration = duration;
            shakeIntensity = intensity;
        }
        
        public override void ModifyScreenPosition()
        {
            if (shakeDuration > 0)
            {
                Main.screenPosition += new Vector2(Main.rand.NextFloat(-shakeIntensity, shakeIntensity), Main.rand.NextFloat(-shakeIntensity, shakeIntensity)) * 16f;
                shakeDuration--;
            }
        }
    }
}
