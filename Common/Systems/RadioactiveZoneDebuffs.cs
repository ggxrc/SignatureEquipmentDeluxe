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
        /// Aplica debuffs baseados na proximidade do centro
        /// </summary>
        private void ApplyCenterDebuffs(float intensity, int zoneLevel)
        {
            // ZONA 1: Borda (0% - 25% do raio) - Sem efeitos
            if (intensity < 0.25f)
                return;
            
            // ZONA 2: Média (25% - 50% do raio) - Debuffs leves
            if (intensity >= 0.25f && intensity < 0.50f)
            {
                // Poisoned por 5 segundos
                Player.AddBuff(BuffID.Poisoned, 5 * 60);
                
                // Visual: partículas verdes ocasionais
                if (Main.rand.NextBool(10))
                {
                    Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Poisoned);
                }
            }
            
            // ZONA 3: Próximo (50% - 75% do raio) - Debuffs médios
            else if (intensity >= 0.50f && intensity < 0.75f)
            {
                // Poisoned + Weak por 5 segundos
                Player.AddBuff(BuffID.Poisoned, 5 * 60);
                Player.AddBuff(BuffID.Weak, 5 * 60);
                
                // Redução de regeneração de vida
                Player.lifeRegen -= 2;
                
                // Visual: mais partículas
                if (Main.rand.NextBool(5))
                {
                    Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.CursedTorch);
                    dust.noGravity = true;
                }
            }
            
            // ZONA 4: Muito Próximo (75% - 90% do raio) - Debuffs pesados
            else if (intensity >= 0.75f && intensity < 0.90f)
            {
                // Poisoned + Weak + Slow por 5 segundos
                Player.AddBuff(BuffID.Poisoned, 5 * 60);
                Player.AddBuff(BuffID.Weak, 5 * 60);
                Player.AddBuff(BuffID.Slow, 5 * 60);
                
                // Redução severa de regeneração
                Player.lifeRegen -= 5;
                
                // Redução de velocidade de movimento
                Player.moveSpeed *= 0.85f;
                
                // Visual: muitas partículas
                if (Main.rand.NextBool(3))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.CursedTorch, 0f, 0f, 100, Color.LimeGreen, 1.5f);
                        dust.noGravity = true;
                    }
                }
                
                // Mensagem de aviso
                if (Main.rand.NextBool(600)) // ~1 vez a cada 10 segundos
                {
                    Main.NewText("You are dangerously close to the zone's epicenter!", Color.Orange);
                }
            }
            
            // ZONA 5: EPICENTRO (90% - 100% do raio) - APOCALIPSE
            else // intensity >= 0.90f
            {
                // TODOS os debuffs anteriores + extras
                Player.AddBuff(BuffID.Poisoned, 5 * 60);
                Player.AddBuff(BuffID.Weak, 5 * 60);
                Player.AddBuff(BuffID.Slow, 5 * 60);
                Player.AddBuff(BuffID.Bleeding, 5 * 60);
                Player.AddBuff(BuffID.Cursed, 5 * 60); // Não pode usar itens
                Player.AddBuff(BuffID.Darkness, 5 * 60); // Visão reduzida
                
                // Dano constante (baseado no nível da zona)
                int damagePerTick = 5 + (zoneLevel / 10); // 5 base + nível/10
                Player.Hurt(Terraria.DataStructures.PlayerDeathReason.ByCustomReason($"{Player.name} was consumed by radioactive energy"), damagePerTick, 0, false, false, -1, false);
                
                // Regeneração extremamente negativa
                Player.lifeRegen -= 10;
                
                // Movimento muito lento
                Player.moveSpeed *= 0.60f;
                
                // Visual: partículas MASSIVAS
                if (Main.rand.NextBool(2))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.CursedTorch, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 100, Color.Red, 2f);
                        dust.noGravity = true;
                    }
                }
                
                // Screen shake
                if (Main.rand.NextBool(30))
                {
                    Main.LocalPlayer.GetModPlayer<ScreenShakePlayer>()?.AddShake(5, 0.3f);
                }
                
                // Mensagem de PERIGO EXTREMO
                if (Main.rand.NextBool(300)) // ~1 vez a cada 5 segundos
                {
                    Main.NewText("☢ DANGER! YOU ARE IN THE EPICENTER! ☢", Color.Red);
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
