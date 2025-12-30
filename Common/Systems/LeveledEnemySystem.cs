using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// Sistema que gerencia inimigos com nível (leveled enemies)
    /// ZONA RADIOATIVA REMOVIDA - Sistema simplificado para uso futuro
    /// </summary>
    public class LeveledEnemySystem : ModSystem
    {
        // ZONA RADIOATIVA COMPLETAMENTE REMOVIDA DESTA VERSÃO
        // Inimigos nivelados permanecem como sistema disponível para uso futuro
        
        public override void ClearWorld()
        {
            // Nada a limpar - zonas radioativas foram removidas
        }
    }
    
    /// <summary>
    /// GlobalNPC que adiciona sistema de nível aos inimigos
    /// TODOS os inimigos recebem nível baseado na progressão do mundo
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
        /// Atribui nível ao NPC quando ele spawna
        /// </summary>
        public override void OnSpawn(NPC npc, Terraria.DataStructures.IEntitySource source)
        {
            // Ignora bonecos de treino, town NPCs, etc
            if (npc.friendly || npc.townNPC || npc.type == Terraria.ID.NPCID.TargetDummy)
                return;
            
            // Ignora bosses (eles não ganham nível)
            if (npc.boss)
                return;
            
            // Atribui nível baseado no mundo
            AssignWorldLevel(npc);
        }
        
        /// <summary>
        /// Atribui nível baseado na progressão do mundo
        /// </summary>
        private void AssignWorldLevel(NPC npc)
        {
            if (hasAssignedLevel)
                return;
            
            // Obtém nível do mundo com variância
            EnemyLevel = WorldProgressionSystem.GetNPCLevel();
            
            if (EnemyLevel > 0)
            {
                hasAssignedLevel = true;
                ApplyLevelScaling(npc);
            }
        }
        
        /// <summary>
        /// Aplica scaling baseado no nível do inimigo
        /// </summary>
        public void ApplyLevelScaling(NPC npc)
        {
            if (EnemyLevel <= 0 || hasAppliedScaling)
                return;
            
            // Salva valores base na primeira vez
            if (baseLifeMax == -1)
            {
                baseLifeMax = npc.lifeMax;
                baseDamage = npc.damage;
                baseDefense = npc.defense;
            }
            
            // +35% vida por nível
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
        /// Modifica o dano que o NPC causa ao jogador (com penetração baseada em nível)
        /// </summary>
        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (EnemyLevel <= 0)
                return;
            
            // +3% dano por nível
            float damageMultiplier = 1f + (EnemyLevel * 0.03f);
            modifiers.SourceDamage *= damageMultiplier;
            
            // Penetração baseada em diferença de nível vs armadura do player
            int totalArmorLevel = 0;
            int armorPieces = 0;
            
            for (int i = 0; i < 3; i++)
            {
                if (target.armor[i] != null && !target.armor[i].IsAir)
                {
                    var armorGlobal = target.armor[i].GetGlobalItem<GlobalItems.SignatureGlobalItem>();
                    if (armorGlobal != null)
                    {
                        totalArmorLevel += armorGlobal.Level;
                        armorPieces++;
                    }
                }
            }
            
            int averageArmorLevel = armorPieces > 0 ? totalArmorLevel / armorPieces : 0;
            int levelDifference = EnemyLevel - averageArmorLevel;
            
            // +2% penetração por nível de diferença (máximo 80%)
            if (levelDifference > 0)
            {
                float penetrationPercent = System.Math.Min(levelDifference * 0.02f, 0.80f);
                modifiers.ArmorPenetration += (int)(target.statDefense * penetrationPercent);
            }
        }
        
        /// <summary>
        /// Modifica o dano que o NPC recebe (resistência baseada em nível)
        /// </summary>
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (EnemyLevel <= 0)
                return;
            
            // Resistência baseada em diferença de nível vs arma do player
            Player attacker = Main.player[Main.myPlayer];
            if (attacker != null && attacker.active && attacker.HeldItem != null && !attacker.HeldItem.IsAir)
            {
                var weaponGlobal = attacker.HeldItem.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
                if (weaponGlobal != null)
                {
                    int weaponLevel = weaponGlobal.Level;
                    int levelDifference = EnemyLevel - weaponLevel;
                    
                    // 0% resist em igual nível, 100% resist em +20 níveis
                    if (levelDifference > 0)
                    {
                        float resistancePercent = System.Math.Min(levelDifference / 20f, 1.0f);
                        modifiers.FinalDamage *= (1f - resistancePercent);
                    }
                }
            }
            
            // -2% knockback por nível, máximo 100%
            float knockbackReduction = System.Math.Max(0f, 1f - (EnemyLevel * 0.02f));
            modifiers.Knockback *= knockbackReduction;
        }
        
        /// <summary>
        /// Quando NPC morre: Drop de runas E registra boss derrotado
        /// </summary>
        public override void OnKill(NPC npc)
        {
            // Registra boss derrotado para progressão de mundo
            if (npc.boss)
            {
                WorldProgressionSystem.RegisterBossDefeat(npc.type);
            }
            
            // Drop de runas para inimigos nivelados
            if (EnemyLevel > 0)
            {
                DropRandomRune(npc);
            }
        }
        
        /// <summary>
        /// Desenha o level acima da cabeça e aura verde
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
                        0f, 0f, 100,
                        new Color(100, 255, 100),
                        0.8f
                    );
                    dust.noGravity = true;
                    dust.velocity *= 0.3f;
                }
            }
        }
        
        /// <summary>
        /// Desenha "LVL X" acima da cabeça do inimigo
        /// </summary>
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (EnemyLevel <= 0)
                return;
            
            string levelText = $"LVL {EnemyLevel}";
            Vector2 textSize = Terraria.GameContent.FontAssets.MouseText.Value.MeasureString(levelText);
            Vector2 textPos = npc.Top - screenPos - new Vector2(textSize.X / 2f, 30f);
            
            Utils.DrawBorderString(
                spriteBatch,
                levelText,
                textPos,
                new Color(100, 255, 100),
                0.9f
            );
        }
        
        /// <summary>
        /// Calcula XP bônus ao matar inimigo com nível
        /// </summary>
        public float GetKillXPBonus()
        {
            if (EnemyLevel <= 0)
                return 1f;
            
            // +5% XP por nível
            return 1f + (EnemyLevel * 0.05f);
        }
        
        /// <summary>
        /// Define o nível diretamente
        /// </summary>
        public void SetLevelDirectly(int level, NPC npc)
        {
            EnemyLevel = level;
            hasAppliedScaling = false;
            
            // Salva stats base
            if (baseLifeMax == -1)
            {
                baseLifeMax = npc.lifeMax;
                baseDamage = npc.damage;
                baseDefense = npc.defense;
            }
            
            ApplyLevelScaling(npc);
        }
        
        private void DropRandomRune(NPC npc)
        {
            var config = ModContent.GetInstance<Configs.ServerConfig>();
            float dropChance = 0.05f + (EnemyLevel * 0.002f);
            
            if (Main.rand.NextFloat() < dropChance)
            {
                int runeType = Main.rand.Next(new int[] {
                    ModContent.ItemType<Content.Items.Runes.FireRune>(),
                    ModContent.ItemType<Content.Items.Runes.IceRune>(),
                    ModContent.ItemType<Content.Items.Runes.PoisonRune>(),
                    ModContent.ItemType<Content.Items.Runes.LightningRune>(),
                    ModContent.ItemType<Content.Items.Runes.AttackSpeedRune>(),
                    ModContent.ItemType<Content.Items.Runes.LifeRegenRune>(),
                    ModContent.ItemType<Content.Items.Runes.LifestealRune>()
                });
                
                Item.NewItem(npc.GetSource_Loot(), npc.getRect(), runeType);
            }
        }
    }
}
