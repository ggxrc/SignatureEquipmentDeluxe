namespace Progression.Common.Systems
{
    /// <summary>
    /// Define como o scaling progressivo é calculado
    /// </summary>
    public enum ScalingMode
    {
        /// <summary>
        /// Modo Fixo: Bônus fixo por nível usando PerLevel × PerLevelMult
        /// Exemplo: Nível 50 com PerLevel=2 e Mult=1 = 50 × 2 × 1 = 100 de bônus
        /// </summary>
        Fixed,

        /// <summary>
        /// Modo Escalonado: Bônus varia por faixa de nível usando Scaling Tiers
        /// Exemplo: Níveis 1-50 ganham 2/nível, 51-100 ganham 5/nível
        /// </summary>
        Tiered
    }
}
