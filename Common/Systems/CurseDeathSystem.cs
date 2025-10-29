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
            DropWeaponAndCreateRadioactiveZone(item, sigItem.Level, killerNPC, sigItem.EquippedRunes);
            
            // Remove o item do inventário
            item.TurnToAir();
            
            // Mensagem para o jogador
            Main.NewText($"Your cursed {item.Name} was dropped and created a radioactive zone!", 
                new Color(255, 100, 100));
        }
        
        /// <summary>
        /// Dropa a arma e cria/aumenta zona radioativa
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
            
            // Verifica se já está em zona radioativa
            bool isInExistingZone = LeveledEnemySystem.IsInRadioactiveZone(Player.Center, out int existingMaxLevel, out Vector2 zoneCenter, out float zoneRadius);
            
            if (isInExistingZone)
            {
                // AUMENTA O NÍVEL DE PERIGO DA ZONA EXISTENTE ao invés de criar nova
                IncreaseZoneDangerLevel(Player.Center, weaponLevel, item.type);
            }
            else
            {
                // Cria nova zona com animação épica
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
            }
            
            // Se foi morto por um NPC, ele herda poderes
            if (killerNPC != null && killerNPC.active)
            {
                PowerUpKillerNPC(killerNPC, weaponLevel, savedRunes);
            }
            
            // Efeito visual dramático
            SpawnRadioactiveExplosion(Player.Center);
        }
        
        /// <summary>
        /// Aumenta o nível de perigo de uma zona existente
        /// </summary>
        private void IncreaseZoneDangerLevel(Vector2 position, int weaponLevel, int itemType)
        {
            // Encontra a zona
            foreach (var zone in LeveledEnemySystem.radioactiveZones)
            {
                float distance = Vector2.Distance(position, zone.Position);
                if (distance <= zone.Radius)
                {
                    // Aumenta o nível máximo da zona (adiciona 50% do nível da arma)
                    int levelIncrease = System.Math.Max(1, weaponLevel / 2);
                    zone.MaxEnemyLevel += levelIncrease;
                    
                    // Força atualização do danger level
                    zone.UpdateDangerLevel();
                    
                    // ANIMAÇÃO ÉPICA DE UPGRADE DA ZONA
                    SpawnZoneUpgradeAnimation(zone.Position, zone.Radius, itemType);
                    
                    // Mensagem dramática
                    if (Main.netMode != Terraria.ID.NetmodeID.Server)
                    {
                        Main.NewText($"☢ The zone's power increases! New level: {zone.MaxEnemyLevel} ☢", Color.Red);
                    }
                    
                    // Som épico
                    Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.Roar, position);
                    
                    break; // Só aumenta a primeira zona encontrada
                }
            }
        }
        
        /// <summary>
        /// Animação épica quando a zona aumenta de poder
        /// </summary>
        private void SpawnZoneUpgradeAnimation(Vector2 zoneCenter, float zoneRadius, int itemType)
        {
            // Onda de choque expandindo do centro
            for (int ring = 0; ring < 5; ring++)
            {
                int particlesInRing = 60;
                float ringRadius = zoneRadius * (0.2f + ring * 0.2f);
                
                for (int i = 0; i < particlesInRing; i++)
                {
                    float angle = (i / (float)particlesInRing) * MathHelper.TwoPi;
                    Vector2 pos = zoneCenter + new Vector2(
                        (float)System.Math.Cos(angle) * ringRadius,
                        (float)System.Math.Sin(angle) * ringRadius
                    );
                    
                    Dust shockwave = Dust.NewDustPerfect(pos, Terraria.ID.DustID.Electric, Vector2.Zero, 0, Color.Red, 2.5f);
                    shockwave.noGravity = true;
                }
            }
            
            // Explosão central massiva
            for (int i = 0; i < 150; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(12f, 12f);
                Dust explosion = Dust.NewDustPerfect(
                    zoneCenter,
                    Terraria.ID.DustID.CursedTorch,
                    velocity,
                    0,
                    Color.OrangeRed,
                    Main.rand.NextFloat(2f, 3.5f)
                );
                explosion.noGravity = true;
            }
            
            // Spawn a arma levitando no centro temporariamente (5 segundos)
            if (Main.netMode != Terraria.ID.NetmodeID.MultiplayerClient)
            {
                int projectile = Projectile.NewProjectile(
                    Main.LocalPlayer.GetSource_FromThis(),
                    zoneCenter,
                    Vector2.Zero,
                    ModContent.ProjectileType<Content.Projectiles.ZoneUpgradeVisual>(),
                    0,
                    0f,
                    Main.myPlayer,
                    0f,
                    itemType // ai[1] = tipo do item
                );
            }
            
            // Som de poder crescente
            Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.DD2_BetsyScream, zoneCenter);
        }
        
        /// <summary>
        /// Potencializa o NPC que matou o jogador
        /// </summary>
        private void PowerUpKillerNPC(NPC killer, int weaponLevel, System.Collections.Generic.List<Data.EquippedRune> runes)
        {
            var leveledNPC = killer.GetGlobalNPC<LeveledEnemyGlobalNPC>();
            
            // Dobro do nível da arma perdida
            int newLevel = weaponLevel * 2;
            leveledNPC.SetLevelDirectly(newLevel, killer); // Passa o NPC para aplicar scaling
            
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
