using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SignatureEquipmentDeluxe.Common.Data;
using SignatureEquipmentDeluxe.Common.Configs;
using SignatureEquipmentDeluxe.Common.GlobalItems;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// Sistema de efeitos elementais das runas (trails e DoT)
    /// </summary>
    public static class RuneElementalEffects
    {
        /// <summary>
        /// Aplica efeitos visuais de trail durante ataque melee
        /// </summary>
        public static void ApplyMeleeTrailEffects(Player player, Item item, Rectangle hitbox)
        {
            var config = ModContent.GetInstance<ServerConfig>();
            if (!config.EnableElementalTrailEffects)
                return;
            
            var sigItem = item.GetGlobalItem<SignatureGlobalItem>();
            if (sigItem.EquippedRunes.Count == 0)
                return;
            
            // Para cada runa elemental, spawna partículas
            foreach (var rune in sigItem.EquippedRunes)
            {
                switch (rune.Type)
                {
                    case RuneType.Fire:
                        SpawnFireTrail(hitbox);
                        break;
                    case RuneType.Ice:
                        SpawnIceTrail(hitbox);
                        break;
                    case RuneType.Poison:
                        SpawnPoisonTrail(hitbox);
                        break;
                    case RuneType.Lightning:
                        SpawnLightningTrail(hitbox);
                        break;
                }
            }
        }
        
        /// <summary>
        /// Aplica efeitos DoT ao inimigo atingido
        /// </summary>
        public static void ApplyElementalDoT(NPC target, Player player, Item weapon, int damage)
        {
            var config = ModContent.GetInstance<ServerConfig>();
            if (!config.EnableElementalDoTEffects)
                return;
            
            var sigItem = weapon.GetGlobalItem<SignatureGlobalItem>();
            if (sigItem.EquippedRunes.Count == 0)
                return;
            
            // Para cada runa elemental, aplica debuff correspondente
            foreach (var rune in sigItem.EquippedRunes)
            {
                switch (rune.Type)
                {
                    case RuneType.Fire:
                        ApplyFireDoT(target, rune, config, damage);
                        break;
                    case RuneType.Ice:
                        ApplyIceDoT(target, rune, config, damage);
                        break;
                    case RuneType.Poison:
                        ApplyPoisonDoT(target, rune, config, damage);
                        break;
                    case RuneType.Lightning:
                        ApplyLightningDoT(target, rune, config, damage);
                        break;
                }
            }
        }
        
        // ==================== TRAIL EFFECTS ====================
        
        private static void SpawnFireTrail(Rectangle hitbox)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 position = new Vector2(
                    hitbox.X + Main.rand.Next(hitbox.Width),
                    hitbox.Y + Main.rand.Next(hitbox.Height)
                );
                
                Dust dust = Dust.NewDustDirect(position, 0, 0, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                dust.noGravity = true;
                dust.velocity *= 0.5f;
            }
        }
        
        private static void SpawnIceTrail(Rectangle hitbox)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 position = new Vector2(
                    hitbox.X + Main.rand.Next(hitbox.Width),
                    hitbox.Y + Main.rand.Next(hitbox.Height)
                );
                
                Dust dust = Dust.NewDustDirect(position, 0, 0, DustID.Ice, 0f, 0f, 100, default, 1.5f);
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }
        }
        
        private static void SpawnPoisonTrail(Rectangle hitbox)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 position = new Vector2(
                    hitbox.X + Main.rand.Next(hitbox.Width),
                    hitbox.Y + Main.rand.Next(hitbox.Height)
                );
                
                Dust dust = Dust.NewDustDirect(position, 0, 0, DustID.Poisoned, 0f, 0f, 100, default, 1.2f);
                dust.noGravity = true;
                dust.velocity *= 0.4f;
            }
        }
        
        private static void SpawnLightningTrail(Rectangle hitbox)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 position = new Vector2(
                    hitbox.X + Main.rand.Next(hitbox.Width),
                    hitbox.Y + Main.rand.Next(hitbox.Height)
                );
                
                Dust dust = Dust.NewDustDirect(position, 0, 0, DustID.Electric, 0f, 0f, 100, default, 1.3f);
                dust.noGravity = true;
                dust.velocity *= 0.6f;
            }
        }
        
        // ==================== DOT EFFECTS ====================
        
        private static void ApplyFireDoT(NPC target, EquippedRune rune, ServerConfig config, int baseDamage)
        {
            int duration = config.FireDoTDuration;
            float damagePerLevel = config.FireDoTDamagePerLevel;
            int totalDamage = (int)(rune.Level * damagePerLevel);
            
            target.AddBuff(BuffID.OnFire, duration);
            
            // Spawn visual feedback
            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustDirect(target.position, target.width, target.height, DustID.Torch, 0f, 0f, 100, default, 1.2f);
                dust.noGravity = true;
            }
        }
        
        private static void ApplyIceDoT(NPC target, EquippedRune rune, ServerConfig config, int baseDamage)
        {
            int duration = config.IceDoTDuration;
            float damagePerLevel = config.IceDoTDamagePerLevel;
            
            target.AddBuff(BuffID.Frostburn, duration);
            
            // Spawn visual feedback
            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustDirect(target.position, target.width, target.height, DustID.Ice, 0f, 0f, 100, default, 1.2f);
                dust.noGravity = true;
            }
        }
        
        private static void ApplyPoisonDoT(NPC target, EquippedRune rune, ServerConfig config, int baseDamage)
        {
            int duration = config.PoisonDoTDuration;
            float damagePerLevel = config.PoisonDoTDamagePerLevel;
            
            target.AddBuff(BuffID.Poisoned, duration);
            
            // Spawn visual feedback
            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustDirect(target.position, target.width, target.height, DustID.Poisoned, 0f, 0f, 100, default, 1.0f);
                dust.noGravity = true;
            }
        }
        
        private static void ApplyLightningDoT(NPC target, EquippedRune rune, ServerConfig config, int baseDamage)
        {
            int duration = config.LightningDoTDuration;
            float damagePerLevel = config.LightningDoTDamagePerLevel;
            
            target.AddBuff(BuffID.Electrified, duration);
            
            // Spawn visual feedback
            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustDirect(target.position, target.width, target.height, DustID.Electric, 0f, 0f, 100, default, 1.3f);
                dust.noGravity = true;
            }
        }
    }
}
