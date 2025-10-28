using Terraria.ID;
using Terraria.ModLoader;
using SignatureEquipmentDeluxe.Common.Data;

namespace SignatureEquipmentDeluxe.Content.Items.Runes
{
    public class FireRune : BaseRuneItem
    {
        protected override RuneType RuneType => RuneType.Fire;
        protected override int Rarity => ItemRarityID.Orange;
        
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fire Rune");
            // Tooltip.SetDefault("Adds fire damage and burns enemies");
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Fireblossom, 3)
                .AddIngredient(ItemID.Obsidian, 10)
                .AddIngredient(ItemID.FallenStar, 2)
                .AddTile(TileID.Hellforge)
                .Register();
        }
    }
    
    public class IceRune : BaseRuneItem
    {
        protected override RuneType RuneType => RuneType.Ice;
        protected override int Rarity => ItemRarityID.Blue;
        
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ice Rune");
            // Tooltip.SetDefault("Adds ice damage and slows enemies");
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Shiverthorn, 3)
                .AddIngredient(ItemID.IceBlock, 50)
                .AddIngredient(ItemID.FallenStar, 2)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }
    
    public class PoisonRune : BaseRuneItem
    {
        protected override RuneType RuneType => RuneType.Poison;
        protected override int Rarity => ItemRarityID.Green;
        
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Poison Rune");
            // Tooltip.SetDefault("Adds poison damage over time");
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Deathweed, 3)
                .AddIngredient(ItemID.Stinger, 15)
                .AddIngredient(ItemID.FallenStar, 2)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
    
    public class LightningRune : BaseRuneItem
    {
        protected override RuneType RuneType => RuneType.Lightning;
        protected override int Rarity => ItemRarityID.Yellow;
        
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lightning Rune");
            // Tooltip.SetDefault("Adds electric damage with chain effect");
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Sunflower, 3)
                .AddIngredient(ItemID.Wire, 50)
                .AddIngredient(ItemID.FallenStar, 3)
                .AddTile(TileID.SkyMill)
                .Register();
        }
    }
}
