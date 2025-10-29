using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// Sistema que gerencia inimigos com nível (leveled enemies)
    /// Inimigos ganham nível em regiões onde armas com maldição foram dropadas
    /// </summary>
    public class LeveledEnemySystem : ModSystem
    {
        // Lista de zonas radioativas (onde armas foram dropadas)
        public static List<RadioactiveZone> radioactiveZones = new List<RadioactiveZone>();
        
        /// <summary>
        /// Adiciona uma zona radioativa onde inimigos podem ganhar nível
        /// </summary>
        public static void AddRadioactiveZone(Vector2 position, int weaponLevel)
        {
            int maxEnemyLevel = weaponLevel; // 100% do nível da arma (era 50%)
            maxEnemyLevel = System.Math.Max(1, maxEnemyLevel); // Mínimo 1
            
            var config = ModContent.GetInstance<Configs.ServerConfig>();
            float baseRadius = (config?.RadioactiveZoneRadius ?? 150);
            float radius = baseRadius * 2.5f * 16f; // 150% maior = 2.5x, tiles para pixels
            int duration = (config?.RadioactiveZoneDurationMinutes ?? 30) * 60 * 60; // minutos para frames
            
            radioactiveZones.Add(new RadioactiveZone
            {
                Position = position,
                Radius = radius,
                MaxEnemyLevel = maxEnemyLevel,
                TimeLeft = duration
            });
            
            // Mensagem de debug
            Main.NewText($"Radioactive Zone created! Level: {maxEnemyLevel}, Radius: {radius / 16f} tiles", Color.Lime);
        }
        
        /// <summary>
        /// Verifica se uma posição está em zona radioativa
        /// </summary>
        public static bool IsInRadioactiveZone(Vector2 position, out int maxLevel)
        {
            maxLevel = 0;
            
            foreach (var zone in radioactiveZones)
            {
                float distance = Vector2.Distance(position, zone.Position);
                if (distance <= zone.Radius)
                {
                    maxLevel = System.Math.Max(maxLevel, zone.MaxEnemyLevel);
                }
            }
            
            return maxLevel > 0;
        }
        
        /// <summary>
        /// Verifica se posição está em zona radioativa e retorna informações da zona
        /// </summary>
        public static bool IsInRadioactiveZone(Vector2 position, out int maxLevel, out Vector2 zoneCenter, out float zoneRadius)
        {
            maxLevel = 0;
            zoneCenter = Vector2.Zero;
            zoneRadius = 0;
            
            foreach (var zone in radioactiveZones)
            {
                float distance = Vector2.Distance(position, zone.Position);
                if (distance <= zone.Radius)
                {
                    maxLevel = System.Math.Max(maxLevel, zone.MaxEnemyLevel);
                    if (zone.MaxEnemyLevel >= maxLevel)
                    {
                        zoneCenter = zone.Position;
                        zoneRadius = zone.Radius;
                    }
                }
            }
            
            return maxLevel > 0;
        }
        
        /// <summary>
        /// Atualiza zonas radioativas (remove expiradas e atualiza danger level)
        /// </summary>
        public override void PostUpdateEverything()
        {
            for (int i = radioactiveZones.Count - 1; i >= 0; i--)
            {
                radioactiveZones[i].TimeLeft--;
                radioactiveZones[i].UpdateDangerLevel(); // Atualiza nível de perigo
                
                if (radioactiveZones[i].TimeLeft <= 0)
                {
                    radioactiveZones.RemoveAt(i);
                }
            }
        }
        
        /// <summary>
        /// Limpa zonas ao descarregar mundo
        /// </summary>
        public override void ClearWorld()
        {
            radioactiveZones.Clear();
        }
        
        /// <summary>
        /// Classe que representa uma zona radioativa
        /// </summary>
        public class RadioactiveZone
        {
            public Vector2 Position { get; set; }
            public float Radius { get; set; }
            public int MaxEnemyLevel { get; set; }
            public int TimeLeft { get; set; }
            public int DangerLevel { get; set; } = 0; // 0-5, calculado dinamicamente
            private int dangerUpdateTimer = 0;
            private int previousDangerLevel = 0; // Para detectar mudanças
            
            /// <summary>
            /// Atualiza o nível de perigo baseado em quantos inimigos fortes existem
            /// </summary>
            public void UpdateDangerLevel()
            {
                dangerUpdateTimer++;
                if (dangerUpdateTimer < 300) return; // Atualiza a cada 5 segundos
                dangerUpdateTimer = 0;
                
                // Salva o nível anterior para detectar mudanças
                int oldDangerLevel = DangerLevel;
                
                // Conta quantos NPCs nivelados e fortes estão na zona
                int strongEnemyCount = 0;
                float threshold = MaxEnemyLevel * 0.5f; // 50% do nível máximo da zona
                
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.active || npc.friendly) continue;
                    
                    // Verifica se está na zona
                    float distance = Vector2.Distance(npc.Center, Position);
                    if (distance > Radius) continue;
                    
                    // Verifica nível
                    var leveledNPC = npc.GetGlobalNPC<LeveledEnemyGlobalNPC>();
                    if (leveledNPC != null && leveledNPC.EnemyLevel >= threshold)
                    {
                        strongEnemyCount++;
                    }
                }
                
                // Calcula danger level (0-5)
                // Tier 0: 0 inimigos fortes
                // Tier 1: 1-2 inimigos fortes
                // Tier 2: 3-5 inimigos fortes
                // Tier 3: 6-9 inimigos fortes
                // Tier 4: 10-14 inimigos fortes
                // Tier 5: 15+ inimigos fortes (APOCALIPSE)
                if (strongEnemyCount == 0)
                    DangerLevel = 0;
                else if (strongEnemyCount <= 2)
                    DangerLevel = 1;
                else if (strongEnemyCount <= 5)
                    DangerLevel = 2;
                else if (strongEnemyCount <= 9)
                    DangerLevel = 3;
                else if (strongEnemyCount <= 14)
                    DangerLevel = 4;
                else
                    DangerLevel = 5; // MAXIMUM DANGER
                
                // Se o danger level AUMENTOU, toca efeitos
                if (DangerLevel > oldDangerLevel)
                {
                    OnDangerLevelIncrease(oldDangerLevel, DangerLevel);
                }
                
                // Aplica modificadores baseado no danger level
                ApplyDangerModifiers();
            }
            
            /// <summary>
            /// Chamado quando o nível de perigo aumenta - efeitos visuais e sonoros
            /// </summary>
            private void OnDangerLevelIncrease(int oldLevel, int newLevel)
            {
                // Som crescente de perigo
                switch (newLevel)
                {
                    case 1:
                        Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.Roar with { Pitch = -0.5f, Volume = 0.5f }, Position);
                        break;
                    case 2:
                        Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.Roar with { Pitch = -0.3f, Volume = 0.7f }, Position);
                        break;
                    case 3:
                        Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.Roar with { Pitch = 0f, Volume = 0.9f }, Position);
                        break;
                    case 4:
                        Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.DD2_BetsyScream with { Volume = 1f }, Position);
                        break;
                    case 5:
                        Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.DD2_BetsyScream with { Volume = 1.2f }, Position);
                        Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.DD2_EtherianPortalOpen, Position);
                        break;
                }
                
                // Efeitos visuais - onda de choque pulsante
                int particleIntensity = 30 * newLevel; // Mais partículas em níveis mais altos
                for (int i = 0; i < particleIntensity; i++)
                {
                    float angle = (i / (float)particleIntensity) * MathHelper.TwoPi;
                    Vector2 pos = Position + new Vector2(
                        (float)System.Math.Cos(angle) * Radius * 0.8f,
                        (float)System.Math.Sin(angle) * Radius * 0.8f
                    );
                    
                    Color waveColor = GetDangerColor();
                    Dust wave = Dust.NewDustPerfect(pos, Terraria.ID.DustID.Electric, Vector2.Zero, 0, waveColor, 3f);
                    wave.noGravity = true;
                }
                
                // Explosão central
                for (int i = 0; i < 50; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2CircularEdge(8f, 8f);
                    Dust explosion = Dust.NewDustPerfect(Position, Terraria.ID.DustID.CursedTorch, velocity, 0, GetDangerColor(), 2.5f);
                    explosion.noGravity = true;
                }
                
                // Mensagem dramática
                if (Main.netMode != Terraria.ID.NetmodeID.Server)
                {
                    string message = newLevel switch
                    {
                        1 => "⚠ The zone's danger level rises to TIER 1",
                        2 => "⚠ The zone's danger level rises to TIER 2 - Caution!",
                        3 => "⚠⚠ The zone's danger level rises to TIER 3 - DANGEROUS!",
                        4 => "☢ The zone's danger level rises to TIER 4 - EXTREME DANGER!",
                        5 => "☢☢ The zone has reached TIER 5 - APOCALYPSE! ☢☢",
                        _ => ""
                    };
                    
                    Color messageColor = newLevel switch
                    {
                        1 => Color.Yellow,
                        2 => Color.Orange,
                        3 => Color.OrangeRed,
                        4 => Color.Red,
                        5 => Color.DarkRed,
                        _ => Color.White
                    };
                    
                    if (!string.IsNullOrEmpty(message))
                    {
                        Main.NewText(message, messageColor);
                    }
                }
            }
            
            /// <summary>
            /// Aplica modificadores de stats baseado no nível de perigo
            /// </summary>
            private void ApplyDangerModifiers()
            {
                if (DangerLevel == 0) return;
                
                // Cada tier: +40% maxLevel, +10% radius
                float levelMultiplier = 1f + (DangerLevel * 0.4f);
                float radiusMultiplier = 1f + (DangerLevel * 0.1f);
                
                // Aplica multiplicadores (mas guarda valores originais)
                // Isso será usado pelos sistemas de visual e spawn
            }
            
            /// <summary>
            /// Retorna multiplicador de partículas baseado no danger level
            /// </summary>
            public float GetParticleMultiplier()
            {
                // Cada tier: +50% partículas
                return 1f + (DangerLevel * 0.5f);
            }
            
            /// <summary>
            /// Retorna cor de partícula baseada no danger level
            /// </summary>
            public Color GetDangerColor()
            {
                return DangerLevel switch
                {
                    0 => Color.Green,
                    1 => Color.LightGreen,
                    2 => Color.Yellow,
                    3 => Color.Orange,
                    4 => Color.OrangeRed,
                    5 => Color.Red,
                    _ => Color.Green
                };
            }
        }
    }
    
    /// <summary>
    /// GlobalNPC que adiciona sistema de nível aos inimigos
    /// </summary>
    public class LeveledEnemyGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        
        public int EnemyLevel { get; set; } = 0;
        private bool hasSpawnedInRadioactiveZone = false;
        private bool hasCheckedForLevel = false;
        private int checkTimer = 0;
        
        // Sistema de permanência
        public bool isPermanent = false; // Inimigos nivelados não despawnam
        private Vector2 homeZoneCenter = Vector2.Zero;
        private float homeZoneRadius = 0;
        
        public override void AI(NPC npc)
        {
            // Ignora bonecos de treino e outros NPCs especiais
            if (npc.type == Terraria.ID.NPCID.TargetDummy || npc.type == Terraria.ID.NPCID.DungeonGuardian)
                return;
            
            // === AI AVANÇADA PARA INIMIGOS NIVELADOS ===
            if (EnemyLevel > 0 && hasSpawnedInRadioactiveZone && !npc.friendly)
            {
                // Marca como permanente
                if (!isPermanent)
                {
                    isPermanent = true;
                    if (IsInRadioactiveZone(npc.Center, out int _, out Vector2 center, out float radius))
                    {
                        homeZoneCenter = center;
                        homeZoneRadius = radius;
                    }
                }
                
                // Previne despawn
                npc.timeLeft = 600; // Mantém vivo
                
                // Sistema de confinamento à zona
                if (homeZoneCenter != Vector2.Zero)
                {
                    float distanceToCenter = Vector2.Distance(npc.Center, homeZoneCenter);
                    
                    // Se saiu da zona, empurra de volta
                    if (distanceToCenter > homeZoneRadius)
                    {
                        Vector2 directionToCenter = Vector2.Normalize(homeZoneCenter - npc.Center);
                        npc.velocity = directionToCenter * npc.velocity.Length();
                    }
                }
            }
            
            // Velocidade não aumenta mais com nível
            
            // Ignora NPCs que não são inimigos ou que já têm nível (verificação de level-up)
            if (npc.friendly || npc.townNPC || npc.lifeMax <= 5 || hasCheckedForLevel)
                return;
            
            // Ignora bonecos de treino e NPCs especiais para ganhar nível
            if (npc.type == Terraria.ID.NPCID.TargetDummy || npc.type == Terraria.ID.NPCID.DungeonGuardian)
                return;
            
            // Verifica a cada 30 frames (0.5 segundos)
            checkTimer++;
            if (checkTimer < 30)
                return;
            
            checkTimer = 0;
            hasCheckedForLevel = true;
            
            // Verifica se está em zona radioativa
            if (IsInRadioactiveZone(npc.Center, out int maxLevel, out Vector2 zoneCenter, out float zoneRadius))
            {
                var config = ModContent.GetInstance<Configs.ServerConfig>();
                float spawnChance = config?.LeveledEnemySpawnChance ?? 0.15f;
                
                // Chance de ganhar nível
                if (Main.rand.NextFloat() < spawnChance)
                {
                    // Calcula a distância do centro (0.0 = centro, 1.0 = borda)
                    float distanceFromCenter = Vector2.Distance(npc.Center, zoneCenter);
                    float normalizedDistance = MathHelper.Clamp(distanceFromCenter / zoneRadius, 0f, 1f);
                    
                    // Quanto mais perto do centro, maior a chance de níveis altos
                    // No centro: 100% de chance de nível máximo
                    // Na borda: níveis mais baixos
                    float centerBias = 1f - normalizedDistance; // 1.0 no centro, 0.0 na borda
                    
                    // Calcula o nível baseado na distância
                    // Centro favorece níveis altos, borda favorece níveis baixos
                    int minLevel = (int)(maxLevel * (1f - normalizedDistance) * 0.5f) + 1;
                    int targetMaxLevel = (int)(maxLevel * (0.3f + centerBias * 0.7f)) + 1;
                    targetMaxLevel = (int)MathHelper.Clamp(targetMaxLevel, minLevel, maxLevel);
                    
                    EnemyLevel = Main.rand.Next(minLevel, targetMaxLevel + 1);
                    hasSpawnedInRadioactiveZone = true;
                    ApplyLevelScaling(npc);
                    
                    // Verifica se está visível na tela para animação dramática
                    bool isOnScreen = npc.Hitbox.Intersects(new Rectangle(
                        (int)Main.screenPosition.X,
                        (int)Main.screenPosition.Y,
                        Main.screenWidth,
                        Main.screenHeight
                    ));
                    
                    if (isOnScreen)
                    {
                        // ANIMAÇÃO DRAMÁTICA NA TELA
                        // Explosão massiva de partículas
                        for (int i = 0; i < 40; i++)
                        {
                            Vector2 velocity = Main.rand.NextVector2CircularEdge(5f, 5f);
                            Dust dust = Dust.NewDustPerfect(
                                npc.Center,
                                Terraria.ID.DustID.GreenTorch,
                                velocity,
                                0,
                                new Color(100, 255, 100),
                                Main.rand.NextFloat(1.5f, 2.5f)
                            );
                            dust.noGravity = true;
                        }
                        
                        // Onda de choque visual
                        for (int i = 0; i < 20; i++)
                        {
                            float angle = MathHelper.TwoPi * i / 20f;
                            Vector2 velocity = new Vector2(
                                (float)System.Math.Cos(angle) * 8f,
                                (float)System.Math.Sin(angle) * 8f
                            );
                            Dust shockwave = Dust.NewDustPerfect(
                                npc.Center,
                                Terraria.ID.DustID.Electric,
                                velocity,
                                0,
                                Color.Lime,
                                2f
                            );
                            shockwave.noGravity = true;
                        }
                        
                        // Som dramático de power-up
                        Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.Roar, npc.Center);
                        
                        // Texto flutuante
                        CombatText.NewText(npc.Hitbox, Color.Lime, $"LEVEL {EnemyLevel}!", true, true);
                    }
                    else
                    {
                        // SOM DE ALERTA FORA DA TELA
                        // Som sinistro indicando perigo
                        Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.Roar with { 
                            Volume = 0.5f,
                            Pitch = -0.5f
                        });
                        
                        // Mensagem de aviso
                        if (Main.netMode != Terraria.ID.NetmodeID.Server)
                        {
                            Main.NewText("A powerful presence awakens nearby...", new Color(100, 255, 100));
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Aplica scaling baseado no nível do inimigo
        /// </summary>
        public void ApplyLevelScaling(NPC npc)
        {
            if (EnemyLevel <= 0)
                return;
            
            // Salva valores originais para referência futura
            if (npc.defDamage == 0)
                npc.defDamage = npc.damage;
            if (npc.defDefense == 0)
                npc.defDefense = npc.defense;
            
            // +75% vida por nível (balanceamento: 15% -> 75%)
            float hpMultiplier = 1f + (EnemyLevel * 0.75f);
            int originalMaxLife = npc.lifeMax;
            npc.lifeMax = (int)(npc.lifeMax * hpMultiplier);
            npc.life = npc.lifeMax;
            
            // Outros stats serão aplicados dinamicamente via hooks
        }
        
        /// <summary>
        /// Modifica o dano que o NPC causa ao jogador (com penetração baseada em nível)
        /// </summary>
        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (EnemyLevel <= 0 || !hasSpawnedInRadioactiveZone)
                return;
            
            // +2% dano por nível (dobrado: 1% -> 2%)
            float damageMultiplier = 1f + (EnemyLevel * 0.02f);
            modifiers.SourceDamage *= damageMultiplier;
            
            // NOVO SISTEMA: Penetração baseada em diferença de nível vs armadura do player
            // Calcula nível médio da armadura do player
            int totalArmorLevel = 0;
            int armorPieces = 0;
            
            for (int i = 0; i < 3; i++) // Head, Body, Legs
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
            
            // Se inimigo é mais forte que a armadura, ignora defesa
            // +2% penetração por nível de diferença (máximo 80% em +40 níveis)
            if (levelDifference > 0)
            {
                float penetrationPercent = System.Math.Min(levelDifference * 0.02f, 0.80f);
                modifiers.ArmorPenetration += (int)(target.statDefense * penetrationPercent);
                
                if (Main.netMode != Terraria.ID.NetmodeID.Server && Main.rand.NextBool(120))
                {
                    Main.NewText($"Enemy lvl {EnemyLevel} vs Armor lvl {averageArmorLevel} = {penetrationPercent:P0} pen!", Color.Orange);
                }
            }
        }
        
        /// <summary>
        /// Modifica o dano que o NPC recebe (resistência baseada em nível)
        /// </summary>
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (EnemyLevel <= 0 || !hasSpawnedInRadioactiveZone)
                return;
            
            // NOVO SISTEMA: Resistência baseada em diferença de nível vs arma do player
            // Obtém o jogador que está atacando
            Player attacker = Main.player[Main.myPlayer];
            if (attacker != null && attacker.active && attacker.HeldItem != null && !attacker.HeldItem.IsAir)
            {
                var weaponGlobal = attacker.HeldItem.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
                if (weaponGlobal != null)
                {
                    int weaponLevel = weaponGlobal.Level;
                    int levelDifference = EnemyLevel - weaponLevel;
                    
                    // CORRIGIDO: 0% resist em igual nível, 100% resist em +20 níveis
                    // Fórmula: resistance = (levelDiff / 20) capped at 100%
                    if (levelDifference > 0)
                    {
                        float resistancePercent = System.Math.Min(levelDifference / 20f, 1.0f);
                        modifiers.FinalDamage *= (1f - resistancePercent);
                        
                        if (Main.netMode != Terraria.ID.NetmodeID.Server && Main.rand.NextBool(120))
                        {
                            Main.NewText($"Enemy lvl {EnemyLevel} vs Weapon lvl {weaponLevel} = {resistancePercent:P0} resist!", Color.Cyan);
                        }
                    }
                    // Se arma é mais forte, dano normal (sem bônus)
                }
            }
            
            // -1% knockback por nível (dobrado: 0.5% -> 1%)
            float knockbackReduction = 1f - (EnemyLevel * 0.01f);
            modifiers.Knockback *= knockbackReduction;
        }
        
        /// <summary>
        /// Quando NPC morre: Drop de runas e recompensa XP para player
        /// </summary>
        public override void OnKill(NPC npc)
        {
            if (EnemyLevel > 0 && hasSpawnedInRadioactiveZone)
            {
                // Drop normal de runas
                DropRunesOnDeath(npc);
                
                // Verifica se foi morto por player
                if (npc.lastInteraction != 255)
                {
                    // Recompensa de XP por matar inimigo nivelado (2% do XP total acumulado)
                    if (EnemyLevel > 1) // Apenas se tem nível significativo
                    {
                        Player killer = Main.player[npc.lastInteraction];
                        if (killer != null && killer.active)
                        {
                            // Calcula XP total que este inimigo acumulou (do nível 1 até o nível atual)
                            int totalXP = CalculateTotalXPForLevel(EnemyLevel);
                            int bonusXP = System.Math.Max(1, (int)(totalXP * 0.02f)); // 2% do total
                            
                            // Dá o XP para a arma equipada do jogador
                            if (killer.HeldItem != null && !killer.HeldItem.IsAir)
                            {
                                var globalItem = killer.HeldItem.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
                                if (globalItem != null && globalItem.Level > 0)
                                {
                                    // Adiciona XP diretamente
                                    globalItem.Experience += bonusXP;
                                    
                                    // Verifica level up
                                    int requiredXP = globalItem.GetRequiredXP(globalItem.Level);
                                    bool leveledUp = false;
                                    
                                    while (globalItem.Experience >= requiredXP && globalItem.Level < 100)
                                    {
                                        globalItem.Experience -= requiredXP;
                                        globalItem.Level++;
                                        requiredXP = globalItem.GetRequiredXP(globalItem.Level);
                                        leveledUp = true;
                                    }
                                    
                                    // Feedback visual épico
                                    CombatText.NewText(npc.Hitbox, Color.Gold, $"+{bonusXP} XP BONUS!", true, true);
                                    
                                    if (leveledUp)
                                    {
                                        // Som especial ao upar
                                        Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.Item4, killer.Center);
                                        CombatText.NewText(killer.Hitbox, Color.Cyan, "LEVEL UP!", true, true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Drop de runas ao morrer (quando morto por player)
        /// </summary>
        private void DropRunesOnDeath(NPC npc)
        {
            // Chance de dropar runa baseado no nível
            float dropChance = 0.05f + (EnemyLevel * 0.002f); // 5% base + 0.2% por nível
            
            if (Main.rand.NextFloat() < dropChance)
            {
                DropRandomRune(npc);
            }
        }
        
        /// <summary>
        /// Desenha o level acima da cabeça e aura verde
        /// </summary>
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (EnemyLevel <= 0 || !hasSpawnedInRadioactiveZone)
                return;
            
            // Spawn de partículas de aura verde (dobrado: 20% -> 40%)
            if (Main.rand.NextBool(5)) // 20% de chance por frame
            {
                // Spawn 2 partículas ao invés de 1 (100% mais partículas)
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
            if (EnemyLevel <= 0 || !hasSpawnedInRadioactiveZone)
                return;
            
            string levelText = $"LVL {EnemyLevel}";
            Vector2 textSize = Terraria.GameContent.FontAssets.MouseText.Value.MeasureString(levelText);
            Vector2 textPos = npc.Top - screenPos - new Vector2(textSize.X / 2f, 30f);
            
            // Sombra
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
        /// Calcula o XP total acumulado do nível 1 até o nível especificado
        /// </summary>
        private int CalculateTotalXPForLevel(int targetLevel)
        {
            int totalXP = 0;
            
            // Cria instância temporária de SignatureGlobalItem para usar GetRequiredXP
            var tempItem = new Item();
            tempItem.SetDefaults(Terraria.ID.ItemID.CopperShortsword);
            var globalItem = tempItem.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
            
            // Soma o XP necessário de cada nível (1 até targetLevel)
            for (int i = 1; i <= targetLevel; i++)
            {
                totalXP += globalItem.GetRequiredXP(i);
            }
            
            return totalXP;
        }
        
        private void DropRandomRune(NPC npc)
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
        
        /// <summary>
        /// Verifica se posição está em zona radioativa
        /// </summary>
        private bool IsInRadioactiveZone(Vector2 position, out int maxLevel)
        {
            return LeveledEnemySystem.IsInRadioactiveZone(position, out maxLevel);
        }
        
        /// <summary>
        /// Verifica se posição está em zona radioativa com informações detalhadas
        /// </summary>
        private bool IsInRadioactiveZone(Vector2 position, out int maxLevel, out Vector2 zoneCenter, out float zoneRadius)
        {
            return LeveledEnemySystem.IsInRadioactiveZone(position, out maxLevel, out zoneCenter, out zoneRadius);
        }
        
        /// <summary>
        /// Define o nível diretamente (usado quando killer absorve poder)
        /// </summary>
        public void SetLevelDirectly(int level, NPC npc)
        {
            EnemyLevel = level;
            hasSpawnedInRadioactiveZone = true;
            hasCheckedForLevel = true;
            isPermanent = true; // Marca como permanente
            
            // Define zona home para confinamento
            if (IsInRadioactiveZone(npc.Center, out int _, out Vector2 center, out float radius))
            {
                homeZoneCenter = center;
                homeZoneRadius = radius;
            }
            
            // TRUQUE: Finge que coletou dinheiro para prevenir despawn total
            npc.playerInteraction[Main.myPlayer] = true;
            
            // CRITICAL: Aplica o scaling de stats
            ApplyLevelScaling(npc);
        }
        
        /// <summary>
        /// Adiciona efeito elemental ao NPC (herda de runas)
        /// </summary>
        public void AddElementalEffect(Data.RuneType runeType, int runeLevel)
        {
            // Implementar efeitos visuais baseados nas runas
            // Por enquanto, apenas marca que tem efeito
        }
    }
}
