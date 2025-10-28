using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using SignatureEquipmentDeluxe.Common.Data;

namespace SignatureEquipmentDeluxe.Content.Items.Runes
{
    /// <summary>
    /// Maldições têm visual diferenciado e avisos ao aplicar
    /// </summary>
    public abstract class BaseCurseItem : BaseRuneItem
    {
        public override bool CanUseItem(Player player)
        {
            if (!base.CanUseItem(player))
                return false;
            
            // Aviso extra para maldições
            Main.NewText("WARNING: Curses give powerful buffs but have severe penalties!", Color.Red);
            Main.NewText("You will have a chance to drop this weapon on death!", Color.Orange);
            
            return true;
        }
        
        public override bool? UseItem(Player player)
        {
            var result = base.UseItem(player);
            
            if (result == true)
            {
                // Efeito dramático extra para maldições
                SpawnCurseParticles(player.Center);
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, player.Center);
            }
            
            return result;
        }
        
        private void SpawnCurseParticles(Vector2 position)
        {
            for (int i = 0; i < 50; i++)
            {
                float angle = MathHelper.TwoPi * i / 50f;
                Vector2 velocity = new Vector2(
                    (float)System.Math.Cos(angle) * 5f,
                    (float)System.Math.Sin(angle) * 5f
                );
                
                Dust dust = Dust.NewDustPerfect(position, DustID.Shadowflame, velocity, 0,
                    RuneDefinitions.GetColor(RuneType), 2f);
                dust.noGravity = true;
            }
        }
    }
    
    public class BerserkerCurse : BaseCurseItem
    {
        protected override RuneType RuneType => RuneType.CurseBerserker;
        protected override int Rarity => ItemRarityID.Expert;
        
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Curse of Berserker");
        }
    }
    
    public class GlassCurse : BaseCurseItem
    {
        protected override RuneType RuneType => RuneType.CurseGlass;
        protected override int Rarity => ItemRarityID.Expert;
        
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Curse of Glass Cannon");
        }
    }
    
    public class AnnihilationCurse : BaseCurseItem
    {
        protected override RuneType RuneType => RuneType.CurseAnnihilation;
        protected override int Rarity => ItemRarityID.Master;
        
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Curse of Annihilation");
        }
        
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.buyPrice(0, 10, 0, 0); // Mais cara
        }
    }
}
