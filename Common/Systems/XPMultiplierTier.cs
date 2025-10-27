namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// Níveis de multiplicador de XP para eventos (mais intuitivo que percentuais)
    /// </summary>
    public enum XPMultiplierTier
    {
        /// <summary>Sem bônus (1.0x = +0%)</summary>
        None = 0,
        
        /// <summary>Bônus baixo (1.15x = +15%)</summary>
        Low = 1,
        
        /// <summary>Bônus médio (1.35x = +35%)</summary>
        Medium = 2,
        
        /// <summary>Bônus alto (1.60x = +60%)</summary>
        High = 3,
        
        /// <summary>Bônus muito alto (1.90x = +90%)</summary>
        VeryHigh = 4,
        
        /// <summary>Bônus extremo (2.50x = +150%)</summary>
        Extreme = 5
    }
    
    /// <summary>
    /// Extensões para converter tier em multiplicador
    /// </summary>
    public static class XPMultiplierTierExtensions
    {
        /// <summary>
        /// Converte tier em multiplicador real
        /// </summary>
        public static float ToMultiplier(this XPMultiplierTier tier)
        {
            return tier switch
            {
                XPMultiplierTier.None => 1.0f,      // +0%
                XPMultiplierTier.Low => 1.15f,      // +15%
                XPMultiplierTier.Medium => 1.35f,   // +35%
                XPMultiplierTier.High => 1.60f,     // +60%
                XPMultiplierTier.VeryHigh => 1.90f, // +90%
                XPMultiplierTier.Extreme => 2.50f,  // +150%
                _ => 1.0f
            };
        }
        
        /// <summary>
        /// Retorna a porcentagem de bônus (para exibição)
        /// </summary>
        public static int ToPercentage(this XPMultiplierTier tier)
        {
            return tier switch
            {
                XPMultiplierTier.None => 0,
                XPMultiplierTier.Low => 15,
                XPMultiplierTier.Medium => 35,
                XPMultiplierTier.High => 60,
                XPMultiplierTier.VeryHigh => 90,
                XPMultiplierTier.Extreme => 150,
                _ => 0
            };
        }
    }
}
