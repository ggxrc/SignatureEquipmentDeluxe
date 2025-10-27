using Terraria;
using Terraria.ID;
using Terraria.GameContent.Events;
using System.Collections.Generic;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// Sistema de detecção de eventos ativos do jogo
    /// </summary>
    public static class EventDetector
    {
        /// <summary>
        /// Verifica se um evento específico está ativo no momento
        /// </summary>
        public static bool IsEventActive(GameEventType eventType)
        {
            return eventType switch
            {
                // ===== BOSS EVENTS =====
                GameEventType.KingSlime => NPC.AnyNPCs(NPCID.KingSlime),
                GameEventType.EyeOfCthulhu => NPC.AnyNPCs(NPCID.EyeofCthulhu),
                GameEventType.EaterOfWorlds => NPC.AnyNPCs(NPCID.EaterofWorldsHead),
                GameEventType.BrainOfCthulhu => NPC.AnyNPCs(NPCID.BrainofCthulhu),
                GameEventType.QueenBee => NPC.AnyNPCs(NPCID.QueenBee),
                GameEventType.Skeletron => NPC.AnyNPCs(NPCID.SkeletronHead),
                GameEventType.Deerclops => NPC.AnyNPCs(NPCID.Deerclops),
                GameEventType.WallOfFlesh => NPC.AnyNPCs(NPCID.WallofFlesh),
                GameEventType.QueenSlime => NPC.AnyNPCs(NPCID.QueenSlimeBoss),
                GameEventType.TheTwins => NPC.AnyNPCs(NPCID.Retinazer) || NPC.AnyNPCs(NPCID.Spazmatism),
                GameEventType.TheDestroyer => NPC.AnyNPCs(NPCID.TheDestroyer),
                GameEventType.SkeletronPrime => NPC.AnyNPCs(NPCID.SkeletronPrime),
                GameEventType.Plantera => NPC.AnyNPCs(NPCID.Plantera),
                GameEventType.Golem => NPC.AnyNPCs(NPCID.Golem),
                GameEventType.EmpressOfLight => NPC.AnyNPCs(NPCID.HallowBoss),
                GameEventType.DukeFishron => NPC.AnyNPCs(NPCID.DukeFishron),
                GameEventType.LunaticCultist => NPC.AnyNPCs(NPCID.CultistBoss),
                GameEventType.MoonLord => NPC.AnyNPCs(NPCID.MoonLordCore),
                
                // ===== MOON EVENTS =====
                GameEventType.BloodMoon => Main.bloodMoon,
                GameEventType.FullMoon => !Main.dayTime && Main.moonPhase == 0,
                GameEventType.NewMoon => !Main.dayTime && Main.moonPhase == 4,
                
                // ===== INVASION EVENTS =====
                GameEventType.GoblinArmy => Main.invasionType == InvasionID.GoblinArmy,
                GameEventType.FrostLegion => Main.invasionType == InvasionID.SnowLegion,
                GameEventType.PirateInvasion => Main.invasionType == InvasionID.PirateInvasion,
                GameEventType.MartianMadness => Main.invasionType == InvasionID.MartianMadness,
                GameEventType.PumpkinMoon => Main.pumpkinMoon,
                GameEventType.FrostMoon => Main.snowMoon,
                GameEventType.SolarEclipse => Main.eclipse,
                GameEventType.LunarEvent => NPC.LunarApocalypseIsUp,
                
                // ===== TIME EVENTS =====
                GameEventType.Day => Main.dayTime,
                GameEventType.Night => !Main.dayTime,
                
                // ===== WEATHER EVENTS =====
                GameEventType.Rain => Main.raining,
                GameEventType.Sandstorm => Sandstorm.Happening,
                GameEventType.Blizzard => Main.raining && Main.IsItAHappyWindyDay && Main.cloudAlpha > 0f,
                
                // ===== SPECIAL EVENTS =====
                GameEventType.PartyEvent => BirthdayParty.PartyIsUp,
                GameEventType.LanternNight => LanternNight.LanternsUp,
                
                _ => false
            };
        }
        
        /// <summary>
        /// Obtém todos os eventos ativos no momento
        /// </summary>
        public static List<GameEventType> GetActiveEvents()
        {
            List<GameEventType> activeEvents = new List<GameEventType>();
            
            // Percorre todos os tipos de evento e verifica quais estão ativos
            foreach (GameEventType eventType in System.Enum.GetValues(typeof(GameEventType)))
            {
                if (IsEventActive(eventType))
                {
                    activeEvents.Add(eventType);
                }
            }
            
            return activeEvents;
        }
    }
}
