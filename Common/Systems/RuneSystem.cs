using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SignatureEquipmentDeluxe.Common.Data;
using SignatureEquipmentDeluxe.Common.Configs;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// Sistema que gerencia runas e maldições em armas
    /// </summary>
    public static class RuneSystem
    {
        /// <summary>
        /// Verifica se o item pode ter runas (apenas armas por enquanto)
        /// </summary>
        public static bool CanHaveRunes(Item item)
        {
            return item != null && item.damage > 0 && !item.accessory;
        }
        
        /// <summary>
        /// Retorna o número máximo de slots baseado no nível da arma
        /// </summary>
        public static int GetMaxRuneSlots(int weaponLevel)
        {
            var config = ModContent.GetInstance<ServerConfig>();
            if (!config.EnableRuneSystem)
                return 0;
            
            int slots = 0;
            if (weaponLevel >= config.RuneSlot1Level) slots++;
            if (weaponLevel >= config.RuneSlot2Level) slots++;
            if (weaponLevel >= config.RuneSlot3Level) slots++;
            if (weaponLevel >= config.RuneSlot4Level) slots++;
            if (weaponLevel >= config.RuneSlot5Level) slots++;
            
            return slots;
        }
        
        /// <summary>
        /// Adiciona XP às runas equipadas (chamado quando arma ganha XP)
        /// </summary>
        public static void AddXPToRunes(List<EquippedRune> runes, int amount, bool isKill, int weaponMaxLevel)
        {
            if (runes == null || runes.Count == 0)
                return;
            
            var config = ModContent.GetInstance<ServerConfig>();
            float multiplier = isKill ? config.RuneXPPerKillMultiplier : config.RuneXPPerHitMultiplier;
            
            int runeXP = (int)(amount * multiplier);
            if (runeXP <= 0)
                return;
            
            foreach (var rune in runes)
            {
                // Atualiza max level da runa baseado na arma
                rune.MaxLevel = weaponMaxLevel;
                
                bool leveledUp = rune.AddExperience(runeXP);
                if (leveledUp && Main.LocalPlayer != null)
                {
                    // TODO: Notificação de level up da runa
                }
            }
        }
        
        /// <summary>
        /// Calcula o multiplicador total de XP das maldições
        /// </summary>
        public static float GetCurseXPMultiplier(List<EquippedRune> runes, bool isKill)
        {
            if (runes == null || runes.Count == 0)
                return 1f;
            
            var config = ModContent.GetInstance<ServerConfig>();
            if (!config.EnableCurseSystem)
                return 1f;
            
            int curseCount = runes.Count(r => r.IsCurse());
            if (curseCount == 0)
                return 1f;
            
            float bonusPerCurse = isKill ? config.CurseXPBonusPerKill : config.CurseXPBonusPerHit;
            return 1f + (curseCount * bonusPerCurse);
        }
        
        /// <summary>
        /// Calcula a chance total de dropar a arma ao morrer
        /// </summary>
        public static float GetCurseDropChance(List<EquippedRune> runes)
        {
            if (runes == null || runes.Count == 0)
                return 0f;
            
            var config = ModContent.GetInstance<ServerConfig>();
            if (!config.EnableCurseSystem)
                return 0f;
            
            int curseCount = runes.Count(r => r.IsCurse());
            return curseCount * config.CurseDropChancePerCurse;
        }
        
        /// <summary>
        /// Aplica bônus de dano das runas (chamado por ModifyWeaponDamage)
        /// </summary>
        public static void ApplyRuneDamageBonus(List<EquippedRune> runes, ref StatModifier damage)
        {
            if (runes == null || runes.Count == 0)
                return;
            
            foreach (var rune in runes)
            {
                float bonus = 0f;
                
                switch (rune.Type)
                {
                    case RuneType.Fire:
                    case RuneType.Ice:
                    case RuneType.Poison:
                    case RuneType.Lightning:
                        bonus = RuneDefinitions.GetDamageBonus(rune.Type, rune.Level);
                        break;
                    
                    case RuneType.CurseBerserker:
                        bonus = 50f; // +50% damage
                        break;
                    
                    case RuneType.CurseAnnihilation:
                        bonus = 150f; // +150% damage
                        break;
                }
                
                if (bonus > 0)
                {
                    damage += bonus / 100f; // Converte para multiplicador (ex: 50% = 0.5)
                }
            }
        }
        
        /// <summary>
        /// Retorna multiplicador de velocidade de ataque das runas
        /// </summary>
        public static float GetRuneAttackSpeedMultiplier(List<EquippedRune> runes)
        {
            if (runes == null || runes.Count == 0)
                return 1f;
            
            float totalSpeedBonus = 0f;
            
            foreach (var rune in runes)
            {
                if (rune.Type == RuneType.AttackSpeed)
                {
                    totalSpeedBonus += RuneDefinitions.GetAttackSpeedBonus(rune.Type, rune.Level);
                }
            }
            
            // Retorna multiplicador (ex: 25% speed = 1.25x)
            return 1f + (totalSpeedBonus / 100f);
        }
        
        /// <summary>
        /// Aplica bônus de crit das curses
        /// </summary>
        public static void ApplyRuneCritBonus(List<EquippedRune> runes, ref float crit)
        {
            if (runes == null || runes.Count == 0)
                return;
            
            foreach (var rune in runes)
            {
                if (rune.Type == RuneType.CurseGlass)
                {
                    crit += 100f; // +100% crit chance
                }
            }
        }
        
        /// <summary>
        /// Aplica efeitos passivos das runas (LifeRegen, penalties de vida/defesa)
        /// Chamado por HoldItem (mas não modifica stats de combate)
        /// </summary>
        public static void ApplyRunePassiveEffects(List<EquippedRune> runes, Player player)
        {
            if (runes == null || runes.Count == 0)
                return;
            
            foreach (var rune in runes)
            {
                switch (rune.Type)
                {
                    case RuneType.LifeRegen:
                        int regenBonus = RuneDefinitions.GetLifeRegenBonus(rune.Type, rune.Level);
                        player.lifeRegen += regenBonus;
                        break;
                    
                    case RuneType.CurseGlass:
                        // Penalidade de vida máxima
                        player.statLifeMax2 = (int)(player.statLifeMax2 * 0.5f);
                        break;
                    
                    case RuneType.CurseAnnihilation:
                        // Penalidade de vida máxima
                        player.statLifeMax2 = (int)(player.statLifeMax2 * 0.65f);
                        break;
                }
            }
        }
        
        /// <summary>
        /// Aplica efeitos das runas nos stats da arma (DEPRECATED - usar métodos específicos)
        /// </summary>
        public static void ApplyRuneEffectsToWeapon(List<EquippedRune> runes, Item item, Player player)
        {
            // Mantido para compatibilidade, mas não deve ser usado
        }
        
        /// <summary>
        /// Processa efeitos especiais de runas ao acertar inimigo
        /// </summary>
        public static void ProcessRuneOnHitEffects(List<EquippedRune> runes, NPC target, int damage, Player player)
        {
            if (runes == null || runes.Count == 0)
                return;
            
            foreach (var rune in runes)
            {
                ProcessSingleRuneOnHit(rune, target, damage, player);
            }
        }
        
        /// <summary>
        /// Processa efeito de uma única runa ao acertar
        /// </summary>
        private static void ProcessSingleRuneOnHit(EquippedRune rune, NPC target, int damage, Player player)
        {
            switch (rune.Type)
            {
                case RuneType.Fire:
                    target.AddBuff(BuffID.OnFire, 180); // 3 segundos de queima
                    SpawnFireParticles(target.Center);
                    break;
                
                case RuneType.Ice:
                    target.AddBuff(BuffID.Chilled, 120); // 2 segundos de slow
                    if (Main.rand.NextBool(3)) // 33% chance de freeze
                        target.AddBuff(BuffID.Frozen, 60);
                    SpawnIceParticles(target.Center);
                    break;
                
                case RuneType.Poison:
                    target.AddBuff(BuffID.Poisoned, 300); // 5 segundos de veneno
                    SpawnPoisonParticles(target.Center);
                    break;
                
                case RuneType.Lightning:
                    // TODO: Implementar chain lightning (futura expansão)
                    SpawnLightningParticles(target.Center);
                    break;
                
                case RuneType.Lifesteal:
                    float lifestealPercent = RuneDefinitions.GetLifestealBonus(rune.Type, rune.Level);
                    int healAmount = (int)(damage * (lifestealPercent / 100f));
                    if (healAmount > 0)
                    {
                        player.statLife += healAmount;
                        player.HealEffect(healAmount);
                    }
                    break;
            }
        }
        
        // === PARTÍCULAS VISUAIS ===
        
        private static void SpawnFireParticles(Vector2 position)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(position, 10, 10, DustID.Torch, 
                    Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 
                    0, default, 1.2f);
            }
        }
        
        private static void SpawnIceParticles(Vector2 position)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(position, 10, 10, DustID.Ice, 
                    Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f), 
                    0, default, 1.0f);
            }
        }
        
        private static void SpawnPoisonParticles(Vector2 position)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(position, 10, 10, DustID.GreenTorch, 
                    Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f), 
                    0, default, 1.0f);
            }
        }
        
        private static void SpawnLightningParticles(Vector2 position)
        {
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDust(position, 10, 10, DustID.Electric, 
                    Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 
                    0, default, 1.5f);
            }
        }
    }
}
