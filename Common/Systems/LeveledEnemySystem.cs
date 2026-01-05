using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Progression.Common.Systems
{
    /// <summary>
    /// GlobalNPC that adds level system to enemies
    /// ALL enemies receive level based on world progression
    /// </summary>
    public class LeveledEnemyGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int EnemyLevel { get; set; } = 0;
        private bool hasAppliedScaling = false;
        private bool hasAssignedLevel = false;

        // Stats base para evitar stacking
        private int baseLifeMax = -1;
        private int baseDamage = -1;
        private int baseDefense = -1;

        /// <summary>
        /// Assigns level to NPC when it spawns
        /// </summary>
        public override void OnSpawn(NPC npc, Terraria.DataStructures.IEntitySource source)
        {
            // Check if system is enabled
            var config = ModContent.GetInstance<Configs.WorldConfig>();
            if (!config.EnableLeveledEnemies)
                return;

            // Ignores training dummies, town NPCs, etc
            if (npc.friendly || npc.townNPC || npc.type == Terraria.ID.NPCID.TargetDummy)
                return;

            // Ignores bosses (they don't gain levels)
            if (npc.boss)
                return;

            // Assigns level based on world
            AssignWorldLevel(npc);
        }

        /// <summary>
        /// Assigns level based on world progression
        /// </summary>
        private void AssignWorldLevel(NPC npc)
        {
            if (hasAssignedLevel)
                return;

            // Gets world level with variance
            EnemyLevel = WorldProgressionSystem.GetNPCLevel();

            if (EnemyLevel > 0)
            {
                hasAssignedLevel = true;
                ApplyLevelScaling(npc);
            }
        }

        /// <summary>
        /// Applies scaling based on enemy level
        /// </summary>
        public void ApplyLevelScaling(NPC npc)
        {
            if (EnemyLevel <= 0 || hasAppliedScaling)
                return;

            // Saves base values on first time
            if (baseLifeMax == -1)
            {
                baseLifeMax = npc.lifeMax;
                baseDamage = npc.damage;
                baseDefense = npc.defense;
            }

            // +35% health per level
            float hpMultiplier = 1f + (EnemyLevel * 0.35f);
            npc.lifeMax = (int)(baseLifeMax * hpMultiplier);
            npc.life = npc.lifeMax;

            hasAppliedScaling = true;

            // Efeito visual de level up
            if (Main.netMode != Terraria.ID.NetmodeID.Server)
            {
                for (int i = 0; i < 20; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2CircularEdge(3f, 3f);
                    Dust dust = Dust.NewDustPerfect(
                        npc.Center,
                        Terraria.ID.DustID.GreenTorch,
                        velocity,
                        0,
                        new Color(100, 255, 100),
                        1.5f
                    );
                    dust.noGravity = true;
                }

                CombatText.NewText(npc.Hitbox, Color.Lime, $"LEVEL {EnemyLevel}!", true, false);
            }
        }

        /// <summary>
        /// Modifies damage the NPC deals to player (with level-based penetration)
        /// </summary>
        public override void ModifyHitPlayer(
            NPC npc,
            Player target,
            ref Player.HurtModifiers modifiers
        )
        {
            if (EnemyLevel <= 0)
                return;

            // +3% damage per level
            float damageMultiplier = 1f + (EnemyLevel * 0.03f);
            modifiers.SourceDamage *= damageMultiplier;

            // Penetration based on level difference vs player armor
            int totalArmorLevel = 0;
            int armorPieces = 0;

            for (int i = 0; i < 3; i++)
            {
                if (target.armor[i] != null && !target.armor[i].IsAir)
                {
                    var armorGlobal = target
                        .armor[i]
                        .GetGlobalItem<GlobalItems.SignatureGlobalItem>();
                    if (armorGlobal != null)
                    {
                        totalArmorLevel += armorGlobal.Level;
                        armorPieces++;
                    }
                }
            }

            int averageArmorLevel = armorPieces > 0 ? totalArmorLevel / armorPieces : 0;
            int levelDifference = EnemyLevel - averageArmorLevel;

            // +2% penetration per level difference (maximum 80%)
            if (levelDifference > 0)
            {
                float penetrationPercent = System.Math.Min(levelDifference * 0.02f, 0.80f);
                modifiers.ArmorPenetration += (int)(target.statDefense * penetrationPercent);
            }
        }

        /// <summary>
        /// Modifies damage the NPC receives (level-based resistance)
        /// </summary>
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (EnemyLevel <= 0)
                return;

            // Resistance based on level difference vs player weapon
            Player attacker = Main.player[Main.myPlayer];
            if (
                attacker != null
                && attacker.active
                && attacker.HeldItem != null
                && !attacker.HeldItem.IsAir
            )
            {
                var weaponGlobal =
                    attacker.HeldItem.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
                if (weaponGlobal != null)
                {
                    int weaponLevel = weaponGlobal.Level;
                    int levelDifference = EnemyLevel - weaponLevel;

                    // 0% resist at equal level, 100% resist at +20 levels
                    if (levelDifference > 0)
                    {
                        float resistancePercent = System.Math.Min(levelDifference / 20f, 1.0f);
                        modifiers.FinalDamage *= (1f - resistancePercent);
                    }
                }
            }

            // -2% knockback per level, maximum 100%
            float knockbackReduction = System.Math.Max(0f, 1f - (EnemyLevel * 0.02f));
            modifiers.Knockback *= knockbackReduction;
        }

        /// <summary>
        /// Quando NPC morre: registra boss derrotado
        /// </summary>
        public override void OnKill(NPC npc)
        {
            // Registers defeated boss for world progression
            if (npc.boss)
            {
                WorldProgressionSystem.RegisterBossDefeat(npc.type);
            }
        }

        /// <summary>
        /// Draws level above head and green aura
        /// </summary>
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (EnemyLevel <= 0)
                return;

            // Aura verde
            if (Main.rand.NextBool(5))
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust dust = Dust.NewDustDirect(
                        npc.position,
                        npc.width,
                        npc.height,
                        Terraria.ID.DustID.GreenTorch,
                        0f,
                        0f,
                        100,
                        new Color(100, 255, 100),
                        0.8f
                    );
                    dust.noGravity = true;
                    dust.velocity *= 0.3f;
                }
            }
        }

        /// <summary>
        /// Draws "LVL X" above enemy head
        /// </summary>
        public override void PostDraw(
            NPC npc,
            SpriteBatch spriteBatch,
            Vector2 screenPos,
            Color drawColor
        )
        {
            if (EnemyLevel <= 0)
                return;

            string levelText = $"LVL {EnemyLevel}";
            Vector2 textSize = Terraria.GameContent.FontAssets.MouseText.Value.MeasureString(
                levelText
            );
            Vector2 textPos = npc.Top - screenPos - new Vector2(textSize.X / 2f, 30f);

            Utils.DrawBorderString(spriteBatch, levelText, textPos, new Color(100, 255, 100), 0.9f);
        }

        /// <summary>
        /// Calculates bonus XP when killing leveled enemy
        /// </summary>
        public float GetXPMultiplier()
        {
            if (EnemyLevel <= 0)
                return 1f;

            var config = ModContent.GetInstance<Configs.WorldConfig>();

            // Bonus per level from config (in %)
            float bonusPerLevel = config.LeveledEnemyXPBonusPerLevel / 100f;
            return 1f + (EnemyLevel * bonusPerLevel);
        }

        /// <summary>
        /// Sets level directly
        /// </summary>
        public void SetLevelDirectly(int level, NPC npc)
        {
            EnemyLevel = level;
            hasAppliedScaling = false;

            // Saves base stats
            if (baseLifeMax == -1)
            {
                baseLifeMax = npc.lifeMax;
                baseDamage = npc.damage;
                baseDefense = npc.defense;
            }

            ApplyLevelScaling(npc);
        }
    }
}
