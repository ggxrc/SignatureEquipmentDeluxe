using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Progression.Common.Systems;
using Progression.Common.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Progression.Common.Systems
{
    /// <summary>
    /// World progression system based on defeated bosses
    /// Automatically calculates world level
    /// </summary>
    public class WorldProgressionSystem : ModSystem
    {
        // ==================== TRACKING DE BOSSES ====================

        /// <summary>
        /// Set de IDs de bosses derrotados pela primeira vez
        /// </summary>
        public static HashSet<int> DefeatedBosses { get; private set; } = new HashSet<int>();

        /// <summary>
        /// List of pre-hardmode bosses detected (IDs)
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
        /// Calculated world level
        /// </summary>
        public static int WorldLevel { get; private set; } = 1;

        /// <summary>
        /// Dias de jogo acumulados (para modo Time Progression)
        /// </summary>
        public static int DaysPlayed { get; private set; } = 0;

        private static double lastDayTime = 0;

        // ==================== INITIALIZATION ====================

        public override void OnWorldLoad()
        {
            DetectBosses();
            RecalculateWorldLevel();
        }

        public override void PostUpdateWorld()
        {
            // Atualiza contador de dias (modo Time Progression)
            var config = ModContent.GetInstance<Configs.WorldConfig>();
            if (config.WorldLevelMode == WorldLevelMode.TimeProgression)
            {
                // Detecta quando um dia completo passa
                if (Main.dayTime && Main.time < lastDayTime)
                {
                    DaysPlayed++;
                    RecalculateWorldLevel();

                    if (Main.netMode != NetmodeID.Server)
                    {
                        WorldLevelNotificationUI.Show(
                            WorldLevel,
                            $"Day {DaysPlayed} - World grows stronger..."
                        );
                    }
                }

                lastDayTime = Main.time;
            }

            // Update world phase based on world flags
            UpdateWorldPhase();
        }

        // ==================== BOSS DETECTION ====================

        /// <summary>
        /// Detects all bosses in the game (vanilla + mods)
        /// </summary>
        private void DetectBosses()
        {
            PreHardmodeBosses.Clear();
            HardmodeBosses.Clear();

            // Iterates over all registered NPCs
            for (int i = -65; i < NPCLoader.NPCCount; i++)
            {
                NPC npc = new NPC();
                npc.SetDefaults(i);

                // Checks if it's a boss
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
                Main.NewText(
                    $"[World Progression] Detected {PreHardmodeBosses.Count} Pre-Hardmode bosses",
                    Color.Cyan
                );
                Main.NewText(
                    $"[World Progression] Detected {HardmodeBosses.Count} Hardmode bosses",
                    Color.Cyan
                );
            }
        }

        /// <summary>
        /// Checks if a boss is pre-hardmode
        /// </summary>
        private bool IsPreHardmodeBoss(int npcID)
        {
            // Vanilla pre-hardmode
            if (npcID == NPCID.KingSlime)
                return true;
            if (npcID == NPCID.EyeofCthulhu)
                return true;
            if (npcID == NPCID.EaterofWorldsHead)
                return true;
            if (npcID == NPCID.BrainofCthulhu)
                return true;
            if (npcID == NPCID.QueenBee)
                return true;
            if (npcID == NPCID.SkeletronHead)
                return true;
            if (npcID == NPCID.Deerclops)
                return true;
            if (npcID == NPCID.WallofFlesh)
                return true;

            // Mod bosses: assumes bosses spawn in pre-hardmode if:
            // - Not in hardmode list
            // - And world is not in hardmode yet
            return false; // By default, assumes hardmode if not vanilla known
        }

        /// <summary>
        /// Checks if a boss is hardmode
        /// </summary>
        private bool IsHardmodeBoss(int npcID)
        {
            // Vanilla hardmode
            if (npcID == NPCID.QueenSlimeBoss)
                return true;
            if (npcID == NPCID.TheDestroyer)
                return true;
            if (npcID == NPCID.SkeletronPrime)
                return true;
            if (npcID == NPCID.Retinazer)
                return true; // Twins
            if (npcID == NPCID.Spazmatism)
                return true; // Twins
            if (npcID == NPCID.Plantera)
                return true;
            if (npcID == NPCID.Golem)
                return true;
            if (npcID == NPCID.DukeFishron)
                return true;
            if (npcID == NPCID.HallowBoss)
                return true; // Empress
            if (npcID == NPCID.CultistBoss)
                return true;
            if (npcID == NPCID.MoonLordCore)
                return true;

            // Pillars
            if (npcID == NPCID.LunarTowerSolar)
                return true;
            if (npcID == NPCID.LunarTowerVortex)
                return true;
            if (npcID == NPCID.LunarTowerNebula)
                return true;
            if (npcID == NPCID.LunarTowerStardust)
                return true;

            return false;
        }

        /// <summary>
        /// Checks if a boss is post-Moon Lord (life > Moon Lord)
        /// </summary>
        private bool IsPostMoonLordBoss(int npcID)
        {
            // Gets Moon Lord max life for comparison
            const int MOON_LORD_HP = 145000; // Normal mode life

            NPC testNPC = new NPC();
            testNPC.SetDefaults(npcID);

            // Boss is post-moon lord if it has more HP than Moon Lord
            return testNPC.boss && testNPC.lifeMax > MOON_LORD_HP;
        }

        // ==================== BOSS DEFEATED TRACKING ====================

        /// <summary>
        /// Registers defeated boss (called via GlobalNPC)
        /// </summary>
        public static void RegisterBossDefeat(int npcID)
        {
            // Ignores if already defeated before
            if (DefeatedBosses.Contains(npcID))
                return;

            DefeatedBosses.Add(npcID);
            RecalculateWorldLevel();

            // Visual notification (replaces chat messages)
            if (Main.netMode != NetmodeID.Server)
            {
                WorldLevelNotificationUI.Show(WorldLevel, "Enemies grow stronger...");
            }
        }

        // ==================== WORLD LEVEL CALCULATION ====================

        /// <summary>
        /// Recalculates world level based on defeated bosses or time
        /// </summary>
        public static void RecalculateWorldLevel()
        {
            var config = ModContent.GetInstance<Configs.WorldConfig>();

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
        /// Calculates level based on defeated bosses (default mode)
        /// Uses incremental progression: first boss gives 1, second gives 2, etc.
        /// </summary>
        private static void CalculateBossProgressionLevel()
        {
            var config = ModContent.GetInstance<Configs.WorldConfig>();

            int preHardmodeLevel = 0;
            int hardmodeLevel = 0;
            int postMoonLordLevel = 0;

            // ==================== PRE-HARDMODE ====================
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
                int hardDefeated = DefeatedBosses.Count(id =>
                    HardmodeBosses.Contains(id) && id != NPCID.MoonLordCore
                );
                int hardTotal = HardmodeBosses.Count - 1; // -1 para excluir Moon Lord
                int hardCap = config.HardmodeMaxLevel;

                hardmodeLevel = CalculateProgressiveLevel(hardDefeated, hardTotal, hardCap);
            }

            // ==================== POST-MOON LORD ====================
            if (CurrentPhase >= WorldPhase.PostMoonLord)
            {
                // Simply adds the post-moon lord cap
                postMoonLordLevel = config.PostMoonLordMaxLevel;
            }

            // Sum everything (cumulative)
            WorldLevel = preHardmodeLevel + hardmodeLevel + postMoonLordLevel;

            // Ensures minimum of 1
            if (WorldLevel < 1)
                WorldLevel = 1;
        }

        /// <summary>
        /// Calculates progressive level based on defeated bosses
        /// Formula: each boss gives more levels than the previous one
        ///
        /// Example: Cap 10, 4 bosses
        /// Boss 1: +1 (total: 1)
        /// Boss 2: +2 (total: 3)
        /// Boss 3: +3 (total: 6)
        /// Boss 4: +4 (total: 10)
        ///
        /// Sum: 1+2+3+4 = 10 = n(n+1)/2
        /// </summary>
        private static int CalculateProgressiveLevel(int defeated, int total, int cap)
        {
            if (total <= 0 || defeated <= 0)
                return 0;

            // Calculates increment for each boss
            // Sum from 1 to n = n(n+1)/2 = cap
            // So: increment[i] = (i * cap) / (total * (total+1) / 2)

            int totalSum = 0;
            for (int i = 1; i <= defeated; i++)
            {
                // Progressive increment scaled to reach cap
                float increment = (2f * cap * i) / (total * (total + 1));
                totalSum += (int)increment;
            }

            // Ensures it doesn't exceed cap
            return System.Math.Min(totalSum, cap);
        }

        /// <summary>
        /// Calculates level based on days played
        /// </summary>
        private static void CalculateTimeProgressionLevel()
        {
            WorldLevel = 1 + DaysPlayed;
        }

        // ==================== PHASE UPDATE ====================

        /// <summary>
        /// Updates world phase based on Terraria flags
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

            // Redetects bosses after loading
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

        // ==================== UTILITIES ====================

        /// <summary>
        /// Gets current cap based on phase
        /// </summary>
        public static int GetCurrentPhaseCap()
        {
            var config = ModContent.GetInstance<Configs.WorldConfig>();

            return CurrentPhase switch
            {
                WorldPhase.PreHardmode => config.PreHardmodeMaxLevel,
                WorldPhase.Hardmode => config.PreHardmodeMaxLevel + config.HardmodeMaxLevel,
                WorldPhase.PostMoonLord => config.PreHardmodeMaxLevel
                    + config.HardmodeMaxLevel
                    + config.PostMoonLordMaxLevel,
                _ => config.PreHardmodeMaxLevel,
            };
        }

        /// <summary>
        /// Gets NPC level with gaussian variance and spawn chance
        /// </summary>
        public static int GetNPCLevel()
        {
            var config = ModContent.GetInstance<Configs.WorldConfig>();

            if (config.WorldLevelMode == WorldLevelMode.Disabled)
                return 0;

            // Check spawn chance (now 1-100 instead of 0-1)
            int randomRoll = Main.rand.Next(1, 101); // 1 to 100
            if (randomRoll > config.LeveledEnemySpawnChance)
                return 0; // Enemy doesn't get a level

            // Applies variance: WorldLevel ± variance
            int variance = Main.rand.Next(-config.LevelVariance, config.LevelVariance + 1);
            int npcLevel = WorldLevel + variance;

            // Ensures it's not negative and doesn't exceed current cap
            int currentCap = GetCurrentPhaseCap();
            npcLevel = System.Math.Clamp(npcLevel, 1, currentCap);

            return npcLevel;
        }
    }
}
