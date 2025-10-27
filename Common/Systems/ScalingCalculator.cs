using System;
using System.Collections.Generic;
using System.Linq;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// Classe utilitária para calcular valores de stats baseados em scaling tiers
    /// </summary>
    public static class ScalingCalculator
    {
        /// <summary>
        /// Calcula o valor de um stat baseado no nível e nos tiers configurados
        /// </summary>
        /// <param name="level">Nível atual do item</param>
        /// <param name="scalingMode">Modo de cálculo</param>
        /// <param name="scalingTiers">Lista de tiers (deve estar ordenada por StartLevel)</param>
        /// <param name="legacyPerLevel">PerLevel usado no modo Legacy</param>
        /// <param name="legacyPerLevelMult">PerLevelMult usado no modo Legacy</param>
        /// <returns>Valor calculado do stat</returns>
        public static float CalculateStat(
            int level,
            ScalingMode scalingMode,
            List<ScalingTier> scalingTiers,
            float legacyPerLevel,
            int legacyPerLevelMult)
        {
            // Modo Legacy: usa apenas os valores originais PerLevel e PerLevelMult
            if (scalingMode == ScalingMode.Legacy || scalingTiers == null || scalingTiers.Count == 0)
            {
                return level * legacyPerLevel * legacyPerLevelMult;
            }

            // Ordena os tiers por StartLevel para garantir processamento correto
            var sortedTiers = scalingTiers.OrderBy(t => t.StartLevel).ToList();

            // Remove tiers que começam além do nível atual
            var applicableTiers = sortedTiers.Where(t => t.StartLevel <= level).ToList();

            if (applicableTiers.Count == 0)
            {
                // Se nenhum tier se aplica, retorna 0 ou usa legado como fallback
                return level * legacyPerLevel * legacyPerLevelMult;
            }

            float totalValue = 0f;

            switch (scalingMode)
            {
                case ScalingMode.Cumulative:
                case ScalingMode.AccumulativePerTier:
                    // Calcula cumulativamente: soma o valor de cada faixa de tier
                    for (int i = 0; i < applicableTiers.Count; i++)
                    {
                        var currentTier = applicableTiers[i];
                        int tierStartLevel = currentTier.StartLevel;
                        int tierEndLevel;

                        // Determina onde este tier termina
                        if (i + 1 < applicableTiers.Count)
                        {
                            // Termina onde o próximo tier começa
                            tierEndLevel = applicableTiers[i + 1].StartLevel - 1;
                        }
                        else
                        {
                            // Último tier: vai até o nível atual
                            tierEndLevel = level;
                        }

                        // Calcula quantos níveis estão neste tier
                        int levelsInThisTier = Math.Max(0, tierEndLevel - tierStartLevel + 1);

                        // Adiciona o valor deste tier
                        totalValue += levelsInThisTier * currentTier.PerLevel * currentTier.PerLevelMult;
                    }
                    break;

                case ScalingMode.CurrentTierOnly:
                    // Usa apenas o tier onde o nível atual está
                    var currentApplicableTier = applicableTiers.Last();
                    totalValue = level * currentApplicableTier.PerLevel * currentApplicableTier.PerLevelMult;
                    break;

                default:
                    // Fallback para legacy
                    totalValue = level * legacyPerLevel * legacyPerLevelMult;
                    break;
            }

            return totalValue;
        }

        /// <summary>
        /// Versão simplificada para stats inteiros
        /// </summary>
        public static int CalculateStatInt(
            int level,
            ScalingMode scalingMode,
            List<ScalingTier> scalingTiers,
            int legacyPerLevel,
            int legacyPerLevelMult)
        {
            return (int)CalculateStat(level, scalingMode, scalingTiers, legacyPerLevel, legacyPerLevelMult);
        }

        /// <summary>
        /// Calcula o tier atual que se aplica a um determinado nível
        /// </summary>
        public static ScalingTier GetCurrentTier(int level, List<ScalingTier> scalingTiers)
        {
            if (scalingTiers == null || scalingTiers.Count == 0)
                return null;

            var sortedTiers = scalingTiers.OrderBy(t => t.StartLevel).ToList();
            var applicableTiers = sortedTiers.Where(t => t.StartLevel <= level).ToList();

            return applicableTiers.Count > 0 ? applicableTiers.Last() : null;
        }

        /// <summary>
        /// Retorna informações de debug sobre o cálculo de um stat
        /// </summary>
        public static string GetCalculationDebugInfo(
            int level,
            ScalingMode scalingMode,
            List<ScalingTier> scalingTiers,
            float legacyPerLevel,
            int legacyPerLevelMult)
        {
            var result = CalculateStat(level, scalingMode, scalingTiers, legacyPerLevel, legacyPerLevelMult);
            var currentTier = GetCurrentTier(level, scalingTiers);

            return $"Level {level} | Mode: {scalingMode} | Result: {result:F2} | Current Tier: {currentTier?.ToString() ?? "Legacy"}";
        }
    }
}
