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
                // DEBUG: Mostrar que está na zona
                if (Main.rand.NextBool(10)) // Só mostra às vezes para não spam
                {
                    Main.NewText($"DEBUG: In radioactive zone! Level: {maxLevel}, Distance: {Vector2.Distance(Player.Center, zoneCenter):F0}/{zoneRadius:F0}", Color.Yellow);
                }
                
                float distanceFromCenter = Vector2.Distance(Player.Center, zoneCenter);
                float normalizedDistance = MathHelper.Clamp(distanceFromCenter / zoneRadius, 0f, 1f);
                
                // Aplicar debuffs baseados em anéis concêntricos
                ApplyRingBasedDebuffs(normalizedDistance, maxLevel);
            }
        }
        
        /// <summary>
        /// Aplica debuffs baseados em anéis concêntricos da zona
        /// Cada tier ativa debuffs em anéis específicos (dentro do anel)
        /// </summary>
        private void ApplyRingBasedDebuffs(float normalizedDistance, int zoneLevel)
        {
            // Sistema de anéis: 0.0 = centro, 1.0 = borda
            // Tier 1: Anel externo (0.6-1.0)
            // Tier 2: Dois anéis (0.3-1.0) 
            // Tier 3: Três anéis (0.0-1.0)
            // etc.
            
            // Tier 1: Anel mais externo - debuffs leves
            if (zoneLevel >= 1 && normalizedDistance >= 0.6f)
            {
                Player.AddBuff(BuffID.Weak, 5 * 60); // Fraqueza
            }
            
            // Tier 2: Dois anéis externos - adiciona ichor
            if (zoneLevel >= 2 && normalizedDistance >= 0.3f)
            {
                Player.AddBuff(BuffID.Ichor, 5 * 60);
                Player.AddBuff(BuffID.Weak, 5 * 60);
            }
            
            // Tier 3: Toda a zona - adiciona lentidão
            if (zoneLevel >= 3)
            {
                Player.AddBuff(BuffID.Ichor, 5 * 60);
                Player.AddBuff(BuffID.Weak, 5 * 60);
                Player.AddBuff(BuffID.Slow, 5 * 60);
                
                // Anel interno (0.0-0.3): Fogo amaldiçoado
                if (normalizedDistance < 0.3f)
                {
                    Player.AddBuff(BuffID.CursedInferno, 5 * 60);
                }
            }
            
            // Tier 4: Toda a zona + sangramento
            if (zoneLevel >= 4)
            {
                Player.AddBuff(BuffID.Ichor, 5 * 60);
                Player.AddBuff(BuffID.Weak, 5 * 60);
                Player.AddBuff(BuffID.Slow, 5 * 60);
                Player.AddBuff(BuffID.Bleeding, 5 * 60);
                
                // Anel interno (0.0-0.3): Fogo sombrio
                if (normalizedDistance < 0.3f)
                {
                    Player.AddBuff(BuffID.ShadowFlame, 5 * 60);
                }
            }
            
            // Tier 5: CAOS TOTAL
            if (zoneLevel >= 5)
            {
                Player.AddBuff(BuffID.Ichor, 5 * 60);
                Player.AddBuff(BuffID.Weak, 5 * 60);
                Player.AddBuff(BuffID.Slow, 5 * 60);
                Player.AddBuff(BuffID.Bleeding, 5 * 60);
                Player.AddBuff(BuffID.Darkness, 5 * 60);
                Player.lifeRegen -= 5;
                Player.moveSpeed *= 0.9f;
                
                // Anéis internos: múltiplos efeitos
                if (normalizedDistance < 0.2f) // Centro
                {
                    Player.AddBuff(BuffID.ShadowFlame, 5 * 60);
                    Player.AddBuff(BuffID.CursedInferno, 5 * 60);
                    // Dano contínuo no centro
                    Player.Hurt(Terraria.DataStructures.PlayerDeathReason.ByCustomReason(Terraria.Localization.NetworkText.FromLiteral("was consumed by radioactive chaos")), 5, 0, false, false, -1, false);
                }
                else if (normalizedDistance < 0.4f) // Anel médio
                {
                    Player.AddBuff(BuffID.ShadowFlame, 5 * 60);
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
