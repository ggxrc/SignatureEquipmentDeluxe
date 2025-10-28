using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using SignatureEquipmentDeluxe.Common.GlobalItems;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// Sistema que gerencia dano e perda de XP ao ser atingido por inimigos nivelados
    /// </summary>
    public class LeveledEnemyDamagePlayer : ModPlayer
    {        
        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            var leveledNPC = npc.GetGlobalNPC<LeveledEnemyGlobalNPC>();
            if (leveledNPC.EnemyLevel <= 0)
                return;
            
            int damageReceived = hurtInfo.Damage;
            
            // Aplica perda de XP na arma atual
            ApplyWeaponXPLoss(damageReceived);
            
            // Aplica perda de XP em todas as armaduras
            ApplyArmorXPLoss(damageReceived, Player.armor[0]); // Helmet
            ApplyArmorXPLoss(damageReceived, Player.armor[1]); // Chestplate
            ApplyArmorXPLoss(damageReceived, Player.armor[2]); // Leggings
        }
        
        /// <summary>
        /// Aplica perda de XP na arma equipada (500% do dano)
        /// </summary>
        private void ApplyWeaponXPLoss(int damage)
        {
            Item weapon = Player.HeldItem;
            if (weapon == null || weapon.IsAir)
                return;
            
            var sigItem = weapon.GetGlobalItem<SignatureGlobalItem>();
            if (!sigItem.CanGainExperience(weapon) || sigItem.Level <= 0)
                return;
            
            // Perda de 500% do dano
            int xpLoss = damage * 5;
            
            ApplyXPLoss(sigItem, weapon, xpLoss, Player.Center);
        }
        
        /// <summary>
        /// Aplica perda de XP em peça de armadura (500% do dano)
        /// </summary>
        private void ApplyArmorXPLoss(int damage, Item armorPiece)
        {
            if (armorPiece == null || armorPiece.IsAir)
                return;
            
            var sigItem = armorPiece.GetGlobalItem<SignatureGlobalItem>();
            if (sigItem.Level <= 0)
                return;
            
            // Perda de 500% do dano
            int xpLoss = damage * 5;
            
            ApplyXPLoss(sigItem, armorPiece, xpLoss, Player.Center);
        }
        
        /// <summary>
        /// Aplica perda de XP com possibilidade de perder nível
        /// </summary>
        private void ApplyXPLoss(SignatureGlobalItem sigItem, Item item, int xpLoss, Vector2 position)
        {
            sigItem.Experience -= xpLoss;
            
            // Popup de XP perdido em VERMELHO
            if (Main.netMode != Terraria.ID.NetmodeID.Server)
            {
                CombatText.NewText(new Rectangle((int)position.X, (int)position.Y, 10, 10), 
                    Color.Red, $"-{xpLoss} XP", true, false);
            }
            
            // Se o XP ficou negativo, perde nível
            if (sigItem.Experience < 0)
            {
                if (sigItem.Level > 1)
                {
                    sigItem.Level--;
                    sigItem.Experience = 0; // Reseta XP ao perder nível
                    
                    // Notificação dramática de perda de nível
                    if (Main.netMode != Terraria.ID.NetmodeID.Server)
                    {
                        Main.NewText($"Your {item.Name} lost a level! Now level {sigItem.Level}", 
                            Color.OrangeRed);
                        
                        // Efeito visual
                        for (int i = 0; i < 20; i++)
                        {
                            Vector2 velocity = Main.rand.NextVector2CircularEdge(4f, 4f);
                            Dust dust = Dust.NewDustPerfect(
                                position,
                                Terraria.ID.DustID.Smoke,
                                velocity,
                                100,
                                Color.DarkRed,
                                1.5f
                            );
                            dust.noGravity = true;
                        }
                        
                        Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.NPCDeath6, position);
                    }
                }
                else
                {
                    // Nível 1 não pode perder mais, apenas zera XP
                    sigItem.Experience = 0;
                }
            }
        }
        
        /// <summary>
        /// Verifica se o jogador não tem mais níveis em nenhum equipamento
        /// NOTA: O bônus de +50% dano deveria ser aplicado aqui, mas não há hook adequado no tModLoader atual
        /// </summary>
        private bool CheckIfNoLevelsRemaining()
        {
            // Verifica arma
            if (Player.HeldItem != null && !Player.HeldItem.IsAir)
            {
                var weaponSig = Player.HeldItem.GetGlobalItem<SignatureGlobalItem>();
                if (weaponSig.Level > 1) // Nível 1 conta como "tem nível"
                    return false;
            }
            
            // Verifica armaduras
            for (int i = 0; i < 3; i++)
            {
                if (Player.armor[i] != null && !Player.armor[i].IsAir)
                {
                    var armorSig = Player.armor[i].GetGlobalItem<SignatureGlobalItem>();
                    if (armorSig.Level > 1)
                        return false;
                }
            }
            
            return true; // Não tem mais níveis para perder
        }
    }
}
