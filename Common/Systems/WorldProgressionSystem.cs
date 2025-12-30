using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// Sistema de progressão do mundo baseado em bosses derrotados
    /// Calcula o nível de mundo automaticamente
    /// </summary>
    public class WorldProgressionSystem : ModSystem
    {
        // ==================== TRACKING DE BOSSES ====================
        
        /// <summary>
        /// Set de IDs de bosses derrotados pela primeira vez
        /// </summary>
        public static HashSet<int> DefeatedBosses { get; private set; } = new HashSet<int>();
        
        /// <summary>
        /// Lista de bosses pré-hardmode detectados (IDs)
        /// </summary>
        public static List<int> PreHardmodeBosses { get; private set; } = new List<int>();
        
        /// <summary>
        /// Lista de bosses hardmode detectados (IDs)
        /// </summary>
        public static List<int> HardmodeBosses { get; private set; } = new List<int>();
        
        // ==================== ESTADO DO MUNDO ====================
        
        /// <summary>
        /// Fase atual do mundo
        /// </summary>
        public static WorldPhase CurrentPhase { get; private set; } = WorldPhase.PreHardmode;
        
        /// <summary>
        /// Nível calculado do mundo
        /// </summary>
        public static int WorldLevel { get; private set; } = 1;
        
        /// <summary>
        /// Dias de jogo acumulados (para modo Time Progression)
        /// </summary>
        public static int DaysPlayed { get; private set; } = 0;
        
        private static double lastDayTime = 0;
        
        // ==================== INICIALIZAÇÃO ====================
        
        public override void OnWorldLoad()
        {
            DetectBosses();
            RecalculateWorldLevel();
        }
        
        public override void PostUpdateWorld()
        {
            // Atualiza contador de dias (modo Time Progression)
            var config = ModContent.GetInstance<Configs.ServerConfig>();
            if (config.WorldLevelMode == WorldLevelMode.TimeProgression)
            {
                // Detecta quando um dia completo passa
                if (Main.dayTime && Main.time < lastDayTime)
                {
                    DaysPlayed++;
                    RecalculateWorldLevel();
                    
                    if (Main.netMode != NetmodeID.Server)
                    {
                        Main.NewText($"Day {DaysPlayed}: World Level increased to {WorldLevel}!", Color.Gold);
                    }
                }
                
                lastDayTime = Main.time;
            }
            
            // Atualiza fase baseada em flags do mundo
            UpdateWorldPhase();
        }
        
        // ==================== DETECÇÃO DE BOSSES ====================
        
        /// <summary>
        /// Detecta todos os bosses do jogo (vanilla + mods)
        /// </summary>
        private void DetectBosses()
        {
            PreHardmodeBosses.Clear();
            HardmodeBosses.Clear();
            
            // Itera sobre todos os NPCs registrados
            for (int i = -65; i < NPCLoader.NPCCount; i++)
            {
                NPC npc = new NPC();
                npc.SetDefaults(i);
                
                // Verifica se é boss
                if (!npc.boss)
                    continue;
                
                // Classifica por fase
                if (IsPreHardmodeBoss(i))
                {
                    PreHardmodeBosses.Add(i);
                }
                else if (IsHardmodeBoss(i))
                {
                    HardmodeBosses.Add(i);
                }
            }
            
            // Debug: mostra bosses detectados
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText($"[World Progression] Detected {PreHardmodeBosses.Count} Pre-Hardmode bosses", Color.Cyan);
                Main.NewText($"[World Progression] Detected {HardmodeBosses.Count} Hardmode bosses", Color.Cyan);
            }
        }
        
        /// <summary>
        /// Verifica se um boss é pré-hardmode
        /// </summary>
        private bool IsPreHardmodeBoss(int npcID)
        {
            // Vanilla pré-hardmode
            if (npcID == NPCID.KingSlime) return true;
            if (npcID == NPCID.EyeofCthulhu) return true;
            if (npcID == NPCID.EaterofWorldsHead) return true;
            if (npcID == NPCID.BrainofCthulhu) return true;
            if (npcID == NPCID.QueenBee) return true;
            if (npcID == NPCID.SkeletronHead) return true;
            if (npcID == NPCID.Deerclops) return true;
            if (npcID == NPCID.WallofFlesh) return true;
            
            // Bosses de mods: assume que bosses spawnam no pré-hardmode se:
            // - Não está na lista de hardmode
            // - E mundo não está em hardmode ainda
            return false; // Por padrão, assume hardmode se não for vanilla conhecido
        }
        
        /// <summary>
        /// Verifica se um boss é hardmode
        /// </summary>
        private bool IsHardmodeBoss(int npcID)
        {
            // Vanilla hardmode
            if (npcID == NPCID.QueenSlimeBoss) return true;
            if (npcID == NPCID.TheDestroyer) return true;
            if (npcID == NPCID.SkeletronPrime) return true;
            if (npcID == NPCID.Retinazer) return true; // Twins
            if (npcID == NPCID.Spazmatism) return true; // Twins
            if (npcID == NPCID.Plantera) return true;
            if (npcID == NPCID.Golem) return true;
            if (npcID == NPCID.DukeFishron) return true;
            if (npcID == NPCID.HallowBoss) return true; // Empress
            if (npcID == NPCID.CultistBoss) return true;
            if (npcID == NPCID.MoonLordCore) return true;
            
            // Pillars
            if (npcID == NPCID.LunarTowerSolar) return true;
            if (npcID == NPCID.LunarTowerVortex) return true;
            if (npcID == NPCID.LunarTowerNebula) return true;
            if (npcID == NPCID.LunarTowerStardust) return true;
            
            return false;
        }
        
        // ==================== TRACKING DE BOSS DERROTADO ====================
        
        /// <summary>
        /// Registra boss derrotado (chamado via GlobalNPC)
        /// </summary>
        public static void RegisterBossDefeat(int npcID)
        {
            // Ignora se já foi derrotado antes
            if (DefeatedBosses.Contains(npcID))
                return;
            
            DefeatedBosses.Add(npcID);
            RecalculateWorldLevel();
            
            // Mensagem dramática
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText($"???????????????????????????????", Color.Gold);
                Main.NewText($"World Level increased to {WorldLevel}!", new Color(255, 215, 0));
                Main.NewText($"Enemies grow stronger...", Color.OrangeRed);
                Main.NewText($"???????????????????????????????", Color.Gold);
            }
            
            // Som épico
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item4);
        }
        
        // ==================== CÁLCULO DE NÍVEL DE MUNDO ====================
        
        /// <summary>
        /// Recalcula o nível de mundo baseado em bosses derrotados ou tempo
        /// </summary>
        public static void RecalculateWorldLevel()
        {
            var config = ModContent.GetInstance<Configs.ServerConfig>();
            
            switch (config.WorldLevelMode)
            {
                case WorldLevelMode.BossProgression:
                    CalculateBossProgressionLevel();
                    break;
                    
                case WorldLevelMode.TimeProgression:
                    CalculateTimeProgressionLevel();
                    break;
                    
                case WorldLevelMode.Disabled:
                    WorldLevel = 0;
                    break;
            }
        }
        
        /// <summary>
        /// Calcula nível baseado em bosses derrotados (modo padrão)
        /// Usa progressão incremental: primeiro boss dá 1, segundo dá 2, etc.
        /// </summary>
        private static void CalculateBossProgressionLevel()
        {
            var config = ModContent.GetInstance<Configs.ServerConfig>();
            
            int preHardmodeLevel = 0;
            int hardmodeLevel = 0;
            int postMoonLordLevel = 0;
            
            // ==================== PRÉ-HARDMODE ====================
            if (CurrentPhase >= WorldPhase.PreHardmode)
            {
                int preDefeated = DefeatedBosses.Count(id => PreHardmodeBosses.Contains(id));
                int preTotal = PreHardmodeBosses.Count;
                int preCap = config.PreHardmodeMaxLevel;
                
                preHardmodeLevel = CalculateProgressiveLevel(preDefeated, preTotal, preCap);
            }
            
            // ==================== HARDMODE ====================
            if (CurrentPhase >= WorldPhase.Hardmode)
            {
                int hardDefeated = DefeatedBosses.Count(id => HardmodeBosses.Contains(id) && id != NPCID.MoonLordCore);
                int hardTotal = HardmodeBosses.Count - 1; // -1 para excluir Moon Lord
                int hardCap = config.HardmodeMaxLevel;
                
                hardmodeLevel = CalculateProgressiveLevel(hardDefeated, hardTotal, hardCap);
            }
            
            // ==================== PÓS-MOON LORD ====================
            if (CurrentPhase >= WorldPhase.PostMoonLord)
            {
                // Simplesmente adiciona o cap pós-moon lord
                postMoonLordLevel = config.PostMoonLordMaxLevel;
            }
            
            // Soma tudo (acumulativo)
            WorldLevel = preHardmodeLevel + hardmodeLevel + postMoonLordLevel;
            
            // Garante mínimo de 1
            if (WorldLevel < 1)
                WorldLevel = 1;
        }
        
        /// <summary>
        /// Calcula nível progressivo baseado em bosses derrotados
        /// Fórmula: cada boss dá mais níveis que o anterior
        /// 
        /// Exemplo: Cap 10, 4 bosses
        /// Boss 1: +1 (total: 1)
        /// Boss 2: +2 (total: 3)
        /// Boss 3: +3 (total: 6)
        /// Boss 4: +4 (total: 10)
        /// 
        /// Soma: 1+2+3+4 = 10 = n(n+1)/2
        /// </summary>
        private static int CalculateProgressiveLevel(int defeated, int total, int cap)
        {
            if (total <= 0 || defeated <= 0)
                return 0;
            
            // Calcula o incremento de cada boss
            // Soma de 1 até n = n(n+1)/2 = cap
            // Então: incremento[i] = (i * cap) / (total * (total+1) / 2)
            
            int totalSum = 0;
            for (int i = 1; i <= defeated; i++)
            {
                // Incremento progressivo escalonado para atingir o cap
                float increment = (2f * cap * i) / (total * (total + 1));
                totalSum += (int)increment;
            }
            
            // Garante que não ultrapasse o cap
            return System.Math.Min(totalSum, cap);
        }
        
        /// <summary>
        /// Calcula nível baseado em dias jogados
        /// </summary>
        private static void CalculateTimeProgressionLevel()
        {
            WorldLevel = 1 + DaysPlayed;
        }
        
        // ==================== ATUALIZAÇÃO DE FASE ====================
        
        /// <summary>
        /// Atualiza a fase do mundo baseada em flags do Terraria
        /// </summary>
        private static void UpdateWorldPhase()
        {
            if (NPC.downedMoonlord)
            {
                CurrentPhase = WorldPhase.PostMoonLord;
            }
            else if (Main.hardMode)
            {
                CurrentPhase = WorldPhase.Hardmode;
            }
            else
            {
                CurrentPhase = WorldPhase.PreHardmode;
            }
        }
        
        // ==================== SAVE/LOAD ====================
        
        public override void SaveWorldData(TagCompound tag)
        {
            tag["defeatedBosses"] = DefeatedBosses.ToList();
            tag["worldLevel"] = WorldLevel;
            tag["daysPlayed"] = DaysPlayed;
            tag["currentPhase"] = (int)CurrentPhase;
        }
        
        public override void LoadWorldData(TagCompound tag)
        {
            DefeatedBosses = tag.GetList<int>("defeatedBosses").ToHashSet();
            WorldLevel = tag.GetInt("worldLevel");
            DaysPlayed = tag.GetInt("daysPlayed");
            CurrentPhase = (WorldPhase)tag.GetInt("currentPhase");
            
            // Redetecta bosses após carregar
            DetectBosses();
            RecalculateWorldLevel();
        }
        
        public override void ClearWorld()
        {
            DefeatedBosses.Clear();
            PreHardmodeBosses.Clear();
            HardmodeBosses.Clear();
            WorldLevel = 1;
            DaysPlayed = 0;
            CurrentPhase = WorldPhase.PreHardmode;
            lastDayTime = 0;
        }
        
        // ==================== UTILITÁRIOS ====================
        
        /// <summary>
        /// Obtém o cap atual baseado na fase
        /// </summary>
        public static int GetCurrentPhaseCap()
        {
            var config = ModContent.GetInstance<Configs.ServerConfig>();
            
            return CurrentPhase switch
            {
                WorldPhase.PreHardmode => config.PreHardmodeMaxLevel,
                WorldPhase.Hardmode => config.PreHardmodeMaxLevel + config.HardmodeMaxLevel,
                WorldPhase.PostMoonLord => config.PreHardmodeMaxLevel + config.HardmodeMaxLevel + config.PostMoonLordMaxLevel,
                _ => config.PreHardmodeMaxLevel
            };
        }
        
        /// <summary>
        /// Obtém o nível de um NPC com variância gaussiana
        /// </summary>
        public static int GetNPCLevel()
        {
            var config = ModContent.GetInstance<Configs.ServerConfig>();
            
            if (config.WorldLevelMode == WorldLevelMode.Disabled)
                return 0;
            
            // Aplica variância: WorldLevel ± variance
            int variance = Main.rand.Next(-config.LevelVariance, config.LevelVariance + 1);
            int npcLevel = WorldLevel + variance;
            
            // Garante que não seja negativo e não ultrapasse o cap atual
            int currentCap = GetCurrentPhaseCap();
            npcLevel = System.Math.Clamp(npcLevel, 1, currentCap);
            
            return npcLevel;
        }
    }
}
