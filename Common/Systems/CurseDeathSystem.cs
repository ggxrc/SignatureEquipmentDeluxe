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
            
            float dropChance = RuneSystem.GetCurseDropChance(sigItem.EquippedRunes);
            if (dropChance <= 0)
                return;
            
            // Roll de chance (FORÇADO 100% PARA TESTE)
            dropChance = 1.0f; // TESTE: Sempre dropa
            if (Main.rand.NextFloat() < dropChance)
            {
                DropWeaponAndCreateRadioactiveZone(item, sigItem.Level, killerNPC, sigItem.EquippedRunes);
                
                // Remove o item do inventário
                item.TurnToAir();
                
                // Mensagem para o jogador
                Main.NewText($"Your cursed {item.Name} was dropped and created a radioactive zone!", 
                    new Color(255, 100, 100));
            }
        }
        
        /// <summary>
        /// Dropa a arma e cria zona radioativa
        /// </summary>
        private void DropWeaponAndCreateRadioactiveZone(Item item, int weaponLevel, NPC killerNPC, System.Collections.Generic.List<Data.EquippedRune> runes)
        {
            // Salva os dados antes de dropar
            var originalSig = item.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
            int savedLevel = originalSig.Level;
            int savedExp = originalSig.Experience;
            var savedRunes = new System.Collections.Generic.List<Data.EquippedRune>(originalSig.EquippedRunes);
            
            // Dropa o item no chão usando o método seguro do jogador
            Player.QuickSpawnItem(Player.GetSource_Death(), item.type, 1);
            
            // Cria zona radioativa
            LeveledEnemySystem.AddRadioactiveZone(Player.Center, weaponLevel);
            
            // Se foi morto por um NPC, ele herda poderes
            if (killerNPC != null && killerNPC.active)
            {
                PowerUpKillerNPC(killerNPC, weaponLevel, savedRunes);
            }
            
            // Efeito visual dramático
            SpawnRadioactiveExplosion(Player.Center);
        }
        
        /// <summary>
        /// Potencializa o NPC que matou o jogador
        /// </summary>
        private void PowerUpKillerNPC(NPC killer, int weaponLevel, System.Collections.Generic.List<Data.EquippedRune> runes)
        {
            var leveledNPC = killer.GetGlobalNPC<LeveledEnemyGlobalNPC>();
            
            // Dobro do nível da arma perdida
            int newLevel = weaponLevel * 2;
            leveledNPC.SetLevelDirectly(newLevel);
            
            // Herda efeitos de runas elementais
            foreach (var rune in runes)
            {
                if (IsElementalRune(rune.Type))
                {
                    leveledNPC.AddElementalEffect(rune.Type, rune.Level);
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
        /// Efeito visual de explosão radioativa
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
