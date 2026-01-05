using System;
using System.Collections.Generic;
using System.Linq;
using Progression.Common.Configs;

namespace Progression.Common.Systems
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
        /// <param name="fixedPerLevel">PerLevel usado no modo Fixed</param>
        /// <param name="fixedPerLevelMult">PerLevelMult usado no modo Fixed</param>
        /// <returns>Valor calculado do stat</returns>
        public static float CalculateStat(
            int level,
            ScalingMode scalingMode,
            List<ScalingTier> scalingTiers,
            float fixedPerLevel,
            int fixedPerLevelMult)
        {
            return CalculateStat(level, scalingMode, scalingTiers, fixedPerLevel, fixedPerLevelMult, 0, 0, StatusIncreaseType.Raw);
        }

        /// <summary>
        /// Calcula o valor de um stat baseado no nível e nos tiers configurados (com suporte a porcentagem)
        /// </summary>
        /// <param name="level">Nível atual do item</param>
        /// <param name="scalingMode">Modo de cálculo</param>
        /// <param name="scalingTiers">Lista de tiers (deve estar ordenada por StartLevel)</param>
        /// <param name="fixedPerLevel">PerLevel usado no modo Fixed (Raw)</param>
        /// <param name="fixedPerLevelMult">PerLevelMult usado no modo Fixed (Raw)</param>
        /// <param name="originalStat">Stat original do item</param>
        /// <param name="percentagePerLevel">Porcentagem por nível (Percentage)</param>
        /// <param name="statusIncreaseType">Tipo de aumento: Raw ou Percentage</param>
        /// <returns>Valor calculado do stat</returns>
        public static float CalculateStat(
            int level,
            ScalingMode scalingMode,
            List<ScalingTier> scalingTiers,
            float fixedPerLevel,
            int fixedPerLevelMult,
            float originalStat,
            int percentagePerLevel,
            StatusIncreaseType statusIncreaseType)
        {
            // Se for modo Percentage, calcula baseado no stat original
            if (statusIncreaseType == StatusIncreaseType.Percentage && originalStat > 0)
            {
                // Modo Fixed com Percentage
                if (scalingMode == ScalingMode.Fixed || scalingTiers == null || scalingTiers.Count == 0)
                {
                    return level * originalStat * (percentagePerLevel / 100f);
                }

                // Modo Tiered com Percentage
                var sortedTiersPercentage = scalingTiers.OrderBy(t => t.StartLevel).ToList();
                var applicableTiersPercentage = sortedTiersPercentage.Where(t => t.StartLevel <= level).ToList();

                if (applicableTiersPercentage.Count == 0)
                {
                    return level * originalStat * (percentagePerLevel / 100f);
                }

                float totalValuePercentage = 0f;

                for (int i = 0; i < applicableTiersPercentage.Count; i++)
                {
                    var currentTier = applicableTiersPercentage[i];
                    int tierStartLevel = currentTier.StartLevel;
                    int tierEndLevel;

                    if (i + 1 < applicableTiersPercentage.Count)
                    {
                        tierEndLevel = applicableTiersPercentage[i + 1].StartLevel - 1;
                    }
                    else
                    {
                        tierEndLevel = level;
                    }

                    int levelsInThisTier = Math.Max(0, tierEndLevel - tierStartLevel + 1);
                    totalValuePercentage += levelsInThisTier * originalStat * (currentTier.PercentagePerLevel / 100f);
                }

                return totalValuePercentage;
            }

            // Modo Raw (comportamento original)
            // Modo Fixed: usa apenas os valores fixos PerLevel e PerLevelMult
            if (scalingMode == ScalingMode.Fixed || scalingTiers == null || scalingTiers.Count == 0)
            {
                return level * fixedPerLevel * fixedPerLevelMult;
            }

            // Ordena os tiers por StartLevel para garantir processamento correto
            var sortedTiers = scalingTiers.OrderBy(t => t.StartLevel).ToList();

            // Remove tiers que começam além do nível atual
            var applicableTiers = sortedTiers.Where(t => t.StartLevel <= level).ToList();

            if (applicableTiers.Count == 0)
            {
                // Se nenhum tier se aplica, retorna 0 ou usa fixed como fallback
                return level * fixedPerLevel * fixedPerLevelMult;
            }

            float totalValue = 0f;

            switch (scalingMode)
            {
                case ScalingMode.Tiered:
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

                default:
                    // Fallback para fixed
                    totalValue = level * fixedPerLevel * fixedPerLevelMult;
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
            int fixedPerLevel,
            int fixedPerLevelMult)
        {
            return (int)CalculateStat(level, scalingMode, scalingTiers, fixedPerLevel, fixedPerLevelMult);
        }

        /// <summary>
        /// Versão simplificada para stats inteiros (com suporte a porcentagem)
        /// </summary>
        public static int CalculateStatInt(
            int level,
            ScalingMode scalingMode,
            List<ScalingTier> scalingTiers,
            int fixedPerLevel,
            int fixedPerLevelMult,
            int originalStat,
            int percentagePerLevel,
            StatusIncreaseType statusIncreaseType)
        {
            return (int)CalculateStat(level, scalingMode, scalingTiers, fixedPerLevel, fixedPerLevelMult, originalStat, percentagePerLevel, statusIncreaseType);
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
            float fixedPerLevel,
            int fixedPerLevelMult)
        {
            var result = CalculateStat(level, scalingMode, scalingTiers, fixedPerLevel, fixedPerLevelMult);
            var currentTier = GetCurrentTier(level, scalingTiers);

            return $"Level {level} | Mode: {scalingMode} | Result: {result:F2} | Current Tier: {currentTier?.ToString() ?? "Fixed"}";
        }
    }
}
