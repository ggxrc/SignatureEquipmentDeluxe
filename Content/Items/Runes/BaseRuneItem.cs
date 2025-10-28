using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using SignatureEquipmentDeluxe.Common.Data;
using SignatureEquipmentDeluxe.Common.Systems;
using SignatureEquipmentDeluxe.Common.GlobalItems;
using SignatureEquipmentDeluxe.Common.Configs;

namespace SignatureEquipmentDeluxe.Content.Items.Runes
{
    /// <summary>
    /// Classe base para todos os itens de runa
    /// </summary>
    public abstract class BaseRuneItem : ModItem
    {
        protected abstract RuneType RuneType { get; }
        protected abstract int Rarity { get; }
        
        // Sistema de seleção de arma
        private static int selectedRuneItemType = -1;
        private static RuneType selectedRuneType = RuneType.Fire;
        
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 99;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = Rarity;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.consumable = true;
            Item.UseSound = SoundID.Item29;
        }
        
        public override bool CanUseItem(Player player)
        {
            // Se nenhuma runa está selecionada, seleciona esta
            if (selectedRuneItemType != Item.type)
            {
                selectedRuneItemType = Item.type;
                selectedRuneType = RuneType;
                
                string runeName = RuneDefinitions.GetName(RuneType);
                Color runeColor = RuneDefinitions.GetColor(RuneType);
                Main.NewText($"{runeName} selected! Click on a weapon in your inventory to apply.", runeColor);
                
                return false; // Não consome o item ainda
            }
            
            return true;
        }
        
        public override bool? UseItem(Player player)
        {
            // Nunca deve chegar aqui na seleção inicial
            return false;
        }
        
        /// <summary>
        /// Tenta aplicar a runa selecionada no item clicado
        /// </summary>
        public static bool TryApplyRune(Player player, Item targetItem, int runeItemType, RuneType runeType)
        {
            if (targetItem == null || targetItem.IsAir)
                return false;
            
            if (!RuneSystem.CanHaveRunes(targetItem))
            {
                Main.NewText("This item cannot have runes!", Color.Red);
                return false;
            }
            
            var sigItem = targetItem.GetGlobalItem<SignatureGlobalItem>();
            if (sigItem.Level <= 0)
            {
                Main.NewText("This weapon needs to be leveled before applying runes!", Color.Yellow);
                return false;
            }
            
            // Verifica se tem slots disponíveis
            int maxSlots = RuneSystem.GetMaxRuneSlots(sigItem.Level);
            if (sigItem.EquippedRunes.Count >= maxSlots)
            {
                Main.NewText($"This weapon has no available rune slots! ({sigItem.EquippedRunes.Count}/{maxSlots})", Color.Orange);
                return false;
            }
            
            // Verifica se já tem essa runa
            if (sigItem.EquippedRunes.Any(r => r.Type == runeType))
            {
                Main.NewText("This weapon already has this rune!", Color.Orange);
                return false;
            }
            
            // Aplica a runa
            var config = ModContent.GetInstance<ServerConfig>();
            int maxLevel = config.WeaponMaxLevel > 0 ? config.WeaponMaxLevel : 100;
            
            var newRune = new EquippedRune(runeType, maxLevel);
            sigItem.EquippedRunes.Add(newRune);
            
            // Efeito visual e sonoro
            SpawnSuccessParticles(targetItem.Center, runeType);
            
            string runeName = RuneDefinitions.GetName(runeType);
            Color runeColor = RuneDefinitions.GetColor(runeType);
            Main.NewText($"Applied {runeName} to {targetItem.Name}!", runeColor);
            
            // Consome o item de runa
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].type == runeItemType)
                {
                    player.inventory[i].stack--;
                    if (player.inventory[i].stack <= 0)
                        player.inventory[i].TurnToAir();
                    break;
                }
            }
            
            // Reseta a seleção
            selectedRuneItemType = -1;
            
            return true;
        }
        
        /// <summary>
        /// Cancela a seleção de runa
        /// </summary>
        public static void CancelSelection()
        {
            if (selectedRuneItemType != -1)
            {
                Main.NewText("Rune application cancelled.", Color.Gray);
                selectedRuneItemType = -1;
            }
        }
        
        /// <summary>
        /// Retorna se alguma runa está selecionada para aplicação
        /// </summary>
        public static bool HasSelectedRune(out int itemType, out RuneType runeType)
        {
            itemType = selectedRuneItemType;
            runeType = selectedRuneType;
            return selectedRuneItemType != -1;
        }
        
        private static void SpawnSuccessParticles(Vector2 position, RuneType runeType)
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(3f, 3f);
                Dust dust = Dust.NewDustPerfect(position, DustID.GoldCoin, velocity, 0, 
                    RuneDefinitions.GetColor(runeType), 1.5f);
                dust.noGravity = true;
            }
        }
        
        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            var descLine = new TooltipLine(Mod, "RuneDescription", RuneDefinitions.GetDescription(RuneType))
            {
                OverrideColor = RuneDefinitions.GetColor(RuneType)
            };
            tooltips.Add(descLine);
            
            var useLine = new TooltipLine(Mod, "RuneUsage", "Right-click to select, then click on a weapon in your inventory")
            {
                OverrideColor = Color.Gray
            };
            tooltips.Add(useLine);
            
            var cancelLine = new TooltipLine(Mod, "RuneCancel", "Press ESC to cancel selection")
            {
                OverrideColor = Color.DarkGray
            };
            tooltips.Add(cancelLine);
        }
    }
}
