namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// Fases de progressão do mundo
    /// </summary>
    public enum WorldPhase
    {
        /// <summary>
        /// Pré-Hardmode: Antes de derrotar Wall of Flesh
        /// </summary>
        PreHardmode,
        
        /// <summary>
        /// Hardmode: Após derrotar Wall of Flesh, antes de Moon Lord
        /// </summary>
        Hardmode,
        
        /// <summary>
        /// Pós-Moon Lord: Após derrotar Moon Lord
        /// </summary>
        PostMoonLord
    }
    
    /// <summary>
    /// Modos de progressão de nível de mundo
    /// </summary>
    public enum WorldLevelMode
    {
        /// <summary>
        /// Nível baseado em bosses derrotados com caps por fase
        /// </summary>
        BossProgression,
        
        /// <summary>
        /// Nível aumenta +1 por dia de jogo (tempo real no Terraria)
        /// </summary>
        TimeProgression,
        
        /// <summary>
        /// Sistema de inimigos nivelados desativado
        /// </summary>
        Disabled
    }
}
