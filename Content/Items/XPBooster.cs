using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using SignatureEquipmentDeluxe.Common.Players;

namespace SignatureEquipmentDeluxe.Content.Items
{
    /// <summary>
    /// Item especial que aumenta o multiplicador de XP permanentemente
    /// </summary>
    public class XPBooster : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Permite que o item funcione sem textura customizada
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.UseSound = SoundID.Item4;
            Item.consumable = true;
            Item.maxStack = 99;
        }

        public override bool? UseItem(Player player)
        {
            SignaturePlayer sigPlayer = player.GetModPlayer<SignaturePlayer>();
            if (sigPlayer != null)
            {
                sigPlayer.IncreaseXPMultiplier(0.1f); // +10% XP permanente
                Main.NewText($"Multiplicador de XP aumentado para {sigPlayer.xpMultiplier:P0}!", Color.Gold);
                return true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofLight, 5)
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddIngredient(ItemID.CrystalShard, 10)
                .AddIngredient(ItemID.PixieDust, 20)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
