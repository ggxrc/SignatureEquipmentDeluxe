using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SignatureEquipmentDeluxe.Common.GlobalItems;
using SignatureEquipmentDeluxe.Common.Systems;

namespace SignatureEquipmentDeluxe.Content.Items.Runes
{
    /// <summary>
    /// Item para remover runas de armas
    /// </summary>
    public class RuneRemover : ModItem
    {
        // Sistema de seleção
        private static bool isSelected = false;
        
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 99;
            Item.value = Item.buyPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.consumable = true;
            Item.UseSound = SoundID.Item3;
        }
        
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rune Remover");
            // Tooltip.SetDefault("Removes all runes from a weapon\nCurses have 20% chance to cost 1/8 of weapon level");
        }
        
        public override bool CanUseItem(Player player)
        {
            // Se ainda não está selecionado, seleciona
            if (!isSelected)
            {
                isSelected = true;
                Main.NewText("Rune Remover selected! Click on a weapon in your inventory to remove runes.", Color.Cyan);
                return false; // Não consome
            }
            
            return true;
        }
        
        public override bool? UseItem(Player player)
        {
            // Nunca deve chegar aqui na seleção inicial
            return false;
        }
        
        /// <summary>
        /// Tenta remover runas do item clicado
        /// </summary>
        public static bool TryRemoveRunes(Player player, Item targetItem)
        {
            if (targetItem == null || targetItem.IsAir)
                return false;
            
            var sigItem = targetItem.GetGlobalItem<SignatureGlobalItem>();
            if (sigItem.EquippedRunes.Count == 0)
            {
                Main.NewText("This weapon has no runes to remove!", Color.Yellow);
                return false;
            }
            
            int runesRemoved = 0;
            int cursesRemoved = 0;
            bool lostLevels = false;
            int totalLevelsLost = 0;
            
            // Remove todas as runas, verificando maldições
            for (int i = sigItem.EquippedRunes.Count - 1; i >= 0; i--)
            {
                var rune = sigItem.EquippedRunes[i];
                
                if (rune.IsCurse())
                {
                    bool lost;
                    int levelsLost;
                    CurseRemovalSystem.RemoveCurse(targetItem, i, out lost, out levelsLost);
                    
                    if (lost)
                    {
                        lostLevels = true;
                        totalLevelsLost += levelsLost;
                    }
                    
                    cursesRemoved++;
                }
                else
                {
                    sigItem.EquippedRunes.RemoveAt(i);
                    runesRemoved++;
                }
            }
            
            // Efeito visual
            SpawnRemovalParticles(targetItem.Center);
            
            // Mensagens
            if (runesRemoved > 0)
                Main.NewText($"Removed {runesRemoved} rune(s)", Color.Cyan);
            
            if (cursesRemoved > 0)
                Main.NewText($"Removed {cursesRemoved} curse(s)", Color.Orange);
            
            if (lostLevels)
                Main.NewText($"Lost {totalLevelsLost} level(s) from curse removal!", Color.Red);
            
            // Consome o item remover
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].type == ModContent.ItemType<RuneRemover>())
                {
                    player.inventory[i].stack--;
                    if (player.inventory[i].stack <= 0)
                        player.inventory[i].TurnToAir();
                    break;
                }
            }
            
            // Reseta a seleção
            isSelected = false;
            
            return true;
        }
        
        /// <summary>
        /// Cancela a seleção
        /// </summary>
        public static void CancelSelection()
        {
            if (isSelected)
            {
                Main.NewText("Rune removal cancelled.", Color.Gray);
                isSelected = false;
            }
        }
        
        /// <summary>
        /// Retorna se o remover está selecionado
        /// </summary>
        public static bool IsSelected()
        {
            return isSelected;
        }
        
        private static void SpawnRemovalParticles(Vector2 position)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(4f, 4f);
                Dust dust = Dust.NewDustPerfect(position, DustID.Smoke, velocity, 100, 
                    Color.Gray, 1.5f);
                dust.noGravity = true;
            }
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.MagicMirror)
                .AddIngredient(ItemID.SoulofLight, 5)
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
