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
            int maxEnemyLevel = (int)(weaponLevel * 0.5f); // 50% do nível da arma
            maxEnemyLevel = System.Math.Max(1, maxEnemyLevel); // Mínimo 1
            
            var config = ModContent.GetInstance<Configs.ServerConfig>();
            float radius = (config?.RadioactiveZoneRadius ?? 150) * 16f; // tiles para pixels
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
        /// Atualiza zonas radioativas (remove expiradas)
        /// </summary>
        public override void PostUpdateEverything()
        {
            for (int i = radioactiveZones.Count - 1; i >= 0; i--)
            {
                radioactiveZones[i].TimeLeft--;
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
        
        public override void AI(NPC npc)
        {
            // Ignora NPCs que não são inimigos ou que já têm nível
            if (npc.friendly || npc.townNPC || npc.lifeMax <= 5 || hasCheckedForLevel)
                return;
            
            // Verifica a cada 30 frames (0.5 segundos)
            checkTimer++;
            if (checkTimer < 30)
                return;
            
            checkTimer = 0;
            hasCheckedForLevel = true;
            
            // Verifica se está em zona radioativa
            if (IsInRadioactiveZone(npc.Center, out int maxLevel))
            {
                var config = ModContent.GetInstance<Configs.ServerConfig>();
                float spawnChance = config?.LeveledEnemySpawnChance ?? 0.15f;
                
                // Chance de ganhar nível
                if (Main.rand.NextFloat() < spawnChance)
                {
                    EnemyLevel = Main.rand.Next(1, maxLevel + 1);
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
        private void ApplyLevelScaling(NPC npc)
        {
            if (EnemyLevel <= 0)
                return;
            
            // +0.25% vida por nível
            float hpMultiplier = 1f + (EnemyLevel * 0.0025f);
            npc.lifeMax = (int)(npc.lifeMax * hpMultiplier);
            npc.life = npc.lifeMax;
            
            // +0.1% dano por nível
            float damageMultiplier = 1f + (EnemyLevel * 0.001f);
            npc.damage = (int)(npc.damage * damageMultiplier);
            
            // +0.04% velocidade por nível (não aplicar em NPCs muito lentos)
            if (npc.velocity.Length() > 0.5f)
            {
                float speedMultiplier = 1f + (EnemyLevel * 0.0004f);
                npc.velocity *= speedMultiplier;
            }
        }
        
        /// <summary>
        /// Desenha o level acima da cabeça e aura verde
        /// </summary>
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (EnemyLevel <= 0 || !hasSpawnedInRadioactiveZone)
                return;
            
            // Spawn de partículas de aura verde
            if (Main.rand.NextBool(5)) // 20% de chance por frame
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
        /// Dropa runas ao morrer (inimigos com nível)
        /// </summary>
        public override void OnKill(NPC npc)
        {
            if (EnemyLevel > 0 && hasSpawnedInRadioactiveZone)
            {
                // Chance de dropar runa baseado no nível
                float dropChance = 0.05f + (EnemyLevel * 0.002f); // 5% base + 0.2% por nível
                
                if (Main.rand.NextFloat() < dropChance)
                {
                    DropRandomRune(npc);
                }
            }
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
        /// Define o nível diretamente (usado quando killer absorve poder)
        /// </summary>
        public void SetLevelDirectly(int level)
        {
            EnemyLevel = level;
            hasSpawnedInRadioactiveZone = true;
            hasCheckedForLevel = true;
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
