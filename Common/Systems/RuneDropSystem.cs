using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// Sistema de drop de runas e maldições de bosses
    /// </summary>
    public class RuneDropSystem : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            // Maldições só dropam de bosses
            if (!npc.boss)
                return;
            
            // Berserker Curse - Bosses early game
            if (npc.type == NPCID.EyeofCthulhu || 
                npc.type == NPCID.KingSlime ||
                npc.type == NPCID.EaterofWorldsHead ||
                npc.type == NPCID.BrainofCthulhu)
            {
                npcLoot.Add(ItemDropRule.Common(
                    ModContent.ItemType<Content.Items.Runes.BerserkerCurse>(), 
                    10)); // 10% chance
            }
            
            // Glass Curse - Bosses mid game
            if (npc.type == NPCID.SkeletronHead ||
                npc.type == NPCID.QueenBee ||
                npc.type == NPCID.WallofFlesh)
            {
                npcLoot.Add(ItemDropRule.Common(
                    ModContent.ItemType<Content.Items.Runes.GlassCurse>(), 
                    8)); // 12.5% chance
            }
            
            // Annihilation Curse - Bosses hardmode/late game
            if (npc.type == NPCID.TheDestroyer ||
                npc.type == NPCID.SkeletronPrime ||
                npc.type == NPCID.Retinazer ||
                npc.type == NPCID.Spazmatism ||
                npc.type == NPCID.Plantera ||
                npc.type == NPCID.Golem ||
                npc.type == NPCID.DukeFishron ||
                npc.type == NPCID.CultistBoss ||
                npc.type == NPCID.MoonLordCore)
            {
                npcLoot.Add(ItemDropRule.Common(
                    ModContent.ItemType<Content.Items.Runes.AnnihilationCurse>(), 
                    5)); // 20% chance
            }
            
            // Runas normais podem dropar de mini-bosses e alguns inimigos especiais
            if (npc.type == NPCID.DungeonGuardian ||
                npc.type == NPCID.DD2DarkMageT1 ||
                npc.type == NPCID.DD2DarkMageT3 ||
                npc.type == NPCID.DD2OgreT2 ||
                npc.type == NPCID.DD2OgreT3)
            {
                // Drop aleatório de runa elemental ou utilitária
                npcLoot.Add(ItemDropRule.OneFromOptions(3, // 33% chance
                    ModContent.ItemType<Content.Items.Runes.FireRune>(),
                    ModContent.ItemType<Content.Items.Runes.IceRune>(),
                    ModContent.ItemType<Content.Items.Runes.PoisonRune>(),
                    ModContent.ItemType<Content.Items.Runes.LightningRune>(),
                    ModContent.ItemType<Content.Items.Runes.AttackSpeedRune>(),
                    ModContent.ItemType<Content.Items.Runes.LifeRegenRune>(),
                    ModContent.ItemType<Content.Items.Runes.LifestealRune>()
                ));
            }
        }
    }
}
