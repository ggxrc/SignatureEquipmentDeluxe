using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace SignatureEquipmentDeluxe.Common.Projectiles
{
    /// <summary>
    /// GlobalProjectile que modifica o comportamento de projéteis baseado no level da arma
    /// </summary>
    public class SignatureGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        // Referências ao item criador e player
        public Item ProjectileCreatorItem;
        public Player ProjectileCreatorPlayer;
        
        // Valores padrão para restauração
        private float defaultMinionSlots;
        private int defaultWidth;
        private int defaultHeight;
        private float defaultScale;

        /// <summary>
        /// Armazena os stats calculados para projéteis que spawnam múltiplas cópias
        /// </summary>
        private ProjectileStatsCache statsCache;

        /// <summary>
        /// Modifica projéteis baseado no level da arma que os criou
        /// </summary>
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (projectile == null || source == null)
                return;

            // Salva valores padrão
            defaultMinionSlots = projectile.minionSlots;
            defaultWidth = projectile.width;
            defaultHeight = projectile.height;
            defaultScale = projectile.scale;

            // Caso 1: Item usado diretamente (EntitySource_ItemUse)
            if (source is EntitySource_ItemUse itemUseSource && itemUseSource.Item != null && !itemUseSource.Item.IsAir)
            {
                ProjectileCreatorPlayer = itemUseSource.Player;
                ProjectileCreatorItem = itemUseSource.Item;
                
                statsCache = new ProjectileStatsCache();
                ApplyProjectileStats(projectile, true);
            }
            // Caso 2: Spawn por outro projectile (ex: minion que spawna projéteis)
            else if (source is EntitySource_Parent parentSource)
            {
                if (parentSource.Entity is Projectile parentProj)
                {
                    // Herda as informações do projectile pai
                    var parentGlobalProj = parentProj.GetGlobalProjectile<SignatureGlobalProjectile>();
                    
                    if (defaultMinionSlots == 0)
                        defaultMinionSlots = parentGlobalProj.defaultMinionSlots;
                    
                    ProjectileCreatorPlayer = parentGlobalProj.ProjectileCreatorPlayer;
                    ProjectileCreatorItem = parentGlobalProj.ProjectileCreatorItem;
                    
                    // Se é do mesmo tipo, reutiliza os stats calculados
                    if (projectile.type == parentProj.type && parentGlobalProj.statsCache != null)
                    {
                        ApplyCachedStats(projectile, parentGlobalProj.statsCache);
                    }
                    else
                    {
                        statsCache = new ProjectileStatsCache();
                        ApplyProjectileStats(projectile, false);
                    }
                }
                else if (parentSource.Entity is Player player)
                {
                    ProjectileCreatorPlayer = player;
                    
                    // Se é minion/sentry, pega o item equipado
                    if (projectile.IsMinionOrSentryRelated)
                    {
                        ProjectileCreatorItem = player.HeldItem;
                        statsCache = new ProjectileStatsCache();
                        ApplyProjectileStats(projectile, false);
                    }
                }
            }
            // Caso 3: Spawn por fonte misc (alguns minions especiais)
            else if (source is EntitySource_Misc && projectile.IsMinionOrSentryRelated && projectile.owner >= 0 && projectile.owner < Main.maxPlayers)
            {
                Player player = Main.player[projectile.owner];
                if (player != null && player.active)
                {
                    ProjectileCreatorPlayer = player;
                    ProjectileCreatorItem = player.HeldItem;
                    
                    statsCache = new ProjectileStatsCache();
                    ApplyProjectileStats(projectile, false);
                }
            }
        }

        /// <summary>
        /// Aplica todos os stats de projétil baseado no item criador
        /// </summary>
        private void ApplyProjectileStats(Projectile projectile, bool isDirectSpawn)
        {
            // Verifica se tem item criador válido
            if (ProjectileCreatorItem == null || ProjectileCreatorItem.IsAir)
                return;

            var globalItem = ProjectileCreatorItem.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
            if (globalItem == null || globalItem.Level <= 0)
                return;

            var config = ModContent.GetInstance<Configs.ServerConfig>();
            if (config == null)
                return;

            // 1. PROJECTILE SIZE (não para minions/sentries)
            if (!projectile.IsMinionOrSentryRelated)
            {
                float sizeBonus = globalItem.GetProjectileSizeCapped(ProjectileCreatorItem);
                if (sizeBonus > 0)
                {
                    float sizeScale = 1f + (sizeBonus / 100f);
                    
                    int newWidth = (int)(defaultWidth * sizeScale);
                    int newHeight = (int)(defaultHeight * sizeScale);
                    
                    projectile.width = newWidth;
                    projectile.height = newHeight;
                    projectile.scale = defaultScale * sizeScale;
                    
                    statsCache.SizeScale = sizeScale;
                    
                    if (config.DebugMode)
                        Main.NewText($"[DEBUG] Size: {defaultWidth}x{defaultHeight} -> {newWidth}x{newHeight} (scale: {sizeScale:F2})");
                }
            }

            // 2. PROJECTILE SPEED
            float speedBonus = globalItem.GetProjectileSpeedCapped(ProjectileCreatorItem);
            if (speedBonus > 0)
            {
                float speedMult = 1f + (speedBonus / 100f);
                projectile.velocity *= speedMult;
                
                statsCache.SpeedMultiplier = speedMult;
                
                if (config.DebugMode)
                    Main.NewText($"[DEBUG] Speed: x{speedMult:F2} ({speedBonus:F1}% faster)");
            }

            // 3. PROJECTILE LIFETIME (não para minions/sentries)
            if (!projectile.IsMinionOrSentryRelated && projectile.timeLeft > 0)
            {
                float lifeTimeBonus = globalItem.GetProjectileLifeTimeCapped(ProjectileCreatorItem);
                if (lifeTimeBonus > 0)
                {
                    int addedTime = (int)lifeTimeBonus;
                    projectile.timeLeft += addedTime;
                    
                    statsCache.AddedLifeTime = addedTime;
                    
                    if (config.DebugMode)
                        Main.NewText($"[DEBUG] Lifetime: +{addedTime} ticks");
                }
            }

            // 4. PROJECTILE PENETRATION
            float penetrationBonus = globalItem.GetProjectilePenetrationCapped(ProjectileCreatorItem);
            if (penetrationBonus > 0 && projectile.penetrate >= 0)
            {
                int addedPenetrate = (int)penetrationBonus;
                projectile.penetrate += addedPenetrate;
                
                statsCache.AddedPenetration = addedPenetrate;
                
                if (config.DebugMode)
                    Main.NewText($"[DEBUG] Penetration: +{addedPenetrate}");
            }

            // 6. ADDITIONAL PROJECTILES (apenas no spawn direto e não para minions/sentries)
            if (isDirectSpawn && !projectile.IsMinionOrSentryRelated && config.AdditionalProjectileChanceIncrement)
            {
                SpawnAdditionalProjectiles(projectile, globalItem, config, false);
            }
        }

        /// <summary>
        /// Spawna projéteis adicionais baseado na chance do item
        /// </summary>
        private void SpawnAdditionalProjectiles(Projectile projectile, GlobalItems.SignatureGlobalItem globalItem, Configs.ServerConfig config, bool isMinion)
        {
            // Verifica blacklist do item
            if (config.WeaponAdditionalProjectileChance.ItemBlackList.Contains(new ItemDefinition(ProjectileCreatorItem.type)))
                return;

            // Calcula a chance com HardCap do item se existir
            float garantChance;
            if (config.WeaponAdditionalProjectileChance.ItemHardCap.TryGetValue(new ItemDefinition(ProjectileCreatorItem.type), out var itemCap))
            {
                float notCapped = globalItem.GetAdditionalProjectileChanceNotCapped(ProjectileCreatorItem);
                garantChance = Math.Clamp(notCapped, int.MinValue, itemCap.Max) * itemCap.Sign;
            }
            else
            {
                garantChance = globalItem.GetAdditionalProjectileChanceCapped(ProjectileCreatorItem);
            }

            if (garantChance <= 0)
                return;

            // Calcula quantos projéteis spawnar
            int countShoots = (int)(garantChance / 100);
            int additionalChance = (int)(garantChance % 100);

            // Chance de 1 projétil adicional
            if (additionalChance > Main.rand.Next(100))
                countShoots++;

            if (countShoots <= 0)
                return;

            // Parâmetros de rotação
            int minRad = isMinion ? config.AdditionalProjectileMinRadMinion : config.AdditionalProjectileMinRad;
            int maxAdditionalRad = isMinion ? config.AdditionalProjectileMaxRadMinion : config.AdditionalProjectileMaxRad;
            float shootsToMaxRad = config.ProjectilesToMaxRad;

            // Spawna os projéteis adicionais
            for (int i = 0; i < countShoots; i++)
            {
                // Calcula rotação baseado no número de projéteis
                float rotation = MathHelper.ToRadians(minRad + maxAdditionalRad * Math.Min((float)countShoots / shootsToMaxRad, 1f));
                Vector2 newVelocity = projectile.velocity.RotatedByRandom(rotation);

                // Adiciona variação de velocidade
                newVelocity *= 1f - Main.rand.NextFloat(-0.1f, 0.1f);

                // Cria o novo projétil
                var cloneSource = new EntitySource_Parent(projectile);
                Projectile.NewProjectile(cloneSource, projectile.position, newVelocity, projectile.type, projectile.damage, ProjectileCreatorItem.knockBack, projectile.owner);
            }

            if (config.DebugMode && countShoots > 0)
                Main.NewText($"[DEBUG] Additional Projectiles: +{countShoots} (chance: {garantChance:F1}%)");
        }

        /// <summary>
        /// Aplica stats já calculados de um cache (otimização para projéteis duplicados)
        /// </summary>
        private void ApplyCachedStats(Projectile projectile, ProjectileStatsCache cache)
        {
            var config = ModContent.GetInstance<Configs.ServerConfig>();
            bool debug = config != null && config.DebugMode;

            if (cache.MinionSlots > 0)
            {
                projectile.minionSlots = cache.MinionSlots;
                if (debug) Main.NewText($"[DEBUG CACHED] Minion Slots: {cache.MinionSlots:F2}");
            }

            if (cache.SizeScale > 1f)
            {
                int newWidth = (int)(defaultWidth * cache.SizeScale);
                int newHeight = (int)(defaultHeight * cache.SizeScale);
                projectile.width = newWidth;
                projectile.height = newHeight;
                projectile.scale = defaultScale * cache.SizeScale;
                if (debug) Main.NewText($"[DEBUG CACHED] Size: {newWidth}x{newHeight}");
            }

            if (cache.SpeedMultiplier > 1f)
            {
                projectile.velocity *= cache.SpeedMultiplier;
                if (debug) Main.NewText($"[DEBUG CACHED] Speed: x{cache.SpeedMultiplier:F2}");
            }

            if (cache.AddedLifeTime > 0)
            {
                projectile.timeLeft += cache.AddedLifeTime;
                if (debug) Main.NewText($"[DEBUG CACHED] Lifetime: +{cache.AddedLifeTime}");
            }

            if (cache.AddedPenetration > 0)
            {
                projectile.penetrate += cache.AddedPenetration;
                if (debug) Main.NewText($"[DEBUG CACHED] Penetration: +{cache.AddedPenetration}");
            }
        }

        /// <summary>
        /// Cache de stats calculados para otimização
        /// </summary>
        private class ProjectileStatsCache
        {
            public float MinionSlots { get; set; }
            public float SizeScale { get; set; } = 1f;
            public float SpeedMultiplier { get; set; } = 1f;
            public int AddedLifeTime { get; set; }
            public int AddedPenetration { get; set; }
        }
        
        /// <summary>
        /// Desenha trail do projétil antes do projétil normal
        /// </summary>
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Spawna efeitos visuais de hit apenas se tiver item criador
            if (Main.netMode != NetmodeID.Server && ProjectileCreatorItem != null && !ProjectileCreatorItem.IsAir)
            {
                var globalItem = ProjectileCreatorItem.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
                if (globalItem != null && globalItem.Level > 0)
                {
                    // Usa a cor do outline para o efeito de hit
                    var clientConfig = ModContent.GetInstance<Configs.ClientConfig>();
                    Color hitColor = GetOutlineColorForLevel(globalItem.Level, clientConfig);
                    
                    Visual.ProjectileHitEffect.SpawnHitEffect(projectile.Center, hitColor, globalItem.Level, damageDone, ProjectileCreatorItem.DamageType);
                }
            }
            
            base.OnHitNPC(projectile, target, hit, damageDone);
        }
        
        /// <summary>
        /// Obtém a cor do outline baseado no nível (mesmo sistema do item)
        /// </summary>
        private Color GetOutlineColorForLevel(int level, Configs.ClientConfig config)
        {
            if (level >= 101)
                return config.OutlineColor_Level101Plus;
            if (level >= 76)
                return config.OutlineColor_Level76_100;
            if (level >= 51)
                return config.OutlineColor_Level51_75;
            if (level >= 26)
                return config.OutlineColor_Level26_50;
            
            return config.OutlineColor_Level1_25;
        }
    }
}
