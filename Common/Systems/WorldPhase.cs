namespace Progression.Common.Systems
{
    /// <summary>
    /// Fases de progress�o do mundo
    /// </summary>
    public enum WorldPhase
    {
        /// <summary>
        /// Pr�-Hardmode: Antes de derrotar Wall of Flesh
        /// </summary>
        PreHardmode,
        
        /// <summary>
        /// Hardmode: Ap�s derrotar Wall of Flesh, antes de Moon Lord
        /// </summary>
        Hardmode,
        
        /// <summary>
        /// P�s-Moon Lord: Ap�s derrotar Moon Lord
        /// </summary>
        PostMoonLord
    }
    
    /// <summary>
    /// Modos de progress�o de n�vel de mundo
    /// </summary>
    public enum WorldLevelMode
    {
        /// <summary>
        /// N�vel baseado em bosses derrotados com caps por fase
        /// </summary>
        BossProgression,
        
        /// <summary>
        /// N�vel aumenta +1 por dia de jogo (tempo real no Terraria)
        /// </summary>
        TimeProgression,
        
        /// <summary>
        /// Sistema de inimigos nivelados desativado
        /// </summary>
        Disabled
    }
}
