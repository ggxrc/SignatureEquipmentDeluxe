using Terraria.ID;
using Terraria.ModLoader;
using SignatureEquipmentDeluxe.Common.Data;

namespace SignatureEquipmentDeluxe.Content.Items.Runes
{
    public class AttackSpeedRune : BaseRuneItem
    {
        protected override RuneType RuneType => RuneType.AttackSpeed;
        protected override int Rarity => ItemRarityID.LightRed;
        
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Attack Speed Rune");
            // Tooltip.SetDefault("Increases attack speed");
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Blinkroot, 3)
                .AddIngredient(ItemID.Cobweb, 50)
                .AddIngredient(ItemID.FallenStar, 3)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
    
    public class LifeRegenRune : BaseRuneItem
    {
        protected override RuneType RuneType => RuneType.LifeRegen;
        protected override int Rarity => ItemRarityID.Pink;
        
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Life Regen Rune");
            // Tooltip.SetDefault("Regenerates health over time");
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Daybloom, 3)
                .AddIngredient(ItemID.LifeCrystal)
                .AddIngredient(ItemID.FallenStar, 2)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
    
    public class LifestealRune : BaseRuneItem
    {
        protected override RuneType RuneType => RuneType.Lifesteal;
        protected override int Rarity => ItemRarityID.Red;
        
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lifesteal Rune");
            // Tooltip.SetDefault("Steals life from enemies");
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Vertebrae, 10)
                .AddIngredient(ItemID.RottenChunk, 10)
                .AddIngredient(ItemID.FallenStar, 5)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
