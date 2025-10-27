namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// Define como o scaling progressivo é calculado através dos tiers
    /// </summary>
    public enum ScalingMode
    {
        /// <summary>
        /// Usa apenas PerLevel e PerLevelMult originais (modo legado, sem tiers)
        /// </summary>
        Legacy,

        /// <summary>
        /// Calcula de forma cumulativa: soma o valor de cada tier até o nível atual
        /// Exemplo: Lvl 60 com tiers [1-50: 2/lvl] [51+: 5/lvl] = (50*2) + (10*5) = 150
        /// </summary>
        Cumulative,

        /// <summary>
        /// Usa apenas o tier atual: aplica o PerLevel do tier onde o nível está
        /// Exemplo: Lvl 60 com tiers [1-50: 2/lvl] [51+: 5/lvl] = (60*5) = 300
        /// </summary>
        CurrentTierOnly,

        /// <summary>
        /// Acumulativo mas soma apenas a diferença de cada tier
        /// Exemplo: Lvl 60 com tiers [1-50: 2/lvl] [51+: 5/lvl] = (50*2) + (10*5) = 150
        /// Mesmo que Cumulative, mas mais claro na intenção
        /// </summary>
        AccumulativePerTier
    }
}
