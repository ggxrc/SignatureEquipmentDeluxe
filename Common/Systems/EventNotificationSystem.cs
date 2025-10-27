using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Chat;
using Terraria.Localization;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// Sistema de notificações extravagantes para eventos de XP
    /// </summary>
    public static class EventNotificationSystem
    {
        // Cores extravagantes para notificações
        private static readonly Color ColorGold = new Color(255, 215, 0);
        private static readonly Color ColorOrange = new Color(255, 140, 0);
        private static readonly Color ColorPurple = new Color(200, 100, 255);
        private static readonly Color ColorGreen = new Color(100, 255, 100);
        private static readonly Color ColorRed = new Color(255, 100, 100);
        private static readonly Color ColorCyan = new Color(100, 255, 255);
        
        /// <summary>
        /// Notifica início de um evento com bônus de XP
        /// </summary>
        public static void NotifyEventStarted(GameEventType eventType, float baseMultiplier, float finalMultiplier, int penaltyPercent)
        {
            string eventName = GetEventDisplayName(eventType);
            int bonusPercent = (int)((finalMultiplier - 1f) * 100f);
            
            string message;
            Color color;
            
            if (penaltyPercent < 100)
            {
                // Evento com penalidade - mostra redução do bônus
                int penaltyReduction = 100 - penaltyPercent; // Ex: penalty 50% = -50% do bônus
                color = ColorOrange;
                message = $"⚡ {eventName} STARTED! ⚡\n" +
                         $"  Base Bonus: +{(int)((baseMultiplier - 1f) * 100f)}% XP\n" +
                         $"  Anti-Farm Penalty: -{penaltyReduction}%\n" +
                         $"  ACTUAL BONUS: +{bonusPercent}% XP";
            }
            else
            {
                // Evento sem penalidade
                color = ColorGold;
                message = $"⭐ {eventName} STARTED! ⭐\n" +
                         $"  XP BONUS: +{bonusPercent}% XP";
            }
            
            BroadcastMessage(message, color);
        }
        
        /// <summary>
        /// Notifica quando múltiplos eventos estão ativos (stack)
        /// </summary>
        public static void NotifyEventsStacked(List<GameEventType> activeEvents, float totalMultiplier)
        {
            if (activeEvents.Count <= 1) return;
            
            int totalBonus = (int)((totalMultiplier - 1f) * 100f);
            
            string eventsList = string.Join(" + ", activeEvents.Select(e => GetEventDisplayName(e)));
            
            string message = $"🔥 MULTIPLE EVENTS ACTIVE! 🔥\n" +
                           $"  {eventsList}\n" +
                           $"  COMBINED BONUS: +{totalBonus}% XP";
            
            BroadcastMessage(message, ColorPurple);
        }
        
        /// <summary>
        /// Notifica quando um evento termina
        /// </summary>
        public static void NotifyEventEnded(GameEventType eventType, float newTotalMultiplier)
        {
            string eventName = GetEventDisplayName(eventType);
            int newBonus = (int)((newTotalMultiplier - 1f) * 100f);
            
            string message;
            if (newTotalMultiplier > 1f)
            {
                message = $"⏹️ {eventName} ended.\n" +
                         $"  Current XP Bonus: +{newBonus}%";
            }
            else
            {
                message = $"⏹️ {eventName} ended.\n" +
                         $"  XP Bonus: NONE (back to normal)";
            }
            
            BroadcastMessage(message, ColorCyan);
        }
        
        /// <summary>
        /// Notifica quando a penalidade de um evento é resetada
        /// </summary>
        public static void NotifyPenaltyReset(GameEventType eventType)
        {
            string eventName = GetEventDisplayName(eventType);
            
            string message = $"✨ PENALTY RESET! ✨\n" +
                           $"  {eventName} is back to full XP bonus!\n" +
                           $"  (3 different events ended)";
            
            BroadcastMessage(message, ColorGreen);
        }
        
        /// <summary>
        /// Notifica quando múltiplos bosses são invocados simultaneamente
        /// </summary>
        public static void NotifyMultiBossPenalty(List<GameEventType> activeBosses)
        {
            if (activeBosses.Count < 2) return;
            
            string bossesList = string.Join(" + ", activeBosses.Select(e => GetEventDisplayName(e)));
            
            string message = $"⚠️ MULTI-BOSS PENALTY! ⚠️\n" +
                           $"  Multiple bosses detected: {bossesList}\n" +
                           $"  All boss XP reduced by 50% to prevent abuse!";
            
            BroadcastMessage(message, ColorRed);
        }
        
        /// <summary>
        /// Envia mensagem para todos os jogadores
        /// </summary>
        private static void BroadcastMessage(string text, Color color)
        {
            if (Main.netMode == Terraria.ID.NetmodeID.SinglePlayer)
            {
                Main.NewText(text, color);
            }
            else if (Main.netMode == Terraria.ID.NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(text), color);
            }
        }
        
        /// <summary>
        /// Retorna o nome amigável do evento
        /// </summary>
        private static string GetEventDisplayName(GameEventType eventType)
        {
            return eventType switch
            {
                // Bosses Pre-Hardmode
                GameEventType.KingSlime => "King Slime",
                GameEventType.EyeOfCthulhu => "Eye of Cthulhu",
                GameEventType.EaterOfWorlds => "Eater of Worlds",
                GameEventType.BrainOfCthulhu => "Brain of Cthulhu",
                GameEventType.QueenBee => "Queen Bee",
                GameEventType.Skeletron => "Skeletron",
                GameEventType.Deerclops => "Deerclops",
                GameEventType.WallOfFlesh => "Wall of Flesh",
                
                // Bosses Hardmode
                GameEventType.QueenSlime => "Queen Slime",
                GameEventType.TheTwins => "The Twins",
                GameEventType.TheDestroyer => "The Destroyer",
                GameEventType.SkeletronPrime => "Skeletron Prime",
                GameEventType.Plantera => "Plantera",
                GameEventType.Golem => "Golem",
                GameEventType.EmpressOfLight => "Empress of Light",
                GameEventType.DukeFishron => "Duke Fishron",
                GameEventType.LunaticCultist => "Lunatic Cultist",
                GameEventType.MoonLord => "Moon Lord",
                
                // Moon Events
                GameEventType.BloodMoon => "Blood Moon",
                GameEventType.FullMoon => "Full Moon",
                GameEventType.NewMoon => "New Moon",
                
                // Invasions
                GameEventType.GoblinArmy => "Goblin Army",
                GameEventType.FrostLegion => "Frost Legion",
                GameEventType.PirateInvasion => "Pirate Invasion",
                GameEventType.MartianMadness => "Martian Madness",
                GameEventType.PumpkinMoon => "Pumpkin Moon",
                GameEventType.FrostMoon => "Frost Moon",
                GameEventType.SolarEclipse => "Solar Eclipse",
                GameEventType.LunarEvent => "Lunar Event",
                
                // Time
                GameEventType.Day => "Day Time",
                GameEventType.Night => "Night Time",
                
                // Weather
                GameEventType.Rain => "Rain",
                GameEventType.Sandstorm => "Sandstorm",
                GameEventType.Blizzard => "Blizzard",
                
                // Special
                GameEventType.PartyEvent => "Party",
                GameEventType.LanternNight => "Lantern Night",
                
                _ => eventType.ToString()
            };
        }
    }
}
