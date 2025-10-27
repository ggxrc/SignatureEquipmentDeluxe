using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.Config;
using SignatureEquipmentDeluxe.Common.Configs;
using SignatureEquipmentDeluxe.Common.Systems;

namespace SignatureEquipmentDeluxe.Common.GlobalItems
{
    /// <summary>
    /// Core GlobalItem que implementa todo o sistema de scaling de stats
    /// Baseado na arquitetura do mod Signature Equipment original
    /// </summary>
    public class SignatureGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        // ==================== DADOS PERSISTENTES ====================
        
        public int Level = 0;
        public int Experience = 0;
        
        // ==================== CACHE DE CONFIG ====================
        
        private ServerConfig GetServerConfig() => ModContent.GetInstance<ServerConfig>();
        private ClientConfig GetClientConfig() => ModContent.GetInstance<ClientConfig>();
        
        // ==================== MÉTODOS DE CÁLCULO DE STATS ====================
        
        /// <summary>
        /// Calcula um stat inteiro com cap aplicado
        /// </summary>
        private int GetStatCapped(ItemStatInt statConfig, Item item = null)
        {
            if (Level == 0) return 0;
            if (item != null && isBlackListed(item, statConfig.ItemBlackList)) return 0;
            
            int calculated = ScalingCalculator.CalculateStatInt(
                Level,
                statConfig.ScalingMode,
                statConfig.ScalingTiers,
                statConfig.PerLevel,
                statConfig.PerLevelMult
            );
            
            // Aplica hard cap específico do item
            if (item != null && isHardCapped(item, statConfig.HardCap, out var hardCap))
            {
                return Math.Min(calculated, hardCap.Max) * hardCap.Sign;
            }
            
            // Aplica cap global
            return Math.Min(calculated, statConfig.Max);
        }
        
        /// <summary>
        /// Calcula um stat inteiro sem cap aplicado
        /// </summary>
        private int GetStatUncapped(ItemStatInt statConfig, Item item = null)
        {
            if (Level == 0) return 0;
            if (item != null && isBlackListed(item, statConfig.ItemBlackList)) return 0;
            
            return ScalingCalculator.CalculateStatInt(
                Level,
                statConfig.ScalingMode,
                statConfig.ScalingTiers,
                statConfig.PerLevel,
                statConfig.PerLevelMult
            );
        }
        
        /// <summary>
        /// Calcula um stat float com cap aplicado
        /// </summary>
        private float GetStatCappedFloat(ItemStatFloat statConfig, Item item = null)
        {
            if (Level == 0) return 0f;
            if (item != null && isBlackListed(item, statConfig.BlackList)) return 0f;
            
            float calculated = ScalingCalculator.CalculateStat(
                Level,
                statConfig.ScalingMode,
                statConfig.ScalingTiers,
                statConfig.PerLevel,
                statConfig.PerLevelMult
            );
            
            // Aplica hard cap específico do item
            if (item != null && isHardCapped(item, statConfig.HardCap, out var hardCap))
            {
                return Math.Min(calculated, hardCap.Max) * hardCap.Sign;
            }
            
            // Aplica cap global
            return Math.Min(calculated, statConfig.Max);
        }
        
        /// <summary>
        /// Calcula um stat float sem cap aplicado
        /// </summary>
        private float GetStatUncappedFloat(ItemStatFloat statConfig, Item item = null)
        {
            if (Level == 0) return 0f;
            if (item != null && isBlackListed(item, statConfig.BlackList)) return 0f;
            
            return ScalingCalculator.CalculateStat(
                Level,
                statConfig.ScalingMode,
                statConfig.ScalingTiers,
                statConfig.PerLevel,
                statConfig.PerLevelMult
            );
        }
        
        /// <summary>
        /// Calcula um stat de projétil float com cap aplicado
        /// </summary>
        private float GetProjectileStatCappedFloat(ProjectileStatFloat statConfig, Item item = null)
        {
            if (Level == 0) return 0f;
            if (item != null && isBlackListed(item, statConfig.ItemBlackList)) return 0f;
            
            float calculated = ScalingCalculator.CalculateStat(
                Level,
                statConfig.ScalingMode,
                statConfig.ScalingTiers,
                statConfig.PerLevel,
                statConfig.PerLevelMult
            );
            
            // Aplica hard cap específico do item
            if (item != null && isItemHardCapped(item, statConfig.ItemHardCap, out var hardCap))
            {
                return Math.Min(calculated, hardCap.Max) * hardCap.Sign;
            }
            
            // Aplica cap global
            return Math.Min(calculated, statConfig.Max);
        }
        
        /// <summary>
        /// Calcula um stat de projétil float sem cap aplicado
        /// </summary>
        private float GetProjectileStatUncappedFloat(ProjectileStatFloat statConfig, Item item = null)
        {
            if (Level == 0) return 0f;
            if (item != null && isBlackListed(item, statConfig.ItemBlackList)) return 0f;
            
            return ScalingCalculator.CalculateStat(
                Level,
                statConfig.ScalingMode,
                statConfig.ScalingTiers,
                statConfig.PerLevel,
                statConfig.PerLevelMult
            );
        }
        
        // ==================== VERIFICADORES DE BLACKLIST/WHITELIST ====================
        
        private bool isBlackListed(Item item, HashSet<ItemDefinition> blacklist)
        {
            if (blacklist == null || blacklist.Count == 0) return false;
            return blacklist.Any(def => def.Type == item.type);
        }
        
        private bool isHardCapped(Item item, Dictionary<ItemDefinition, StatHardCap> hardCaps, out StatHardCap cap)
        {
            cap = null;
            if (hardCaps == null || hardCaps.Count == 0) return false;
            
            foreach (var kvp in hardCaps)
            {
                if (kvp.Key.Type == item.type)
                {
                    cap = kvp.Value;
                    return true;
                }
            }
            return false;
        }
        
        private bool isItemHardCapped(Item item, Dictionary<ItemDefinition, StatHardCap> hardCaps, out StatHardCap cap)
        {
            return isHardCapped(item, hardCaps, out cap);
        }
        
        // ==================== PROPRIEDADES DE STATS (CALCULADAS DINAMICAMENTE) ====================
        
        public int GetDamageCapped(Item item) => GetStatCapped(GetServerConfig().Damage, item);
        public int GetDamageNotCapped(Item item) => GetStatUncapped(GetServerConfig().Damage, item);
        
        public int GetDefenceCapped(Item item) => GetStatCapped(GetServerConfig().Defense, item);
        public int GetDefenceNotCapped(Item item) => GetStatUncapped(GetServerConfig().Defense, item);
        
        public int GetCritChanceCapped(Item item) => GetStatCapped(GetServerConfig().CritChance, item);
        public int GetCritChanceNotCapped(Item item) => GetStatUncapped(GetServerConfig().CritChance, item);
        
        public float GetUseSpeedCapped(Item item) => GetStatCappedFloat(GetServerConfig().UseSpeed, item);
        public float GetUseSpeedNotCapped(Item item) => GetStatUncappedFloat(GetServerConfig().UseSpeed, item);
        
        public float GetMeleeSpeedCapped(Item item) => GetStatCappedFloat(GetServerConfig().MeleeSpeed, item);
        public float GetMeleeSpeedNotCapped(Item item) => GetStatUncappedFloat(GetServerConfig().MeleeSpeed, item);
        
        public float GetMeleeSizeCapped(Item item) => GetStatCappedFloat(GetServerConfig().MeleeSize, item);
        public float GetMeleeSizeNotCapped(Item item) => GetStatUncappedFloat(GetServerConfig().MeleeSize, item);
        
        public float GetManaCostReductionCapped(Item item) => GetStatCappedFloat(GetServerConfig().ManaCostReduction, item);
        public float GetManaCostReductionNotCapped(Item item) => GetStatUncappedFloat(GetServerConfig().ManaCostReduction, item);
        
        public float GetAmmoConsumptionReductionCapped(Item item) => GetStatCappedFloat(GetServerConfig().AmmoConsumptionReduction, item);
        public float GetAmmoConsumptionReductionNotCapped(Item item) => GetStatUncappedFloat(GetServerConfig().AmmoConsumptionReduction, item);
        
        public float GetMinionSlotReductionCapped(Item item) => GetStatCappedFloat(GetServerConfig().MinionSlotReduction, item);
        public float GetMinionSlotReductionNotCapped(Item item) => GetStatUncappedFloat(GetServerConfig().MinionSlotReduction, item);
        
        public float GetProjectileSizeCapped(Item item) => GetProjectileStatCappedFloat(GetServerConfig().ProjectileSize, item);
        public float GetProjectileSizeNotCapped(Item item) => GetProjectileStatUncappedFloat(GetServerConfig().ProjectileSize, item);
        
        public float GetProjectileSpeedCapped(Item item) => GetProjectileStatCappedFloat(GetServerConfig().ProjectileSpeed, item);
        public float GetProjectileSpeedNotCapped(Item item) => GetProjectileStatUncappedFloat(GetServerConfig().ProjectileSpeed, item);
        
        public float GetProjectileLifeTimeCapped(Item item) => GetProjectileStatCappedFloat(GetServerConfig().ProjectileLifeTime, item);
        public float GetProjectileLifeTimeNotCapped(Item item) => GetProjectileStatUncappedFloat(GetServerConfig().ProjectileLifeTime, item);
        
        public float GetProjectilePenetrationCapped(Item item) => GetProjectileStatCappedFloat(GetServerConfig().ProjectilePenetration, item);
        public float GetProjectilePenetrationNotCapped(Item item) => GetProjectileStatUncappedFloat(GetServerConfig().ProjectilePenetration, item);
        
        // ==================== INTEGRAÇÃO COM PLAYER ====================
        
        /// <summary>
        /// Ao acertar um NPC, ganha XP
        /// </summary>
        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Level > 0 || CanGainExperience(item))
            {
                // Fórmula base de XP: 1 XP por hit + % do dano causado
                float xpGain = 1f + (damageDone * 0.01f);
                
                // Aplica multiplicadores do servidor
                var config = GetServerConfig();
                xpGain *= config.GlobalExpMultiplier;
                xpGain *= config.WeaponExpMultiplier;
                
                // Boss multiplier
                if (target.boss)
                {
                    xpGain *= config.BossExpMultiplier;
                }
                
                AddExperience((int)xpGain);
            }
        }
        
        /// <summary>
        /// Verifica se o item pode ganhar XP (é arma, armor ou accessory, mas NÃO munição)
        /// </summary>
        public bool CanGainExperience(Item item)
        {
            // Munições não ganham XP
            if (item.ammo > 0 || item.consumable && item.damage > 0)
                return false;
                
            // Blacklist global
            var config = GetServerConfig();
            if (config.GlobalItemBlacklist != null && config.GlobalItemBlacklist.Any(def => def.Type == item.type))
                return false;
            
            return item.damage > 0 || item.defense > 0 || item.accessory;
        }
        
        // ==================== HOOKS DE MODIFICAÇÃO ====================
        
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (Level > 0 && item.damage > 0)
            {
                int damageBonus = GetDamageCapped(item);
                if (damageBonus > 0)
                {
                    damage.Flat += damageBonus;
                }
            }
        }
        
        public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            if (Level > 0 && item.damage > 0)
            {
                int critBonus = GetCritChanceCapped(item);
                if (critBonus > 0)
                {
                    crit += critBonus;
                }
            }
        }
        
        public override float UseSpeedMultiplier(Item item, Player player)
        {
            if (Level > 0 && item.useTime > 0)
            {
                float speedBonus = GetUseSpeedCapped(item);
                if (speedBonus > 0)
                {
                    // Reduz o use time (valores menores = mais rápido)
                    return 1f / (1f + speedBonus / 100f);
                }
            }
            return 1f;
        }
        
        public override void HoldItem(Item item, Player player)
        {
            if (Level > 0 && item.CountsAsClass(DamageClass.Melee))
            {
                float meleeSpeedBonus = GetMeleeSpeedCapped(item);
                if (meleeSpeedBonus > 0)
                {
                    player.GetAttackSpeed(DamageClass.Melee) += meleeSpeedBonus / 100f;
                }
                
                float meleeSizeBonus = GetMeleeSizeCapped(item);
                if (meleeSizeBonus > 0)
                {
                    player.GetDamage(DamageClass.Melee).Flat += meleeSizeBonus / 100f;
                }
            }
        }
        
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (Level > 0 && item.accessory)
            {
                int defenseBonus = GetDefenceCapped(item);
                if (defenseBonus > 0)
                {
                    player.statDefense += defenseBonus;
                }
            }
        }
        
        public override void UpdateEquip(Item item, Player player)
        {
            if (Level > 0 && item.defense > 0)
            {
                int defenseBonus = GetDefenceCapped(item);
                if (defenseBonus > 0)
                {
                    player.statDefense += defenseBonus;
                }
            }
        }
        
        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            if (Level > 0 && item.mana > 0)
            {
                float manaReduction = GetManaCostReductionCapped(item);
                if (manaReduction > 0)
                {
                    mult *= 1f - (manaReduction / 100f);
                }
            }
        }
        
        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player)
        {
            if (Level > 0 && weapon.useAmmo > 0)
            {
                float ammoReduction = GetAmmoConsumptionReductionCapped(weapon);
                if (ammoReduction > 0 && Main.rand.NextFloat(100f) < ammoReduction)
                {
                    return false;
                }
            }
            return base.CanConsumeAmmo(weapon, ammo, player);
        }
        
        // ==================== TOOLTIPS ====================
        
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            // Verifica se o item pode ter XP
            if (!CanGainExperience(item)) return;
            
            var clientConfig = GetClientConfig();
            
            // Items começam no nível 0, não precisamos inicializar
            
            if (!clientConfig.ShowItemLevel && !clientConfig.ShowItemStats) return;
            
            // Adiciona separador
            if (clientConfig.ShowTooltipSeparator)
            {
                tooltips.Add(new TooltipLine(Mod, "SignatureSeparator", "―――――――――――――――――――")
                {
                    OverrideColor = new Color(100, 100, 100)
                });
            }
            
            // Mostra nível
            if (clientConfig.ShowItemLevel)
            {
                tooltips.Add(new TooltipLine(Mod, "SignatureLevel", $"⭐ Level {Level} ⭐")
                {
                    OverrideColor = clientConfig.TooltipLevelColor
                });
            }
            
            // Mostra experiência
            if (clientConfig.ShowItemExperience)
            {
                int maxLevel = GetMaxLevelForItem(item);
                
                // Se já está no max level (e não é 0 = infinito)
                if (maxLevel > 0 && Level >= maxLevel)
                {
                    tooltips.Add(new TooltipLine(Mod, "SignatureXP", "⚡ MAX LEVEL ⚡")
                    {
                        OverrideColor = Color.Gold
                    });
                }
                else
                {
                    int required = GetRequiredXP(Level);
                    float percent = (float)Experience / required * 100f;
                    
                    string expText = $"XP: {Experience}/{required} ({percent:F1}%)";
                    tooltips.Add(new TooltipLine(Mod, "SignatureXP", expText)
                    {
                        OverrideColor = clientConfig.TooltipExpColor
                    });
                    
                    // Barra de progresso
                    if (clientConfig.ShowProgressBar)
                    {
                        int barLength = 20;
                        int filled = (int)(barLength * (Experience / (float)required));
                        string bar = "[" + new string('█', filled) + new string('░', barLength - filled) + "]";
                        tooltips.Add(new TooltipLine(Mod, "SignatureProgressBar", bar)
                        {
                            OverrideColor = clientConfig.TooltipExpColor
                        });
                    }
                }
            }
            
            // Mostra stats
            if (clientConfig.ShowItemStats)
            {
                AddStatTooltip(tooltips, "Damage", GetDamageCapped(item), clientConfig);
                AddStatTooltip(tooltips, "Defense", GetDefenceCapped(item), clientConfig);
                AddStatTooltip(tooltips, "Crit Chance", GetCritChanceCapped(item), clientConfig, "%");
                AddStatTooltip(tooltips, "Use Speed", GetUseSpeedCapped(item), clientConfig, "%");
                AddStatTooltip(tooltips, "Melee Speed", GetMeleeSpeedCapped(item), clientConfig, "%");
                AddStatTooltip(tooltips, "Melee Size", GetMeleeSizeCapped(item), clientConfig, "%");
                AddStatTooltip(tooltips, "Mana Cost", -GetManaCostReductionCapped(item), clientConfig, "%");
                AddStatTooltip(tooltips, "Ammo Save", GetAmmoConsumptionReductionCapped(item), clientConfig, "%");
            }
        }
        
        private void AddStatTooltip(List<TooltipLine> tooltips, string statName, float value, ClientConfig config, string suffix = "")
        {
            if (value == 0) return;
            
            string sign = value > 0 ? "+" : "";
            string text = $"{sign}{value:F0}{suffix} {statName}";
            
            tooltips.Add(new TooltipLine(Mod, $"Signature{statName.Replace(" ", "")}", text)
            {
                OverrideColor = config.TooltipStatColor
            });
        }
        
        // ==================== OUTLINE VISUAL ====================
        
        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, 
            Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Level == 0) return true;
            
            var clientConfig = GetClientConfig();
            if (!clientConfig.EnableOutline) return true;
            
            Texture2D texture = TextureAssets.Item[item.type].Value;
            Color outlineColor = GetOutlineColor(Level, clientConfig);
            outlineColor *= clientConfig.OutlineOpacity;
            
            if (clientConfig.OutlineMode == OutlineMode.Scale)
            {
                // Desenha outline com scale
                float outlineScale = scale * (1f + clientConfig.OutlineThickness * 0.1f);
                spriteBatch.Draw(texture, position, frame, outlineColor, 0f, origin, outlineScale, SpriteEffects.None, 0f);
            }
            else
            {
                // Desenha outline com cópias ao redor
                int thickness = (int)clientConfig.OutlineThickness;
                for (int x = -thickness; x <= thickness; x++)
                {
                    for (int y = -thickness; y <= thickness; y++)
                    {
                        if (x == 0 && y == 0) continue;
                        Vector2 offset = new Vector2(x, y);
                        spriteBatch.Draw(texture, position + offset, frame, outlineColor, 0f, origin, scale, SpriteEffects.None, 0f);
                    }
                }
            }
            
            return true;
        }
        
        private Color GetOutlineColor(int level, ClientConfig config)
        {
            if (level <= 25) return config.OutlineColor_Level1_25;
            if (level <= 50) return config.OutlineColor_Level26_50;
            if (level <= 75) return config.OutlineColor_Level51_75;
            if (level <= 100) return config.OutlineColor_Level76_100;
            return config.OutlineColor_Level101Plus;
        }
        
        // ==================== SAVE/LOAD ====================
        
        public override void SaveData(Item item, TagCompound tag)
        {
            if (Level > 0)
            {
                tag["Level"] = Level;
                tag["Experience"] = Experience;
            }
        }
        
        public override void LoadData(Item item, TagCompound tag)
        {
            Level = tag.GetInt("Level");
            Experience = tag.GetInt("Experience");
        }
        
        // ==================== UTILIDADES ====================
        
        /// <summary>
        /// Adiciona experiência ao item
        /// </summary>
        public void AddExperience(int amount, Item item = null)
        {
            if (amount <= 0) return;
            
            // Verifica max level (0 = sem limite)
            int maxLevel = GetMaxLevelForItem(item);
            if (maxLevel > 0 && Level >= maxLevel) return;
            
            Experience += amount;
            
            var clientConfig = GetClientConfig();
            
            // Mostra XP ganho se habilitado
            if (clientConfig.ShowExpGainNotification && Main.netMode != NetmodeID.Server)
            {
                CombatText.NewText(Main.LocalPlayer.getRect(), clientConfig.TooltipExpColor, $"+{amount} XP");
            }
            
            // Verifica level up
            int requiredXP = GetRequiredXP(Level);
            bool hasMaxLevel = maxLevel > 0;
            while (Experience >= requiredXP && (!hasMaxLevel || Level < maxLevel))
            {
                Experience -= requiredXP;
                Level++;
                requiredXP = GetRequiredXP(Level);
                
                // Notificação de level up
                if (Main.netMode != NetmodeID.Server && clientConfig.ShowLevelUpNotification)
                {
                    CombatText.NewText(Main.LocalPlayer.getRect(), Color.Gold, $"Level Up! ({Level})", dramatic: true);
                    
                    // Efeitos visuais
                    if (clientConfig.EnableParticleEffects)
                    {
                        for (int i = 0; i < 30; i++)
                        {
                            Dust dust = Dust.NewDustDirect(Main.LocalPlayer.position, Main.LocalPlayer.width, Main.LocalPlayer.height,
                                DustID.GoldCoin, 0f, 0f, 100, Color.Gold, 2f);
                            dust.velocity *= 3f;
                            dust.noGravity = true;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Obtém o max level para este item (individual ou global)
        /// </summary>
        /// <summary>
        /// Obtém o max level para este item (individual ou global)
        /// Se retornar 0, significa sem limite
        /// </summary>
        public int GetMaxLevelForItem(Item item)
        {
            var config = GetServerConfig();
            
            // Verifica se tem max level individual
            if (item != null && config.IndividualMaxLevel != null)
            {
                foreach (var kvp in config.IndividualMaxLevel)
                {
                    if (kvp.Key.Type == item.type)
                        return kvp.Value;
                }
            }
            
            // Usa max level global (0 = sem limite)
            return config.GlobalMaxLevel;
        }
        
        /// <summary>
        /// Calcula XP necessário para alcançar um nível - FORMULA CONFIGURÁVEL
        /// </summary>
        public int GetRequiredXP(int level)
        {
            var config = GetServerConfig();
            
            // Fórmula: BaseXP * (1 + level * ScalingFactor)^ExpScalingFactor
            double baseXP = config.BaseExpPerLevel;
            double scalingFactor = config.ExpScalingFactor;
            
            return (int)(baseXP * Math.Pow(scalingFactor, level - 1));
        }
    }
}