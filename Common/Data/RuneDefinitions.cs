using Microsoft.Xna.Framework;

namespace SignatureEquipmentDeluxe.Common.Data
{
    /// <summary>
    /// Define as propriedades e efeitos de cada tipo de runa
    /// </summary>
    public static class RuneDefinitions
    {
        /// <summary>
        /// Retorna o nome traduzível da runa
        /// </summary>
        public static string GetName(RuneType type)
        {
            return type switch
            {
                RuneType.Fire => "Fire Rune",
                RuneType.Ice => "Ice Rune",
                RuneType.Poison => "Poison Rune",
                RuneType.Lightning => "Lightning Rune",
                RuneType.AttackSpeed => "Attack Speed Rune",
                RuneType.LifeRegen => "Life Regen Rune",
                RuneType.Lifesteal => "Lifesteal Rune",
                RuneType.CurseBerserker => "Curse of Berserker",
                RuneType.CurseGlass => "Curse of Glass Cannon",
                RuneType.CurseAnnihilation => "Curse of Annihilation",
                _ => "Unknown Rune"
            };
        }
        
        /// <summary>
        /// Retorna a descrição da runa
        /// </summary>
        public static string GetDescription(RuneType type)
        {
            return type switch
            {
                RuneType.Fire => "Adds fire damage and burns enemies",
                RuneType.Ice => "Adds ice damage and slows enemies",
                RuneType.Poison => "Adds poison damage over time",
                RuneType.Lightning => "Adds electric damage with chain effect",
                RuneType.AttackSpeed => "Increases attack speed",
                RuneType.LifeRegen => "Regenerates health over time",
                RuneType.Lifesteal => "Steals life from enemies",
                RuneType.CurseBerserker => "+50% damage, -30% defense | +25% XP per hit, +50% per kill",
                RuneType.CurseGlass => "+100% critical chance, -50% max life | +25% XP per hit, +50% per kill",
                RuneType.CurseAnnihilation => "+150% damage, -35% max life, defense = 0 | +25% XP per hit, +50% per kill",
                _ => ""
            };
        }
        
        /// <summary>
        /// Retorna a cor associada à runa
        /// </summary>
        public static Color GetColor(RuneType type)
        {
            return type switch
            {
                RuneType.Fire => new Color(255, 100, 0),           // Laranja
                RuneType.Ice => new Color(100, 200, 255),          // Azul claro
                RuneType.Poison => new Color(100, 255, 100),       // Verde
                RuneType.Lightning => new Color(255, 255, 100),    // Amarelo
                RuneType.AttackSpeed => new Color(255, 200, 100),  // Dourado
                RuneType.LifeRegen => new Color(255, 100, 150),    // Rosa
                RuneType.Lifesteal => new Color(200, 50, 50),      // Vermelho escuro
                RuneType.CurseBerserker => new Color(150, 0, 0),   // Vermelho sangue
                RuneType.CurseGlass => new Color(200, 200, 255),   // Branco gelo
                RuneType.CurseAnnihilation => new Color(100, 0, 100), // Roxo escuro
                _ => Color.White
            };
        }
        
        /// <summary>
        /// Calcula o bônus de dano da runa baseado no nível
        /// </summary>
        public static float GetDamageBonus(RuneType type, int runeLevel)
        {
            float scalingPercent = runeLevel / 100f; // 0.0 a 1.0
            
            return type switch
            {
                RuneType.Fire => 5f + (15f * scalingPercent),        // 5% a 20% de dano de fogo
                RuneType.Ice => 5f + (15f * scalingPercent),         // 5% a 20% de dano de gelo
                RuneType.Poison => 3f + (12f * scalingPercent),      // 3% a 15% de dano de veneno
                RuneType.Lightning => 7f + (18f * scalingPercent),   // 7% a 25% de dano elétrico
                RuneType.CurseBerserker => 50f,                      // Fixo +50%
                RuneType.CurseGlass => 0f,                           // Não dá dano, dá crit
                RuneType.CurseAnnihilation => 150f,                  // Fixo +150%
                _ => 0f
            };
        }
        
        /// <summary>
        /// Calcula o bônus de velocidade de ataque (use speed)
        /// </summary>
        public static float GetAttackSpeedBonus(RuneType type, int runeLevel)
        {
            if (type != RuneType.AttackSpeed)
                return 0f;
            
            float scalingPercent = runeLevel / 100f;
            return 5f + (20f * scalingPercent); // 5% a 25% de velocidade
        }
        
        /// <summary>
        /// Calcula o bônus de regeneração de vida
        /// </summary>
        public static int GetLifeRegenBonus(RuneType type, int runeLevel)
        {
            if (type != RuneType.LifeRegen)
                return 0;
            
            return 1 + (runeLevel / 20); // +1 a +6 life regen
        }
        
        /// <summary>
        /// Calcula o bônus de lifesteal (porcentagem)
        /// </summary>
        public static float GetLifestealBonus(RuneType type, int runeLevel)
        {
            if (type != RuneType.Lifesteal)
                return 0f;
            
            float scalingPercent = runeLevel / 100f;
            return 2f + (8f * scalingPercent); // 2% a 10% de lifesteal
        }
        
        /// <summary>
        /// Calcula o bônus de critical chance
        /// </summary>
        public static float GetCritBonus(RuneType type, int runeLevel)
        {
            if (type != RuneType.CurseGlass)
                return 0f;
            
            return 100f; // Fixo +100% crit
        }
        
        /// <summary>
        /// Calcula a penalidade de defesa (valor negativo)
        /// </summary>
        public static float GetDefensePenalty(RuneType type)
        {
            return type switch
            {
                RuneType.CurseBerserker => -30f,      // -30% defesa
                RuneType.CurseAnnihilation => -100f,  // Defesa = 0 (remove tudo)
                _ => 0f
            };
        }
        
        /// <summary>
        /// Calcula a penalidade de vida máxima (porcentagem)
        /// </summary>
        public static float GetMaxLifePenalty(RuneType type)
        {
            return type switch
            {
                RuneType.CurseGlass => -50f,         // -50% vida máxima
                RuneType.CurseAnnihilation => -35f,  // -35% vida máxima
                _ => 0f
            };
        }
    }
}
