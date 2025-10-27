using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SignatureEquipmentDeluxe.Common.Players
{
    /// <summary>
    /// Armazena dados do jogador relacionados a equipamentos assinados
    /// </summary>
    public class SignaturePlayer : ModPlayer
    {
        // Dicionário que armazena: [ItemType] -> SignatureData
        public Dictionary<int, SignatureData> signedEquipment = new Dictionary<int, SignatureData>();
        
        // Item atualmente vinculado ativo
        public int currentSignatureItem = -1;
        
        // Cooldown para trocar de item assinado
        public int signatureSwitchCooldown = 0;
        
        // Multiplicador de XP global
        public float xpMultiplier = 1f;

        public override void Initialize()
        {
            signedEquipment.Clear();
            currentSignatureItem = -1;
            signatureSwitchCooldown = 0;
            xpMultiplier = 1f;
        }

        /// <summary>
        /// Aumenta o multiplicador de XP
        /// </summary>
        public void IncreaseXPMultiplier(float amount)
        {
            xpMultiplier += amount;
        }

        public override void ResetEffects()
        {
            if (signatureSwitchCooldown > 0)
                signatureSwitchCooldown--;
        }

        /// <summary>
        /// Quando acerta um NPC com item, ganha XP
        /// </summary>
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            var config = ModContent.GetInstance<Configs.ServerConfig>();
            
            // Verifica se pode ganhar XP de estátuas
            if (!config.AllowStatueXP && target.SpawnedFromStatue)
                return;
            
            // Processa XP para o item segurado
            Item heldItem = Player.HeldItem;
            if (heldItem != null && !heldItem.IsAir)
            {
                var globalItem = heldItem.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
                if (globalItem != null && globalItem.CanGainExperience(heldItem))
                {
                    // XP por hit
                    DistributeHitXP(heldItem, globalItem, target, damageDone);
                    
                    // XP adicional e kill count se matou o NPC
                    if (target.life <= 0)
                    {
                        DistributeKillXP(heldItem, globalItem, target);
                    }
                }
            }
        }
        
        /// <summary>
        /// Quando o jogador sofre dano, armaduras ganham XP
        /// </summary>
        public override void OnHurt(Player.HurtInfo info)
        {
            var config = ModContent.GetInstance<Configs.ServerConfig>();
            
            // Calcula dano real recebido
            int damageReceived = info.Damage;
            
            // Se IgnoreDefense estiver ativado, usa o dano antes da defesa
            if (config.ArmorXPIgnoreDefense)
            {
                // Tenta calcular o dano original antes da redução de defesa
                // Damage = OriginalDamage - Defense/2
                damageReceived = info.Damage + (Player.statDefense / 2);
            }
            
            // Distribui XP para armaduras equipadas
            DistributeArmorXP(Player.armor[0], damageReceived, config); // Helm
            DistributeArmorXP(Player.armor[1], damageReceived, config); // Chestplate
            DistributeArmorXP(Player.armor[2], damageReceived, config); // Leggings
        }
        
        /// <summary>
        /// Distribui XP para uma peça de armadura
        /// </summary>
        private void DistributeArmorXP(Item armorPiece, int damageReceived, Configs.ServerConfig config)
        {
            if (armorPiece == null || armorPiece.IsAir || armorPiece.defense <= 0)
                return;
            
            var globalItem = armorPiece.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
            if (globalItem == null || !globalItem.CanGainExperience(armorPiece))
                return;
            
            // Fórmula de XP baseada no dano recebido
            float xpGain = damageReceived * config.ArmorXPPerDamageReceived;
            
            // Multiplicadores
            xpGain *= config.GlobalExpMultiplier;
            xpGain *= config.ArmorExpMultiplier;
            xpGain *= xpMultiplier;
            
            if (xpGain > 0)
            {
                globalItem.AddExperience((int)xpGain, armorPiece);
            }
        }

        /// <summary>
        /// Distribui XP por acertar um NPC
        /// </summary>
        private void DistributeHitXP(Item item, GlobalItems.SignatureGlobalItem globalItem, NPC target, int damageDone)
        {
            var config = ModContent.GetInstance<Configs.ServerConfig>();
            
            // Fórmula base de XP por hit
            float xpGain = config.WeaponBaseXPPerHit;
            
            // XP baseado no dano causado
            xpGain += damageDone * config.WeaponXPPerDamageDealt;
            
            // Multiplicadores
            xpGain *= config.GlobalExpMultiplier;
            xpGain *= config.WeaponExpMultiplier;
            xpGain *= xpMultiplier; // Multiplicador do jogador
            
            // Boss multiplier
            if (target.boss)
            {
                xpGain *= config.BossExpMultiplier;
            }
            
            globalItem.AddExperience((int)xpGain, item);
        }

        /// <summary>
        /// Distribui XP por matar um NPC
        /// </summary>
        private void DistributeKillXP(Item item, GlobalItems.SignatureGlobalItem globalItem, NPC target)
        {
            var config = ModContent.GetInstance<Configs.ServerConfig>();
            
            // XP base de kill
            float xpGain = config.WeaponBaseXPPerKill;
            
            // XP baseado no HP máximo do inimigo
            xpGain += target.lifeMax * config.WeaponXPPerEnemyMaxHP;
            
            // Multiplicadores
            xpGain *= config.GlobalExpMultiplier;
            xpGain *= config.WeaponExpMultiplier;
            xpGain *= xpMultiplier;
            
            // Boss multiplier
            if (target.boss)
            {
                xpGain *= config.BossExpMultiplier;
            }
            
            globalItem.AddExperience((int)xpGain, item);
            
            // Adiciona kill count
            if (signedEquipment.TryGetValue(item.type, out SignatureData data))
            {
                data.totalKills++;
                data.totalDamageDealt += target.lifeMax;
            }
        }

        /// <summary>
        /// Vincula um item como equipamento assinado
        /// </summary>
        public bool SignItem(Item item)
        {
            if (item == null || item.IsAir)
                return false;

            int itemType = item.type;
            
            if (!signedEquipment.ContainsKey(itemType))
            {
                signedEquipment[itemType] = new SignatureData
                {
                    itemType = itemType,
                    experience = 0,
                    level = 1,
                    totalKills = 0,
                    totalDamageDealt = 0,
                    timeUsed = 0,
                    prestigeLevel = 0
                };
                
                Main.NewText($"[c/FFD700:{item.Name}] foi vinculado como seu equipamento assinado!", Color.Gold);
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Remove a vinculação de um item
        /// </summary>
        public bool UnsignItem(int itemType)
        {
            if (signedEquipment.ContainsKey(itemType))
            {
                signedEquipment.Remove(itemType);
                
                if (currentSignatureItem == itemType)
                    currentSignatureItem = -1;
                    
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Verifica se um item está vinculado
        /// </summary>
        public bool IsItemSigned(int itemType)
        {
            return signedEquipment.ContainsKey(itemType);
        }

        /// <summary>
        /// Obtém os dados de assinatura de um item
        /// </summary>
        public SignatureData GetSignatureData(int itemType)
        {
            if (signedEquipment.TryGetValue(itemType, out SignatureData data))
                return data;
            
            return null;
        }

        /// <summary>
        /// Adiciona experiência a um item assinado
        /// </summary>
        public void AddExperience(int itemType, float amount)
        {
            if (signedEquipment.TryGetValue(itemType, out SignatureData data))
            {
                data.experience += (int)(amount * xpMultiplier);
                
                // Verifica se subiu de nível
                CheckLevelUp(data);
            }
        }

        /// <summary>
        /// Verifica e processa level up
        /// </summary>
        private void CheckLevelUp(SignatureData data)
        {
            int requiredXP = GetRequiredXP(data.level);
            
            while (data.experience >= requiredXP && data.level < SignatureData.MAX_LEVEL)
            {
                data.experience -= requiredXP;
                data.level++;
                
                // Efeito visual de level up
                for (int i = 0; i < 30; i++)
                {
                    Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, 
                        DustID.GoldCoin, 0f, 0f, 100, Color.Gold, 2f);
                    dust.velocity *= 3f;
                    dust.noGravity = true;
                }
                
                Main.NewText($"Seu equipamento subiu para o nível {data.level}!", Color.Gold);
                requiredXP = GetRequiredXP(data.level);
            }
        }

        /// <summary>
        /// Calcula XP necessário para o próximo nível
        /// </summary>
        public int GetRequiredXP(int currentLevel)
        {
            // Fórmula: 100 * level^1.5
            return (int)(100 * System.Math.Pow(currentLevel, 1.5));
        }

        /// <summary>
        /// Salva os dados do jogador
        /// </summary>
        public override void SaveData(TagCompound tag)
        {
            var itemTypes = new List<int>();
            var experiences = new List<int>();
            var levels = new List<int>();
            var kills = new List<int>();
            var damages = new List<long>();
            var times = new List<int>();
            var prestiges = new List<int>();

            foreach (var kvp in signedEquipment)
            {
                itemTypes.Add(kvp.Key);
                experiences.Add(kvp.Value.experience);
                levels.Add(kvp.Value.level);
                kills.Add(kvp.Value.totalKills);
                damages.Add(kvp.Value.totalDamageDealt);
                times.Add(kvp.Value.timeUsed);
                prestiges.Add(kvp.Value.prestigeLevel);
            }

            tag["itemTypes"] = itemTypes;
            tag["experiences"] = experiences;
            tag["levels"] = levels;
            tag["kills"] = kills;
            tag["damages"] = damages;
            tag["times"] = times;
            tag["prestiges"] = prestiges;
            tag["currentSignature"] = currentSignatureItem;
        }

        /// <summary>
        /// Carrega os dados do jogador
        /// </summary>
        public override void LoadData(TagCompound tag)
        {
            signedEquipment.Clear();

            if (tag.ContainsKey("itemTypes"))
            {
                var itemTypes = tag.GetList<int>("itemTypes");
                var experiences = tag.GetList<int>("experiences");
                var levels = tag.GetList<int>("levels");
                var kills = tag.GetList<int>("kills");
                var damages = tag.GetList<long>("damages");
                var times = tag.GetList<int>("times");
                var prestiges = tag.GetList<int>("prestiges");

                for (int i = 0; i < itemTypes.Count; i++)
                {
                    signedEquipment[itemTypes[i]] = new SignatureData
                    {
                        itemType = itemTypes[i],
                        experience = experiences[i],
                        level = levels[i],
                        totalKills = kills[i],
                        totalDamageDealt = damages[i],
                        timeUsed = times[i],
                        prestigeLevel = prestiges[i]
                    };
                }
            }

            currentSignatureItem = tag.GetInt("currentSignature");
        }

        /// <summary>
        /// Sincroniza dados em multiplayer
        /// </summary>
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)SignatureEquipmentDeluxe.MessageType.SignaturePlayerSync);
            packet.Write((byte)Player.whoAmI);
            
            packet.Write(signedEquipment.Count);
            foreach (var kvp in signedEquipment)
            {
                packet.Write(kvp.Key);
                kvp.Value.WriteToPacket(packet);
            }
            
            packet.Write(currentSignatureItem);
            packet.Send(toWho, fromWho);
        }
    }

    /// <summary>
    /// Classe que armazena dados de um equipamento assinado
    /// </summary>
    public class SignatureData
    {
        public const int MAX_LEVEL = 100;
        public const int MAX_PRESTIGE = 10;

        public int itemType;
        public int experience;
        public int level;
        public int totalKills;
        public long totalDamageDealt;
        public int timeUsed; // Em ticks
        public int prestigeLevel;

        /// <summary>
        /// Calcula o multiplicador de dano total
        /// </summary>
        public float GetDamageMultiplier()
        {
            // Base: 1% por nível
            float levelBonus = 1f + (level * 0.01f);
            
            // Prestígio: 5% por nível de prestígio
            float prestigeBonus = 1f + (prestigeLevel * 0.05f);
            
            return levelBonus * prestigeBonus;
        }

        /// <summary>
        /// Calcula o bônus de crítico
        /// </summary>
        public int GetCritBonus()
        {
            // 1% a cada 5 níveis
            int levelCrit = level / 5;
            
            // 2% por prestígio
            int prestigeCrit = prestigeLevel * 2;
            
            return levelCrit + prestigeCrit;
        }

        /// <summary>
        /// Calcula o bônus de velocidade de uso
        /// </summary>
        public float GetUseSpeedMultiplier()
        {
            // Máximo de 20% mais rápido
            float bonus = System.Math.Min(0.2f, level * 0.002f + prestigeLevel * 0.02f);
            return 1f - bonus;
        }

        /// <summary>
        /// Verifica se pode fazer prestígio
        /// </summary>
        public bool CanPrestige()
        {
            return level >= MAX_LEVEL && prestigeLevel < MAX_PRESTIGE;
        }

        /// <summary>
        /// Realiza o prestígio
        /// </summary>
        public void Prestige()
        {
            if (CanPrestige())
            {
                prestigeLevel++;
                level = 1;
                experience = 0;
            }
        }

        /// <summary>
        /// Escreve dados no pacote de rede
        /// </summary>
        public void WriteToPacket(ModPacket packet)
        {
            packet.Write(experience);
            packet.Write(level);
            packet.Write(totalKills);
            packet.Write(totalDamageDealt);
            packet.Write(timeUsed);
            packet.Write(prestigeLevel);
        }

        /// <summary>
        /// Lê dados do pacote de rede
        /// </summary>
        public static SignatureData ReadFromPacket(BinaryReader reader, int itemType)
        {
            return new SignatureData
            {
                itemType = itemType,
                experience = reader.ReadInt32(),
                level = reader.ReadInt32(),
                totalKills = reader.ReadInt32(),
                totalDamageDealt = reader.ReadInt64(),
                timeUsed = reader.ReadInt32(),
                prestigeLevel = reader.ReadInt32()
            };
        }
    }
}
