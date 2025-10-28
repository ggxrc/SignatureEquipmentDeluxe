using Terraria;
using Terraria.ModLoader;
using SignatureEquipmentDeluxe.Common.Systems;
using System.Collections.Generic;
using System.Linq;

namespace SignatureEquipmentDeluxe.Common.Players
{
    public class SignaturePlayer : ModPlayer
    {
        public float xpMultiplier = 1f;
        
        // Cache para evitar recalcular a cada hit
        private float cachedEventsMultiplier = 1f;
        private int cacheFrameCount = 0;
        private List<GameEventType> previousActiveEvents = new List<GameEventType>();

        // Tracking para idle aura animation
        private int idleFrameCounter = 0;
        private const int IDLE_THRESHOLD = 600; // 10 segundos (60 fps * 10)
        public bool IsIdleForAura => idleFrameCounter >= IDLE_THRESHOLD;

        public override void Initialize()
        {
            xpMultiplier = 1f;
            cachedEventsMultiplier = 1f;
            cacheFrameCount = 0;
            previousActiveEvents.Clear();
            idleFrameCounter = 0;
        }

        public void IncreaseXPMultiplier(float amount)
        {
            xpMultiplier += amount;
        }
        
        /// <summary>
        /// Atualiza o sistema de eventos a cada frame
        /// </summary>
        public override void PostUpdate()
        {
            // Tracking de movimento para idle aura
            bool isMoving = Player.velocity.LengthSquared() > 0.01f;
            if (isMoving)
            {
                idleFrameCounter = 0; // Reset quando está se movendo
            }
            else
            {
                idleFrameCounter++; // Incrementa quando está parado
            }
            
            // Atualiza tracking de eventos a cada 60 frames (1 segundo)
            if (Main.GameUpdateCount % 60 == 0)
            {
                UpdateEventTracking();
            }
            
            // Atualiza efeitos visuais (client-side apenas)
            if (Main.netMode != Terraria.ID.NetmodeID.Server)
            {
                Visual.AuraEffect.UpdatePlayerAura(Player);
                Visual.EventVisualEffect.UpdateEventVisuals(Player);
                Visual.EventVisualEffect.UpdatePenaltyVisuals(Player);
                Visual.LevelUpStatsAnimation.Update(); // Atualiza animações de stats
                Visual.XPNotificationSystem.Update(); // Atualiza notificações de XP consolidadas
            }
        }
        
        /// <summary>
        /// Atualiza o tracking e notificações de eventos
        /// </summary>
        private void UpdateEventTracking()
        {
            var config = ModContent.GetInstance<Configs.ServerConfig>();
            var activeEvents = EventDetector.GetActiveEvents();
            
            // Detecta eventos que começaram
            var startedEvents = activeEvents.Except(previousActiveEvents).ToList();
            
            // Detecta eventos que terminaram
            var endedEvents = previousActiveEvents.Except(activeEvents).ToList();
            
            // Atualiza tracker (isso dispara notificações de reset internamente)
            EventTracker.Instance.Update(activeEvents);
            
            // Calcula multiplicador atual
            float newMultiplier = CalculateEventsMultiplier(activeEvents);
            
            // Notifica eventos iniciados
            foreach (var eventType in startedEvents)
            {
                var eventConfig = GetEventConfig(eventType);
                if (eventConfig != null && eventConfig.Enabled)
                {
                    float baseMultiplier = eventConfig.GetBaseMultiplier();
                    
                    // Verifica se penalidade está habilitada para esta categoria
                    bool penaltyEnabled = IsPenaltyEnabledForCategory(eventConfig.Category, config);
                    float penalty = penaltyEnabled ? EventTracker.Instance.GetPenaltyMultiplier(eventType) : 1f;
                    
                    float finalMultiplier = 1f + ((baseMultiplier - 1f) * penalty);
                    int penaltyPercent = (int)(penalty * 100f);
                    
                    EventNotificationSystem.NotifyEventStarted(eventType, baseMultiplier, finalMultiplier, penaltyPercent);
                }
            }
            
            // Notifica stack de eventos (se múltiplos ativos)
            if (activeEvents.Count > 1 && startedEvents.Count > 0)
            {
                EventNotificationSystem.NotifyEventsStacked(activeEvents, newMultiplier);
            }
            
            // Notifica eventos terminados
            foreach (var eventType in endedEvents)
            {
                EventNotificationSystem.NotifyEventEnded(eventType, newMultiplier);
            }
            
            previousActiveEvents = activeEvents;
            cachedEventsMultiplier = newMultiplier;
            cacheFrameCount = (int)Main.GameUpdateCount;
        }
        
        /// <summary>
        /// Obtém a configuração de um evento específico
        /// </summary>
        private EventMultiplier GetEventConfig(GameEventType eventType)
        {
            var config = ModContent.GetInstance<Configs.ServerConfig>();
            
            // Busca em todas as listas categorizadas
            var allEvents = new List<EventMultiplier>();
            allEvents.AddRange(config.BossEventsPreHardmode);
            allEvents.AddRange(config.BossEventsHardmode);
            allEvents.AddRange(config.MoonEvents);
            allEvents.AddRange(config.InvasionEvents);
            allEvents.AddRange(config.TimeEvents);
            allEvents.AddRange(config.WeatherEvents);
            allEvents.AddRange(config.SpecialEvents);
            
            return allEvents.FirstOrDefault(e => e.EventType == eventType);
        }
        
        /// <summary>
        /// Calcula o multiplicador combinado de todos os eventos ativos (com penalidades)
        /// </summary>
        private float CalculateEventsMultiplier(List<GameEventType> activeEvents)
        {
            if (activeEvents.Count == 0)
                return 1f;
            
            var config = ModContent.GetInstance<Configs.ServerConfig>();
            float totalMultiplier = 1f;
            
            foreach (var eventType in activeEvents)
            {
                var eventConfig = GetEventConfig(eventType);
                if (eventConfig != null && eventConfig.Enabled)
                {
                    // Multiplicador base da configuração
                    float baseMultiplier = eventConfig.GetBaseMultiplier();
                    
                    // Verifica se esta categoria tem penalidade ativada
                    bool penaltyEnabled = IsPenaltyEnabledForCategory(eventConfig.Category, config);
                    
                    // Penalidade por repetição (se habilitada para esta categoria)
                    float penalty = penaltyEnabled ? EventTracker.Instance.GetPenaltyMultiplier(eventType) : 1f;
                    
                    // Aplica penalidade: se base é 1.5x (+50%) e penalidade é 50%, resultado é 1.25x (+25%)
                    float bonusPercent = (baseMultiplier - 1f) * penalty;
                    
                    totalMultiplier += bonusPercent;
                }
            }
            
            return totalMultiplier;
        }
        
        /// <summary>
        /// Verifica se a penalidade está habilitada para uma categoria específica
        /// </summary>
        private bool IsPenaltyEnabledForCategory(EventCategory category, Configs.ServerConfig config)
        {
            return category switch
            {
                EventCategory.BossPreHardmode => config.EnableBossPenalty,
                EventCategory.BossHardmode => config.EnableBossPenalty,
                EventCategory.Invasion => config.EnableInvasionPenalty,
                EventCategory.Moon => config.EnableMoonPenalty,
                EventCategory.Weather => config.EnableWeatherPenalty,
                EventCategory.Time => config.EnableTimePenalty,
                EventCategory.Special => config.EnableSpecialPenalty,
                _ => false
            };
        }
        
        /// <summary>
        /// Retorna o multiplicador de eventos (usa cache)
        /// </summary>
        private float GetActiveEventsMultiplier()
        {
            // Se cache está desatualizado (mais de 1 segundo), recalcula
            if (Main.GameUpdateCount - cacheFrameCount > 60)
            {
                var activeEvents = EventDetector.GetActiveEvents();
                cachedEventsMultiplier = CalculateEventsMultiplier(activeEvents);
                cacheFrameCount = (int)Main.GameUpdateCount;
            }
            
            return cachedEventsMultiplier;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            var config = ModContent.GetInstance<Configs.ServerConfig>();
            if (!config.AllowStatueXP && target.SpawnedFromStatue) return;
            
            // Adiciona hit ao combo system
            var comboSystem = Player.GetModPlayer<Systems.ComboSystem>();
            if (comboSystem != null)
            {
                comboSystem.AddComboHit(target.Center);
            }
            
            Item heldItem = Player.HeldItem;
            if (heldItem != null && !heldItem.IsAir)
            {
                var globalItem = heldItem.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
                if (globalItem != null && globalItem.CanGainExperience(heldItem))
                {
                    DistributeHitXP(heldItem, globalItem, target, damageDone);
                    if (target.life <= 0)
                    {
                        // Adiciona kill ao streak
                        var killStreakSystem = Player.GetModPlayer<Systems.KillStreakSystem>();
                        if (killStreakSystem != null)
                        {
                            killStreakSystem.AddKill(target.Center, target.lifeMax);
                        }
                        
                        DistributeKillXP(heldItem, globalItem, target);
                    }
                }
            }
        }
        
        public override void OnHurt(Player.HurtInfo info)
        {
            var config = ModContent.GetInstance<Configs.ServerConfig>();
            int damageReceived = info.Damage;
            
            if (config.ArmorXPIgnoreDefense)
            {
                damageReceived = info.Damage + (Player.statDefense / 2);
            }
            
            DistributeArmorXP(Player.armor[0], damageReceived, config);
            DistributeArmorXP(Player.armor[1], damageReceived, config);
            DistributeArmorXP(Player.armor[2], damageReceived, config);
        }
        
        private void DistributeArmorXP(Item armorPiece, int damageReceived, Configs.ServerConfig config)
        {
            if (armorPiece == null || armorPiece.IsAir || armorPiece.defense <= 0) return;
            
            var globalItem = armorPiece.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
            if (globalItem == null || !globalItem.CanGainExperience(armorPiece)) return;
            
            float xpGain = damageReceived * config.ArmorXPPerDamageReceived;
            
            // Aplica multiplicadores: config, player, eventos
            xpGain *= config.GlobalExpMultiplier * config.ArmorExpMultiplier * xpMultiplier;
            xpGain *= GetActiveEventsMultiplier();
            
            if (xpGain > 0)
            {
                globalItem.AddExperience((int)xpGain, armorPiece, isArmor: true);
            }
        }

        private void DistributeHitXP(Item item, GlobalItems.SignatureGlobalItem globalItem, NPC target, int damageDone)
        {
            var config = ModContent.GetInstance<Configs.ServerConfig>();
            float xpGain = config.WeaponBaseXPPerHit + (damageDone * config.WeaponXPPerDamageDealt);
            
            // Aplica multiplicadores: config, player, eventos, combo
            xpGain *= config.GlobalExpMultiplier * config.WeaponExpMultiplier * xpMultiplier;
            xpGain *= GetActiveEventsMultiplier();
            
            var comboSystem = Player.GetModPlayer<Systems.ComboSystem>();
            if (comboSystem != null)
            {
                xpGain *= comboSystem.GetComboXPMultiplier();
            }
            
            globalItem.AddExperience((int)xpGain, item);
        }

        private void DistributeKillXP(Item item, GlobalItems.SignatureGlobalItem globalItem, NPC target)
        {
            var config = ModContent.GetInstance<Configs.ServerConfig>();
            float xpGain = config.WeaponBaseXPPerKill + (target.lifeMax * config.WeaponXPPerEnemyMaxHP);
            
            // Aplica multiplicadores: config, player, eventos, combo, kill streak
            xpGain *= config.GlobalExpMultiplier * config.WeaponExpMultiplier * xpMultiplier;
            xpGain *= GetActiveEventsMultiplier();
            
            var comboSystem = Player.GetModPlayer<Systems.ComboSystem>();
            if (comboSystem != null)
            {
                xpGain *= comboSystem.GetComboXPMultiplier();
            }
            
            var killStreakSystem = Player.GetModPlayer<Systems.KillStreakSystem>();
            if (killStreakSystem != null)
            {
                xpGain *= killStreakSystem.GetStreakXPMultiplier();
            }
            
            globalItem.AddExperience((int)xpGain, item);
        }
        
        /// <summary>
        /// Obtém o EventTracker para acesso aos visuais
        /// </summary>
        public EventTracker GetEventTracker()
        {
            return EventTracker.Instance;
        }
    }
}
