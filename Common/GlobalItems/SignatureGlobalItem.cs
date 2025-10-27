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
            
            // Debug
            var serverConfig = GetServerConfig();
            if (serverConfig.DebugMode && calculated > 0 && Main.netMode != NetmodeID.Server)
            {
                Main.NewText($"[DEBUG] GetStatCapped: Level={Level}, PerLevel={statConfig.PerLevel}, Calculated={calculated}, Max={statConfig.Max}");
            }
            
            // Aplica hard cap específico do item
            if (item != null && isHardCapped(item, statConfig.HardCap, out var hardCap))
            {
                return Math.Min(calculated, hardCap.Max) * hardCap.Sign;
            }
            
            // Aplica cap global (0 = sem limite)
            if (statConfig.Max > 0)
            {
                return Math.Min(calculated, statConfig.Max);
            }
            
            return calculated;
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
            
            // Aplica cap global (0 = sem limite)
            if (statConfig.Max > 0)
            {
                return Math.Min(calculated, statConfig.Max);
            }
            
            return calculated;
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
            
            // Aplica cap global (0 = sem limite)
            if (statConfig.Max > 0)
            {
                return Math.Min(calculated, statConfig.Max);
            }
            
            return calculated;
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
        
        // Weapon Stats
        public int GetDamageCapped(Item item) => GetStatCapped(GetServerConfig().WeaponDamage, item);
        public int GetDamageNotCapped(Item item) => GetStatUncapped(GetServerConfig().WeaponDamage, item);
        
        public int GetCritChanceCapped(Item item) => GetStatCapped(GetServerConfig().WeaponCritChance, item);
        public int GetCritChanceNotCapped(Item item) => GetStatUncapped(GetServerConfig().WeaponCritChance, item);
        
        public float GetUseTimeCapped(Item item) => GetStatCappedFloat(GetServerConfig().WeaponUseTime, item);
        public float GetUseTimeNotCapped(Item item) => GetStatUncappedFloat(GetServerConfig().WeaponUseTime, item);
        public float GetUseTimeCappedPercent(Item item) => GetUseTimeCapped(item) * 0.01f;
        public float GetUseTimeNotCappedPercent(Item item) => GetUseTimeNotCapped(item) * 0.01f;
        
        public float GetUseAnimationCapped(Item item) => GetStatCappedFloat(GetServerConfig().WeaponUseAnimation, item);
        public float GetUseAnimationNotCapped(Item item) => GetStatUncappedFloat(GetServerConfig().WeaponUseAnimation, item);
        public float GetUseAnimationCappedPercent(Item item) => GetUseAnimationCapped(item) * 0.01f;
        public float GetUseAnimationNotCappedPercent(Item item) => GetUseAnimationNotCapped(item) * 0.01f;
        
        public float GetMeleeSizeCapped(Item item) => GetStatCappedFloat(GetServerConfig().WeaponMeleeSize, item);
        public float GetMeleeSizeNotCapped(Item item) => GetStatUncappedFloat(GetServerConfig().WeaponMeleeSize, item);
        public float GetMeleeSizeCappedPercent(Item item) => GetMeleeSizeCapped(item) * 0.01f;
        public float GetMeleeSizeNotCappedPercent(Item item) => GetMeleeSizeNotCapped(item) * 0.01f;
        
        public float GetManaCostReductionCapped(Item item) => GetStatCappedFloat(GetServerConfig().WeaponManaCostReduction, item);
        public float GetManaCostReductionNotCapped(Item item) => GetStatUncappedFloat(GetServerConfig().WeaponManaCostReduction, item);
        public float GetManaCostReductionCappedPercent(Item item) => GetManaCostReductionCapped(item) * 0.01f;
        public float GetManaCostReductionNotCappedPercent(Item item) => GetManaCostReductionNotCapped(item) * 0.01f;
        
        public float GetAmmoConsumptionReductionCapped(Item item) => GetStatCappedFloat(GetServerConfig().WeaponAmmoConsumptionReduction, item);
        public float GetAmmoConsumptionReductionNotCapped(Item item) => GetStatUncappedFloat(GetServerConfig().WeaponAmmoConsumptionReduction, item);
        public float GetAmmoConsumptionReductionCappedPercent(Item item) => GetAmmoConsumptionReductionCapped(item) * 0.01f;
        public float GetAmmoConsumptionReductionNotCappedPercent(Item item) => GetAmmoConsumptionReductionNotCapped(item) * 0.01f;
        
        // Projectile Stats
        public float GetProjectileSizeCapped(Item item) => GetProjectileStatCappedFloat(GetServerConfig().WeaponProjectileSize, item);
        public float GetProjectileSizeNotCapped(Item item) => GetProjectileStatUncappedFloat(GetServerConfig().WeaponProjectileSize, item);
        public float GetProjectileSizeCappedPercent(Item item) => GetProjectileSizeCapped(item) * 0.01f;
        public float GetProjectileSizeNotCappedPercent(Item item) => GetProjectileSizeNotCapped(item) * 0.01f;
        
        public float GetProjectileSpeedCapped(Item item) => GetProjectileStatCappedFloat(GetServerConfig().WeaponProjectileSpeed, item);
        public float GetProjectileSpeedNotCapped(Item item) => GetProjectileStatUncappedFloat(GetServerConfig().WeaponProjectileSpeed, item);
        public float GetProjectileSpeedCappedPercent(Item item) => GetProjectileSpeedCapped(item) * 0.01f;
        public float GetProjectileSpeedNotCappedPercent(Item item) => GetProjectileSpeedNotCapped(item) * 0.01f;
        
        public float GetProjectileLifeTimeCapped(Item item) => GetProjectileStatCappedFloat(GetServerConfig().WeaponProjectileLifeTime, item);
        public float GetProjectileLifeTimeNotCapped(Item item) => GetProjectileStatUncappedFloat(GetServerConfig().WeaponProjectileLifeTime, item);
        public float GetProjectileLifeTimeCappedPercent(Item item) => GetProjectileLifeTimeCapped(item) * 0.01f;
        public float GetProjectileLifeTimeNotCappedPercent(Item item) => GetProjectileLifeTimeNotCapped(item) * 0.01f;
        
        public float GetProjectilePenetrationCapped(Item item) => GetProjectileStatCappedFloat(GetServerConfig().WeaponProjectilePenetration, item);
        public float GetProjectilePenetrationNotCapped(Item item) => GetProjectileStatUncappedFloat(GetServerConfig().WeaponProjectilePenetration, item);
        public float GetProjectilePenetrationCappedPercent(Item item) => GetProjectilePenetrationCapped(item) * 0.01f;
        public float GetProjectilePenetrationNotCappedPercent(Item item) => GetProjectilePenetrationNotCapped(item) * 0.01f;
        
        public float GetAdditionalProjectileChanceCapped(Item item) => GetProjectileStatCappedFloat(GetServerConfig().WeaponAdditionalProjectileChance, item);
        public float GetAdditionalProjectileChanceNotCapped(Item item) => GetProjectileStatUncappedFloat(GetServerConfig().WeaponAdditionalProjectileChance, item);
        
        // Armor Stats
        public int GetDefenceCapped(Item item) => GetStatCapped(GetServerConfig().ArmorDefense, item);
        public int GetDefenceNotCapped(Item item) => GetStatUncapped(GetServerConfig().ArmorDefense, item);
        
        // Accessory Stats (sem stats próprios - XP vai para armas)
        
        // ==================== INTEGRAÇÃO COM PLAYER ====================
        // O XP agora é distribuído pelo SignaturePlayer.cs via OnHitNPC hook
        // Removido o OnHitNPC aqui para evitar duplicação
        
        /// <summary>
        /// Verifica se o item pode ganhar XP (é arma ou armor, mas NÃO munição nem acessório)
        /// </summary>
        public bool CanGainExperience(Item item)
        {
            // Munições não ganham XP
            if (item.ammo > 0 || item.consumable && item.damage > 0)
                return false;
            
            // Acessórios não ganham XP (XP vai para armas equipadas)
            if (item.accessory)
                return false;
                
            // Blacklist global
            var config = GetServerConfig();
            if (config.GlobalItemBlacklist != null && config.GlobalItemBlacklist.Any(def => def.Type == item.type))
                return false;
            
            return item.damage > 0 || item.defense > 0;
        }
        
        // ==================== HOOKS DE MODIFICAÇÃO ====================
        
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            var config = GetServerConfig();
            
            // Verifica se o item está na blacklist
            if (isBlackListed(item, config.WeaponDamage.ItemBlackList))
                return;
            
            // Verifica se o incremento de dano está habilitado
            if (!config.DamageIncrement)
                return;
            
            // Verifica se o item tem dano
            if (Level == 0 || item.damage <= 0)
                return;
            
            // Calcula o dano com HardCap se existir
            int statDamage;
            if (isHardCapped(item, config.WeaponDamage.HardCap, out var cap))
            {
                int uncappedDamage = GetDamageNotCapped(item);
                statDamage = Math.Clamp(uncappedDamage, int.MinValue, cap.Max) * cap.Sign;
            }
            else
            {
                statDamage = GetDamageCapped(item);
            }
            
            // Aplica o dano de acordo com os modos configurados
            if (config.IncreaseBaseDamage)
                damage.Base += statDamage;
            
            if (config.IncreaseFlatDamage)
                damage.Flat += statDamage;
            
            if (config.IncreaseMultDamage)
                damage += statDamage * 0.01f;
            
            // Debug
            if (config.DebugMode && statDamage > 0)
            {
                Main.NewText($"[DEBUG] Item: {item.Name}, Level: {Level}, Damage Bonus: +{statDamage} (Base: {config.IncreaseBaseDamage}, Flat: {config.IncreaseFlatDamage}, Mult: {config.IncreaseMultDamage})");
            }
        }
        
        public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            var config = GetServerConfig();
            
            // Verifica se o item está na blacklist
            if (isBlackListed(item, config.WeaponCritChance.ItemBlackList))
                return;
            
            // Verifica se o incremento de crit está habilitado
            if (!config.CritIncrement)
                return;
            
            // Verifica se o item tem dano
            if (Level == 0 || item.damage <= 0)
                return;
            
            // Calcula o crit com HardCap se existir
            int statCrit;
            if (isHardCapped(item, config.WeaponCritChance.HardCap, out var cap))
            {
                int uncappedCrit = GetCritChanceNotCapped(item);
                statCrit = Math.Clamp(uncappedCrit, int.MinValue, cap.Max) * cap.Sign;
            }
            else
            {
                statCrit = GetCritChanceCapped(item);
            }
            
            if (statCrit > 0)
            {
                crit += statCrit;
            }
        }
        
        public override float UseSpeedMultiplier(Item item, Player player)
        {
            var config = GetServerConfig();
            
            // Verifica se o incremento está habilitado
            if (!config.UseTimeIncrement)
                return 1f;
            
            // Verifica se o item está na blacklist
            if (isBlackListed(item, config.WeaponUseTime.BlackList))
                return 1f;
            
            // Verifica se o item tem useTime válido
            if (Level == 0 || item.useTime <= 0)
                return 1f;
            
            // Calcula com HardCap se aplicável, senão usa o valor capped normal
            float percentValue;
            if (isHardCapped(item, config.WeaponUseTime.HardCap, out var hardCap))
            {
                float notCappedPercent = GetUseTimeNotCappedPercent(item);
                percentValue = Math.Clamp(notCappedPercent, int.MinValue, hardCap.Max * 0.01f) * hardCap.Sign;
            }
            else
            {
                percentValue = GetUseTimeCappedPercent(item);
            }
            
            // Fórmula da referência: 1f - percentValue
            return 1f - percentValue;
        }
        
        public override float UseAnimationMultiplier(Item item, Player player)
        {
            var config = GetServerConfig();
            
            // Verifica se o incremento está habilitado
            if (!config.UseAnimationIncrement)
                return 1f;
            
            // Verifica se o item está na blacklist
            if (isBlackListed(item, config.WeaponUseAnimation.BlackList))
                return 1f;
            
            // Verifica se o item tem useAnimation válido
            if (Level == 0 || item.useAnimation <= 0)
                return 1f;
            
            // Calcula com HardCap se aplicável, senão usa o valor capped normal
            float percentValue;
            if (isHardCapped(item, config.WeaponUseAnimation.HardCap, out var hardCap))
            {
                float notCappedPercent = GetUseAnimationNotCappedPercent(item);
                percentValue = Math.Clamp(notCappedPercent, int.MinValue, hardCap.Max * 0.01f) * hardCap.Sign;
            }
            else
            {
                percentValue = GetUseAnimationCappedPercent(item);
            }
            
            // Fórmula da referência: 1f - percentValue
            return 1f - percentValue;
        }
        
        public override void ModifyItemScale(Item item, Player player, ref float scale)
        {
            var config = GetServerConfig();
            
            // Verifica se o item é noMelee (não tem melee hitbox)
            if (item.noMelee)
                return;
            
            // Verifica se o incremento está habilitado
            if (!config.MeleeWeaponSizeIncrement)
                return;
            
            // Verifica se o item está na blacklist
            if (isBlackListed(item, config.WeaponMeleeSize.BlackList))
                return;
            
            // Verifica se tem nível
            if (Level == 0)
                return;
            
            // Calcula com HardCap se aplicável, senão usa o valor capped normal
            float percentValue;
            if (isHardCapped(item, config.WeaponMeleeSize.HardCap, out var hardCap))
            {
                float notCappedPercent = GetMeleeSizeNotCappedPercent(item);
                percentValue = Math.Clamp(notCappedPercent, int.MinValue, hardCap.Max * 0.01f) * hardCap.Sign;
            }
            else
            {
                percentValue = GetMeleeSizeCappedPercent(item);
            }
            
            scale += percentValue;
        }
        
        public override void HoldItem(Item item, Player player)
        {
            // Hook mantido mas vazio - lógica movida para ModifyItemScale
        }
        
        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player)
        {
            var config = GetServerConfig();
            
            // Verifica se o incremento está habilitado
            if (!config.NotUseAmmoChanceIncrement)
                return true;
            
            // Verifica se o item está na blacklist
            if (isBlackListed(weapon, config.WeaponAmmoConsumptionReduction.BlackList))
                return true;
            
            // Verifica se tem nível
            if (Level == 0)
                return true;
            
            // Calcula a chance de não consumir com HardCap se aplicável
            float chance;
            if (isHardCapped(weapon, config.WeaponAmmoConsumptionReduction.HardCap, out var cap))
            {
                float notCapped = GetAmmoConsumptionReductionNotCapped(weapon);
                chance = Math.Clamp(notCapped, int.MinValue, cap.Max) * cap.Sign;
            }
            else
            {
                chance = GetAmmoConsumptionReductionCapped(weapon);
            }
            
            // Checa se não consome
            if (chance > Main.rand.Next(100))
                return false;
            
            return true;
        }
        
        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            var config = GetServerConfig();
            
            // Verifica se o incremento está habilitado
            if (!config.ManaCostReductionIncrement)
                return;
            
            // Verifica se o item está na blacklist
            if (isBlackListed(item, config.WeaponManaCostReduction.BlackList))
                return;
            
            // Verifica se tem nível e o item usa mana
            if (Level == 0 || item.mana <= 0)
                return;
            
            // Calcula o modificador com HardCap se aplicável
            float manaModifier;
            if (isHardCapped(item, config.WeaponManaCostReduction.HardCap, out var cap))
            {
                float notCappedPercent = GetManaCostReductionNotCappedPercent(item);
                manaModifier = Math.Clamp(notCappedPercent, int.MinValue, cap.Max * 0.01f) * cap.Sign;
            }
            else
            {
                manaModifier = GetManaCostReductionCappedPercent(item);
            }
            
            // Aplica o modificador
            if (manaModifier >= 0)
                reduce -= manaModifier;
            else
                mult -= manaModifier;
        }
        
        // UpdateAccessory removido - acessórios não ganham XP nem stats
        
        public override void UpdateEquip(Item item, Player player)
        {
            var config = GetServerConfig();
            
            // Verifica se o incremento está habilitado
            if (!config.DefenceIncrement)
                return;
            
            // Verifica se o item está na blacklist
            if (isBlackListed(item, config.ArmorDefense.ItemBlackList))
                return;
            
            // Verifica se tem nível e defesa
            if (Level == 0 || item.defense <= 0)
                return;
            
            // Calcula a defesa com HardCap se existir
            int statDefense;
            if (isHardCapped(item, config.ArmorDefense.HardCap, out var cap))
            {
                int uncappedDefense = GetDefenceNotCapped(item);
                statDefense = Math.Clamp(uncappedDefense, int.MinValue, cap.Max) * cap.Sign;
            }
            else
            {
                statDefense = GetDefenceCapped(item);
            }
            
            // Aplica a defesa usando OriginalDefense do Terraria
            item.defense = item.OriginalDefense + statDefense;
        }
        
        // ==================== TOOLTIPS ====================
        
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            // Verifica se o item pode ter XP
            if (!CanGainExperience(item)) return;
            
            var clientConfig = GetClientConfig();
            var serverConfig = GetServerConfig();
            
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
            
            // Mostra stats (apenas os relevantes ao item e que estão habilitados)
            if (clientConfig.ShowItemStats && Level > 0)
            {
                bool isWeapon = item.damage > 0;
                bool isArmor = item.defense > 0;
                
                // WEAPON STATS - só se for arma
                if (isWeapon)
                {
                    bool isMelee = item.CountsAsClass(DamageClass.Melee);
                    bool isRanged = item.CountsAsClass(DamageClass.Ranged);
                    bool isMagic = item.CountsAsClass(DamageClass.Magic);
                    bool isSummoner = item.CountsAsClass(DamageClass.Summon);
                    bool hasMana = item.mana > 0;
                    bool hasAmmo = item.useAmmo > 0;
                    bool hasProjectile = item.shoot > ProjectileID.None && item.shoot != ProjectileID.None;
                    
                    // Damage (com os 3 modos)
                    if (serverConfig.DamageIncrement)
                    {
                        int damageBonus = GetDamageCapped(item);
                        if (damageBonus > 0)
                        {
                            string damageText = "";
                            if (serverConfig.IncreaseBaseDamage) damageText += "Base ";
                            if (serverConfig.IncreaseFlatDamage) damageText += "Flat ";
                            if (serverConfig.IncreaseMultDamage) damageText += "Mult ";
                            
                            AddStatTooltip(tooltips, $"Damage ({damageText.Trim()})", damageBonus, clientConfig);
                        }
                    }
                    
                    // Crit Chance
                    if (serverConfig.CritIncrement)
                    {
                        int critBonus = GetCritChanceCapped(item);
                        if (critBonus > 0)
                            AddStatTooltip(tooltips, "Crit Chance", critBonus, clientConfig, "%");
                    }
                    
                    // Use Time
                    if (serverConfig.UseTimeIncrement)
                    {
                        float useTime = GetUseTimeCapped(item);
                        if (useTime > 0)
                            AddStatTooltip(tooltips, "Attack Speed (Use Time)", useTime, clientConfig, "%", false);
                    }
                    
                    // Use Animation
                    if (serverConfig.UseAnimationIncrement)
                    {
                        float useAnim = GetUseAnimationCapped(item);
                        if (useAnim > 0)
                            AddStatTooltip(tooltips, "Attack Speed (Animation)", useAnim, clientConfig, "%", false);
                    }
                    
                    // Melee Size - APENAS para melee (e não noMelee)
                    if (isMelee && !item.noMelee && serverConfig.MeleeWeaponSizeIncrement)
                    {
                        float meleeSize = GetMeleeSizeCapped(item);
                        if (meleeSize > 0)
                            AddStatTooltip(tooltips, "Melee Size", meleeSize, clientConfig, "%");
                    }
                    
                    // Mana Cost - APENAS para armas com custo de mana
                    if (hasMana && serverConfig.ManaCostReductionIncrement)
                    {
                        float manaCost = GetManaCostReductionCapped(item);
                        if (manaCost > 0)
                            AddStatTooltip(tooltips, "Mana Cost Reduction", manaCost, clientConfig, "%");
                    }
                    
                    // Ammo Consumption - APENAS para armas com munição
                    if (hasAmmo && serverConfig.NotUseAmmoChanceIncrement)
                    {
                        float ammoSave = GetAmmoConsumptionReductionCapped(item);
                        if (ammoSave > 0)
                            AddStatTooltip(tooltips, "Ammo Save Chance", ammoSave, clientConfig, "%");
                    }
                    
                    // PROJECTILE STATS - APENAS para armas que disparam projéteis
                    if (hasProjectile)
                    {
                        // Additional Projectiles
                        if (serverConfig.AdditionalProjectileChanceIncrement)
                        {
                            float additionalChance = GetAdditionalProjectileChanceCapped(item);
                            if (additionalChance > 0)
                            {
                                int guaranteed = (int)(additionalChance / 100);
                                int chanceExtra = (int)(additionalChance % 100);
                                
                                if (guaranteed > 0 && chanceExtra > 0)
                                    AddStatTooltip(tooltips, "Additional Projectiles", $"{guaranteed} + {chanceExtra}% chance", clientConfig);
                                else if (guaranteed > 0)
                                    AddStatTooltip(tooltips, "Additional Projectiles", $"{guaranteed}", clientConfig);
                                else
                                    AddStatTooltip(tooltips, "Additional Projectiles", additionalChance, clientConfig, "%");
                            }
                        }
                        
                        // Projectile Size
                        if (serverConfig.ProjectileSizeIncrement)
                        {
                            float projSize = GetProjectileSizeCapped(item);
                            if (projSize > 0)
                                AddStatTooltip(tooltips, "Projectile Size", projSize, clientConfig, "%");
                        }
                        
                        // Projectile Speed
                        if (serverConfig.ProjectileSpeedIncrement)
                        {
                            float projSpeed = GetProjectileSpeedCapped(item);
                            if (projSpeed > 0)
                                AddStatTooltip(tooltips, "Projectile Speed", projSpeed, clientConfig, "%");
                        }
                        
                        // Projectile Lifetime
                        if (serverConfig.ProjectileLifeTimeIncrement)
                        {
                            float lifetime = GetProjectileLifeTimeCapped(item);
                            if (lifetime > 0)
                                AddStatTooltip(tooltips, "Projectile Lifetime", lifetime, clientConfig, " ticks");
                        }
                        
                        // Projectile Penetration
                        if (serverConfig.ProjectilePenetrationIncrement)
                        {
                            float penetration = GetProjectilePenetrationCapped(item);
                            if (penetration > 0)
                                AddStatTooltip(tooltips, "Projectile Penetration", penetration, clientConfig);
                        }
                    }
                }
                
                // ARMOR STATS - só se for armadura
                if (isArmor && !isWeapon && serverConfig.DefenceIncrement)
                {
                    int defenseBonus = GetDefenceCapped(item);
                    if (defenseBonus > 0)
                        AddStatTooltip(tooltips, "Defense", defenseBonus, clientConfig);
                }
            }
        }
        
        private void AddStatTooltip(List<TooltipLine> tooltips, string statName, float value, ClientConfig config, string suffix = "", bool showSign = true)
        {
            if (value == 0) return;
            
            string sign = showSign && value > 0 ? "+" : "";
            string text = $"  {sign}{value:F0}{suffix} {statName}";
            
            tooltips.Add(new TooltipLine(Mod, $"Signature{statName.Replace(" ", "").Replace("(", "").Replace(")", "")}", text)
            {
                OverrideColor = config.TooltipStatColor
            });
        }
        
        private void AddStatTooltip(List<TooltipLine> tooltips, string statName, string customValue, ClientConfig config)
        {
            string text = $"  {customValue} {statName}";
            
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
            var serverConfig = GetServerConfig();
            
            // Debug
            if (serverConfig.DebugMode && Main.netMode != NetmodeID.Server)
            {
                Main.NewText($"[DEBUG] AddXP: {amount} (Total: {Experience}/{GetRequiredXP(Level)}) Level: {Level}");
            }
            
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
                
                // Debug
                if (serverConfig.DebugMode && Main.netMode != NetmodeID.Server)
                {
                    Main.NewText($"[DEBUG] LEVEL UP! New Level: {Level}");
                }
                
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
        /// Fórmula: (StartPrice + (level * AditionalPrice)) * (1 + (ExtraPrice * level)) * MultiPrice^(level-1)
        /// </summary>
        public int GetRequiredXP(int level)
        {
            var config = GetServerConfig();
            
            double startPrice = config.StartPrice;
            double aditionalPrice = config.AditionalPrice;
            double extraPrice = config.ExtraPrice * 0.01; // Converte percent para decimal
            double multiPrice = config.MultiPrice;
            
            // Preço base linear
            double basePrice = startPrice + (level * aditionalPrice);
            
            // Multiplicador percentual por level
            double percentMultiplier = 1.0 + (extraPrice * level);
            
            // Multiplicador exponencial
            double expMultiplier = Math.Pow(multiPrice, level - 1);
            
            return (int)(basePrice * percentMultiplier * expMultiplier);
        }
    }
}