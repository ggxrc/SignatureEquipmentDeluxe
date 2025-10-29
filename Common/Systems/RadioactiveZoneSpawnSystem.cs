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
        /// Modifica o spawn rate em zonas radioativas (2.5x)
        /// </summary>
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            // Verifica se jogador está em zona radioativa
            if (LeveledEnemySystem.IsInRadioactiveZone(player.Center, out int maxLevel))
            {
                // Reduz spawn rate = aumenta spawns (menor valor = mais spawns)
                spawnRate = (int)(spawnRate / 2.5f);
                
                // Aumenta número máximo de spawns
                maxSpawns = (int)(maxSpawns * 2.5f);
            }
        }
        
        /// <summary>
        /// 40% de chance extra de inimigos spawnarem com nível em zonas radioativas
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
                // 40% de chance extra de ter nível (além da chance base de 15%)
                if (Main.rand.NextFloat() < 0.40f)
                {
                    var leveledSystem = npc.GetGlobalNPC<LeveledEnemyGlobalNPC>();
                    if (leveledSystem != null && leveledSystem.EnemyLevel == 0)
                    {
                        // Aplica nível baseado na distância do centro
                        float distanceFromCenter = Vector2.Distance(npc.Center, zoneCenter);
                        float normalizedDistance = MathHelper.Clamp(distanceFromCenter / zoneRadius, 0f, 1f);
                        
                        float centerBias = 1f - normalizedDistance;
                        
                        int minLevel = (int)(maxLevel * (1f - normalizedDistance) * 0.5f) + 1;
                        int targetMaxLevel = (int)(maxLevel * (0.3f + centerBias * 0.7f)) + 1;
                        targetMaxLevel = (int)MathHelper.Clamp(targetMaxLevel, minLevel, maxLevel);
                        
                        int level = Main.rand.Next(minLevel, targetMaxLevel + 1);
                        
                        // Aplica o nível via SetLevelDirectly
                        leveledSystem.SetLevelDirectly(level, npc); // Passa NPC para aplicar scaling
                    }
                }
            }
        }
    }
}
