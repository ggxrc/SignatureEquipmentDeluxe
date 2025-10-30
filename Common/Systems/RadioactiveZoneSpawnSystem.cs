using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// GlobalNPC para modificar spawn rates e aplicar níveis extras em zonas radioativas
    /// </summary>
    public class RadioactiveZoneSpawnNPC : GlobalNPC
    {
        /// <summary>
        /// Modifica o spawn rate em zonas radioativas (1.5x)
        /// </summary>
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            // Verifica se jogador está em zona radioativa
            if (LeveledEnemySystem.IsInRadioactiveZone(player.Center, out int maxLevel))
            {
                // Reduz spawn rate = aumenta spawns (menor valor = mais spawns)
                spawnRate = (int)(spawnRate / 1.5f);
                
                // Aumenta número máximo de spawns
                maxSpawns = (int)(maxSpawns * 1.5f);
            }
        }
        
        /// <summary>
        /// TODO inimigo spawna com nível em zonas radioativas (mais baixos, notificação apenas para altos)
        /// </summary>
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            // Ignora NPCs amigáveis
            if (npc.friendly || npc.townNPC)
                return;
            
            // Ignora bonecos de treino e NPCs especiais
            if (npc.type == Terraria.ID.NPCID.TargetDummy || npc.type == Terraria.ID.NPCID.DungeonGuardian)
                return;
            
            // Verifica se spawnou em zona radioativa
            if (LeveledEnemySystem.IsInRadioactiveZone(npc.Center, out int maxLevel, out Vector2 zoneCenter, out float zoneRadius))
            {
                var leveledSystem = npc.GetGlobalNPC<LeveledEnemyGlobalNPC>();
                if (leveledSystem != null && leveledSystem.EnemyLevel == 0)
                {
                    // TODO inimigo ganha nível (100% chance)
                    // Distribuição: mais inimigos com nível baixo
                    float distanceFromCenter = Vector2.Distance(npc.Center, zoneCenter);
                    float normalizedDistance = MathHelper.Clamp(distanceFromCenter / zoneRadius, 0f, 1f);
                    
                    // Distribuição enviesada para níveis baixos
                    // 70% chance de nível baixo (1-40% do max), 30% chance de nível alto (41%-100% do max)
                    int level;
                    if (Main.rand.NextFloat() < 0.7f)
                    {
                        // Nível baixo: 1 até 40% do máximo
                        int maxLowLevel = (int)(maxLevel * 0.4f);
                        level = Main.rand.Next(1, Math.Max(2, maxLowLevel + 1));
                    }
                    else
                    {
                        // Nível alto: 41% até 100% do máximo
                        int minHighLevel = (int)(maxLevel * 0.41f) + 1;
                        level = Main.rand.Next(minHighLevel, maxLevel + 1);
                    }
                    
                    // Aplica o nível
                    leveledSystem.SetLevelDirectly(level, npc);
                    
                    // Notificação apenas para níveis altos (60%+ do máximo)
                    bool shouldNotify = level >= (int)(maxLevel * 0.6f);
                    if (shouldNotify)
                    {
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
                            CombatText.NewText(npc.Hitbox, Color.Lime, $"LEVEL {level}!", true, true);
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
        }
    }
}
