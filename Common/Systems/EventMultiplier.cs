using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Terraria.ModLoader.Config;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// Configuração de multiplicador de XP para um evento específico
    /// </summary>
    public class EventMultiplier
    {
        [DefaultValue(GameEventType.KingSlime)]
        public GameEventType EventType { get; set; }
        
        [JsonIgnore]
        public EventCategory Category { get; set; }
        
        [DefaultValue(true)]
        public bool Enabled { get; set; } = true;
        
        [DefaultValue(XPMultiplierTier.Medium)]
        public XPMultiplierTier Tier { get; set; } = XPMultiplierTier.Medium;
        
        public override bool Equals(object obj)
        {
            if (obj is EventMultiplier other)
                return EventType == other.EventType && Enabled == other.Enabled && Tier == other.Tier;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return new { EventType, Enabled, Tier }.GetHashCode();
        }
        
        /// <summary>
        /// Retorna o multiplicador base configurado (sem penalidades de farm)
        /// </summary>
        public float GetBaseMultiplier()
        {
            if (!Enabled) return 1f;
            return Tier.ToMultiplier();
        }
        
        public override string ToString()
        {
            return $"{EventType}: {(Enabled ? $"{Tier} (+{Tier.ToPercentage()}% XP)" : "Disabled")}";
        }
    }
}
