using Terraria.ModLoader.IO;

namespace SignatureEquipmentDeluxe.Common.Data
{
    /// <summary>
    /// Representa uma runa equipada em uma arma, com level e XP próprios
    /// </summary>
    public class EquippedRune
    {
        public RuneType Type { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int MaxLevel { get; set; } // Igual ao max level da arma
        
        public EquippedRune()
        {
            Type = RuneType.None;
            Level = 1;
            Experience = 0;
            MaxLevel = 100;
        }
        
        public EquippedRune(RuneType type, int maxLevel)
        {
            Type = type;
            Level = 1;
            Experience = 0;
            MaxLevel = maxLevel;
        }
        
        /// <summary>
        /// Verifica se é uma maldição
        /// </summary>
        public bool IsCurse()
        {
            return Type == RuneType.CurseBerserker || 
                   Type == RuneType.CurseGlass || 
                   Type == RuneType.CurseAnnihilation;
        }
        
        /// <summary>
        /// Calcula XP necessário para próximo nível (sempre o dobro da arma)
        /// </summary>
        public int GetXPForNextLevel()
        {
            // XP base da arma = 100 * level
            // XP da runa = 2x isso
            return 200 * Level;
        }
        
        /// <summary>
        /// Adiciona XP à runa e verifica level up
        /// </summary>
        public bool AddExperience(int amount)
        {
            if (Level >= MaxLevel)
                return false;
            
            Experience += amount;
            
            bool leveledUp = false;
            while (Experience >= GetXPForNextLevel() && Level < MaxLevel)
            {
                Experience -= GetXPForNextLevel();
                Level++;
                leveledUp = true;
            }
            
            return leveledUp;
        }
        
        /// <summary>
        /// Salva dados da runa
        /// </summary>
        public TagCompound Save()
        {
            return new TagCompound
            {
                ["Type"] = (int)Type,
                ["Level"] = Level,
                ["Experience"] = Experience,
                ["MaxLevel"] = MaxLevel
            };
        }
        
        /// <summary>
        /// Carrega dados da runa
        /// </summary>
        public static EquippedRune Load(TagCompound tag)
        {
            return new EquippedRune
            {
                Type = (RuneType)tag.GetInt("Type"),
                Level = tag.GetInt("Level"),
                Experience = tag.GetInt("Experience"),
                MaxLevel = tag.GetInt("MaxLevel")
            };
        }
    }
}
