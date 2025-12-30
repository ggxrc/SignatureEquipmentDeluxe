using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using SignatureEquipmentDeluxe.Common.Data;
using SignatureEquipmentDeluxe.Common.Configs;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// Sistema que gerencia drops de armas com maldição ao morrer
    /// </summary>
    public class CurseDeathSystem : ModPlayer
    {
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            var config = ModContent.GetInstance<ServerConfig>();
            if (!config.EnableCurseSystem || !config.EnableLeveledEnemies)
                return;
            
            // Verifica se morreu por dano de queda - não dropa arma
            if (damageSource.SourceOtherIndex == 0 && damageSource.SourcePlayerIndex == Player.whoAmI)
            {
                // Dano de queda (SourceOtherIndex == 0 significa dano genérico, e SourcePlayerIndex == player significa auto-dano)
                return;
            }
            
            // Identifica o killer (se foi NPC)
            NPC killerNPC = null;
            if (damageSource.SourceNPCIndex >= 0 && damageSource.SourceNPCIndex < Main.maxNPCs)
            {
                killerNPC = Main.npc[damageSource.SourceNPCIndex];
            }
            
            // Verifica cada item equipado
            CheckAndDropCursedWeapon(Player.HeldItem, killerNPC);
            
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                if (i < 10) // Apenas hotbar
                {
                    CheckAndDropCursedWeapon(Player.inventory[i], killerNPC);
                }
            }
        }
        
        /// <summary>
        /// Verifica se deve dropar arma com maldição
        /// </summary>
        private void CheckAndDropCursedWeapon(Item item, NPC killerNPC)
        {
            if (item == null || item.IsAir)
                return;
            
            var sigItem = item.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
            if (sigItem.Level <= 0 || sigItem.EquippedRunes.Count == 0)
                return;
            
            // Verifica se tem pelo menos uma maldição
            bool hasCurse = false;
            foreach (var rune in sigItem.EquippedRunes)
            {
                if (rune.IsCurse())
                {
                    hasCurse = true;
                    break;
                }
            }
            
            if (!hasCurse)
                return;
            
            // SEMPRE DROPA (100% de chance, não configurável)
            DropWeaponAndTriggerExplosion(item, sigItem.Level, killerNPC, sigItem.EquippedRunes);
            
            // Remove o item do inventário
            item.TurnToAir();
            
            // Mensagem para o jogador
            Main.NewText($"Your cursed {item.Name} unleashed its power!", 
                new Color(255, 100, 100));
        }
        
        /// <summary>
        /// Dropa a arma com animação de explosão (SEM criar zona radioativa)
        /// </summary>
        private void DropWeaponAndTriggerExplosion(Item item, int weaponLevel, NPC killerNPC, System.Collections.Generic.List<Data.EquippedRune> runes)
        {
            // ANIMAÇÃO DE EXPLOSÃO (sem criar zona radioativa)
            if (Main.netMode != Terraria.ID.NetmodeID.MultiplayerClient)
            {
                int projectile = Projectile.NewProjectile(
                    Player.GetSource_Death(),
                    Player.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<Content.Projectiles.WeaponAscensionProjectile>(),
                    0,
                    0f,
                    Player.whoAmI,
                    0f, // ai[0] = flag de inicialização
                    weaponLevel, // ai[1] = nível da arma
                    item.type // ai[2] = tipo do item para desenhar sprite correto
                );
            }
            
            // Se foi morto por um NPC, ele herda poderes
            if (killerNPC != null && killerNPC.active)
            {
                PowerUpKillerNPC(killerNPC, weaponLevel, runes);
            }
            
            // Efeito visual dramático de explosão
            SpawnRadioactiveExplosion(Player.Center);
            
            // Remove o item do inventário APÓS animação
            item.TurnToAir();
        }
        
        
        /// <summary>
        /// Potencializa o NPC que matou o jogador
        /// </summary>
        private void PowerUpKillerNPC(NPC killer, int weaponLevel, System.Collections.Generic.List<Data.EquippedRune> runes)
        {
            var leveledNPC = killer.GetGlobalNPC<LeveledEnemyGlobalNPC>();
            
            // Dobro do nível da arma perdida
            int newLevel = weaponLevel * 2;
            leveledNPC.SetLevelDirectly(newLevel, killer);
            
            // Herda efeitos de runas elementais (efeitos visuais apenas)
            foreach (var rune in runes)
            {
                if (IsElementalRune(rune.Type))
                {
                    // Adiciona efeito visual baseado na runa
                    SpawnRuneInheritEffect(killer.Center, rune.Type);
                }
            }
            
            // Animação ÉPICA
            for (int i = 0; i < 60; i++)
            {
                float angle = MathHelper.TwoPi * i / 60f;
                Vector2 velocity = new Vector2(
                    (float)System.Math.Cos(angle) * 10f,
                    (float)System.Math.Sin(angle) * 10f
                );
                
                Dust dust = Dust.NewDustPerfect(
                    killer.Center,
                    Terraria.ID.DustID.GreenTorch,
                    velocity,
                    0,
                    new Color(255, 50, 50),
                    2.5f
                );
                dust.noGravity = true;
            }
            
            // Som épico
            Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.Roar with { 
                Volume = 1f,
                Pitch = -0.8f
            }, killer.Center);
            
            // Mensagem dramática
            string npcName = killer.TypeName;
            Main.NewText($"☠ {npcName} absorbed the power of your weapon! (Level {newLevel}) ☠", 
                new Color(255, 50, 50));
            
            // Combat text no killer
            CombatText.NewText(killer.Hitbox, new Color(255, 50, 50), $"POWER ABSORBED!", true, true);
        }
        
        
        /// <summary>
        /// Spawn efeito visual de herança de runa
        /// </summary>
        private void SpawnRuneInheritEffect(Vector2 position, Data.RuneType runeType)
        {
            Color runeColor = Data.RuneDefinitions.GetColor(runeType);
            
            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(4f, 4f);
                Dust dust = Dust.NewDustPerfect(
                    position,
                    Terraria.ID.DustID.CursedTorch,
                    velocity,
                    0,
                    runeColor,
                    1.5f
                );
                dust.noGravity = true;
            }
        }
        
        /// <summary>
        /// Verifica se runa é elemental
        /// </summary>
        private bool IsElementalRune(Data.RuneType type)
        {
            return type == Data.RuneType.Fire ||
                   type == Data.RuneType.Ice ||
                   type == Data.RuneType.Poison ||
                   type == Data.RuneType.Lightning;
        }
        
        /// <summary>
        /// Efeito visual de explosão
        /// </summary>
        private void SpawnRadioactiveExplosion(Vector2 position)
        {
            for (int i = 0; i < 50; i++)
            {
                float angle = MathHelper.TwoPi * i / 50f;
                Vector2 velocity = new Vector2(
                    (float)System.Math.Cos(angle) * Main.rand.NextFloat(3f, 8f),
                    (float)System.Math.Sin(angle) * Main.rand.NextFloat(3f, 8f)
                );
                
                Dust dust = Dust.NewDustPerfect(
                    position,
                    Terraria.ID.DustID.GreenTorch,
                    velocity,
                    0,
                    new Color(100, 255, 100),
                    Main.rand.NextFloat(1.5f, 2.5f)
                );
                dust.noGravity = true;
            }
            
            // Som de explosão (opcional, pode ajustar)
            Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.Item14, position);
        }
    }
    
    /// <summary>
    /// Sistema para remover maldições com risco
    /// </summary>
    public static class CurseRemovalSystem
    {
        /// <summary>
        /// Remove uma maldição com chance de perder níveis
        /// </summary>
        public static bool RemoveCurse(Item item, int runeIndex, out bool lostLevels, out int levelsLost)
        {
            lostLevels = false;
            levelsLost = 0;
            
            var config = ModContent.GetInstance<ServerConfig>();
            var sigItem = item.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
            
            if (runeIndex < 0 || runeIndex >= sigItem.EquippedRunes.Count)
                return false;
            
            var rune = sigItem.EquippedRunes[runeIndex];
            if (!rune.IsCurse())
                return false; // Não é maldição, não pode remover por esse método
            
            // Remove a maldição
            sigItem.EquippedRunes.RemoveAt(runeIndex);
            
            // Roll de chance de perder níveis
            if (Main.rand.NextFloat() < config.CurseRemovalLevelLossChance)
            {
                lostLevels = true;
                levelsLost = (int)(sigItem.Level * config.CurseRemovalLevelLossFraction);
                levelsLost = System.Math.Max(1, levelsLost); // Mínimo 1 nível
                
                sigItem.Level -= levelsLost;
                if (sigItem.Level < 1)
                    sigItem.Level = 1;
                
                // Ajusta XP
                sigItem.Experience = 0;
            }
            
            return true;
        }
    }
}
