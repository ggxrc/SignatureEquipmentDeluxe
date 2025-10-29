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
            
            // Usa o dano ORIGINAL do inimigo (npc.damage), não o dano recebido após defesa
            int originalDamage = npc.damage;
            if (originalDamage <= 0)
                originalDamage = hurtInfo.Damage; // Fallback se não tiver damage definido
            
            // APENAS a arma perde XP (armadura não perde mais)
            ApplyWeaponXPLoss(originalDamage);
        }
        
        /// <summary>
        /// Quando o jogador morre, inimigo nivelado pode roubar nível inteiro de arma da hotbar
        /// </summary>
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            // Verifica se foi morto por um NPC nivelado
            if (damageSource.SourceNPCIndex >= 0)
            {
                NPC killer = Main.npc[damageSource.SourceNPCIndex];
                if (killer != null && killer.active)
                {
                    var leveledNPC = killer.GetGlobalNPC<LeveledEnemyGlobalNPC>();
                    if (leveledNPC != null && leveledNPC.EnemyLevel > 0)
                    {
                        // Procura armas na hotbar (slots 0-9) que têm nível
                        System.Collections.Generic.List<Item> weaponsWithLevel = new System.Collections.Generic.List<Item>();
                        
                        for (int i = 0; i < 10; i++)
                        {
                            Item item = Player.inventory[i];
                            if (item != null && !item.IsAir && item.damage > 0) // Apenas itens de dano
                            {
                                var sigItem = item.GetGlobalItem<SignatureGlobalItem>();
                                if (sigItem != null && sigItem.Level > 0)
                                {
                                    weaponsWithLevel.Add(item);
                                }
                            }
                        }
                        
                        // Se tem armas com nível, rouba uma aleatória
                        if (weaponsWithLevel.Count > 0)
                        {
                            Item targetWeapon = weaponsWithLevel[Main.rand.Next(weaponsWithLevel.Count)];
                            var targetSigItem = targetWeapon.GetGlobalItem<SignatureGlobalItem>();
                            
                            int stolenLevel = targetSigItem.Level;
                            
                            // Remove TODOS os níveis da arma
                            targetSigItem.Level = 0;
                            targetSigItem.Experience = 0;
                            
                            // Inimigo ganha os níveis roubados
                            leveledNPC.EnemyLevel += stolenLevel;
                            leveledNPC.ApplyLevelScaling(killer);
                            
                            // Mensagem épica de roubo
                            if (Main.netMode != Terraria.ID.NetmodeID.Server)
                            {
                                Main.NewText($"{killer.TypeName} stole {stolenLevel} levels from your {targetWeapon.Name}!", Color.Red);
                                Main.NewText($"{killer.TypeName} is now level {leveledNPC.EnemyLevel}!", Color.Orange);
                            }
                            
                            // Efeito visual ÉPICO
                            for (int j = 0; j < 80; j++)
                            {
                                Vector2 velocity = Main.rand.NextVector2CircularEdge(10f, 10f);
                                Dust dust = Dust.NewDustPerfect(
                                    Player.Center,
                                    Terraria.ID.DustID.GreenTorch,
                                    velocity,
                                    0,
                                    Color.Red,
                                    2.5f
                                );
                                dust.noGravity = true;
                            }
                            
                            // Partículas fluem do jogador para o killer
                            for (int j = 0; j < 40; j++)
                            {
                                Vector2 direction = Vector2.Normalize(killer.Center - Player.Center);
                                Vector2 velocity = direction * Main.rand.NextFloat(8f, 15f);
                                Dust flow = Dust.NewDustPerfect(
                                    Player.Center,
                                    Terraria.ID.DustID.Electric,
                                    velocity,
                                    0,
                                    Color.Yellow,
                                    2f
                                );
                                flow.noGravity = true;
                            }
                            
                            // Som dramático
                            Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.Roar, Player.Center);
                            
                            // Combat text no killer
                            CombatText.NewText(killer.Hitbox, Color.Lime, $"+{stolenLevel} LEVELS!", true, true);
                        }
                    }
                }
            }
            
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);
        }
        
        /// <summary>
        /// Aplica perda de XP na arma equipada (1625% do dano ORIGINAL do inimigo)
        /// </summary>
        private void ApplyWeaponXPLoss(int originalDamage)
        {
            Item weapon = Player.HeldItem;
            if (weapon == null || weapon.IsAir)
                return;
            
            var sigItem = weapon.GetGlobalItem<SignatureGlobalItem>();
            if (!sigItem.CanGainExperience(weapon) || sigItem.Level <= 0)
                return;
            
            // Perda de 1625% do dano ORIGINAL (225% de aumento: 500% -> 1625%)
            int xpLoss = (int)(originalDamage * 16.25f);
            
            ApplyXPLoss(sigItem, weapon, xpLoss, Player.Center);
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
