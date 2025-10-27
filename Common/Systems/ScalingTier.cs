using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// Define uma faixa de nível com valores específicos de scaling
    /// Permite progressão não-linear dos stats conforme o jogador evolui
    /// </summary>
    public class ScalingTier
    {
        /// <summary>
        /// Nível inicial desta faixa (inclusivo)
        /// </summary>
        [Range(0, int.MaxValue)]
        [DefaultValue(1)]
        public int StartLevel { get; set; }

        /// <summary>
        /// Valor ganho por nível nesta faixa
        /// </summary>
        [Range(-2.5f, 2.5f)]
        [Increment(0.05f)]
        [DefaultValue(1f)]
        public float PerLevel { get; set; }

        /// <summary>
        /// Multiplicador aplicado ao PerLevel desta faixa
        /// </summary>
        [Range(int.MinValue, int.MaxValue)]
        [DefaultValue(1)]
        public int PerLevelMult { get; set; }

        public ScalingTier()
        {
            StartLevel = 1;
            PerLevel = 1f;
            PerLevelMult = 1;
        }

        public override string ToString()
        {
            return $"Lvl {StartLevel}+: {PerLevel} × {PerLevelMult}";
        }

        public override bool Equals(object obj)
        {
            if (obj is ScalingTier other)
            {
                return StartLevel == other.StartLevel && 
                       Math.Abs(PerLevel - other.PerLevel) < 0.0001f && 
                       PerLevelMult == other.PerLevelMult;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(StartLevel, PerLevel, PerLevelMult);
        }
    }
}
