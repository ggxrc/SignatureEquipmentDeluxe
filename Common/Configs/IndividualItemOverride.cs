using System;
using System.ComponentModel;
using Progression.Common.Systems;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace Progression.Common.Configs
{
    /// <summary>
    /// Configuração de override individual para um item específico.
    /// Permite customizar completamente o escalonamento de um item, ignorando as configurações globais.
    /// </summary>
    public class IndividualItemOverride : IEquatable<IndividualItemOverride>
    {
        [Header("$Mods.Progression.Config.IndividualItemOverride.TargetHeader")]
        
        /// <summary>
        /// Item alvo para aplicar este override
        /// </summary>
        [BackgroundColor(100, 100, 150)]
        public ItemDefinition TargetItem { get; set; }

        [Header("$Mods.Progression.Config.IndividualItemOverride.StatsHeader")]

        /// <summary>
        /// Configuração customizada de dano para este item
        /// </summary>
        [BackgroundColor(120, 60, 60)]
        [SeparatePage]
        public ItemStatInt CustomDamage { get; set; } = new ItemStatInt
        {
            ScalingMode = ScalingMode.Fixed,
            PerLevel = 1,
            PerLevelMult = 1,
            PercentagePerLevel = 15,
            Max = 0
        };

            /// <summary>
            /// Tipo de aumento de status para este item (Raw ou Percentage)
            /// </summary>
            [BackgroundColor(80, 60, 40)]
            public StatusIncreaseType StatusIncreaseType { get; set; } = StatusIncreaseType.Raw;

        /// <summary>
        /// Configuração customizada de chance crítica para este item
        /// </summary>

        [BackgroundColor(110, 60, 60)]
        [SeparatePage]
        public ItemStatInt CustomCritChance { get; set; } = new ItemStatInt
        {
            ScalingMode = ScalingMode.Fixed,
            PerLevel = 1,
            PerLevelMult = 1,
            PercentagePerLevel = 15,
            Max = 0
        };

        /// <summary>
        /// Configuração customizada de velocidade de uso para este item
        /// </summary>

        [BackgroundColor(100, 60, 60)]
        [SeparatePage]
        public ItemStatFloat CustomUseTime { get; set; } = new ItemStatFloat
        {
            ScalingMode = ScalingMode.Fixed,
            PerLevel = 1f,
            PerLevelMult = 1,
            PercentagePerLevel = 15,
            Max = 0
        };

        /// <summary>
        /// Configuração customizada de velocidade de animação para este item
        /// </summary>

        [BackgroundColor(90, 60, 60)]
        [SeparatePage]
        public ItemStatFloat CustomUseAnimation { get; set; } = new ItemStatFloat
        {
            ScalingMode = ScalingMode.Fixed,
            PerLevel = 1f,
            PerLevelMult = 1,
            PercentagePerLevel = 15,
            Max = 0
        };

        /// <summary>
        /// Configuração customizada de tamanho (melee) para este item
        /// </summary>

        [BackgroundColor(80, 60, 60)]
        [SeparatePage]
        public ItemStatFloat CustomMeleeSize { get; set; } = new ItemStatFloat
        {
            ScalingMode = ScalingMode.Fixed,
            PerLevel = 1f,
            PerLevelMult = 1,
            PercentagePerLevel = 15,
            Max = 0
        };

        /// <summary>
        /// Configuração customizada de redução de custo de mana para este item
        /// </summary>
        [BackgroundColor(70, 60, 90)]
        [SeparatePage]
        public ItemStatFloat CustomManaCostReduction { get; set; } = new ItemStatFloat
        {
            ScalingMode = ScalingMode.Fixed,
            PerLevel = 1f,
            PerLevelMult = 1,
            PercentagePerLevel = 15,
            Max = 0
        };

        /// <summary>
        /// Configuração customizada de redução de consumo de munição para este item
        /// </summary>
        [BackgroundColor(70, 90, 60)]
        [SeparatePage]
        public ItemStatFloat CustomAmmoConsumptionReduction { get; set; } = new ItemStatFloat
        {
            ScalingMode = ScalingMode.Fixed,
            PerLevel = 1f,
            PerLevelMult = 1,
            PercentagePerLevel = 15,
            Max = 0
        };

        public IndividualItemOverride()
        {
            TargetItem = new ItemDefinition();
        }

        public override string ToString()
        {
            if (TargetItem == null || TargetItem.Type == 0)
            {
                return "Empty Override";
            }

            return ItemLoader.GetItem(TargetItem.Type)?.DisplayName.ToString() ?? $"Item ID: {TargetItem.Type}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IndividualItemOverride);
        }

        public bool Equals(IndividualItemOverride other)
        {
            if (other == null)
                return false;

            if (TargetItem == null && other.TargetItem == null)
                return true;

            if (TargetItem == null || other.TargetItem == null)
                return false;

            return TargetItem.Type == other.TargetItem.Type;
        }

        public override int GetHashCode()
        {
            if (TargetItem == null || TargetItem.Type == 0)
                return 0;

            return TargetItem.Type.GetHashCode();
        }
    }
}
