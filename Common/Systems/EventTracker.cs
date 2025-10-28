using System.Collections.Generic;
using System.Linq;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// Sistema de rastreamento de eventos consecutivos para anti-farm
    /// </summary>
    public class EventTracker
    {
        private static EventTracker _instance;
        public static EventTracker Instance => _instance ??= new EventTracker();
        
        // Histórico de eventos que TERMINARAM (para reset de penalidades)
        private Queue<GameEventType> endedEventsHistory = new Queue<GameEventType>();
        
        // Contador de repetições consecutivas por evento
        private Dictionary<GameEventType, int> consecutiveCount = new Dictionary<GameEventType, int>();
        
        // Eventos ativos no momento
        private HashSet<GameEventType> currentActiveEvents = new HashSet<GameEventType>();
        
        // Controle de notificações
        private HashSet<GameEventType> notifiedEvents = new HashSet<GameEventType>();
        
        // Controle de multi-boss (2+ bosses simultâneos)
        private bool multiBossPenaltyActive = false;
        
        // Propriedades públicas para visual effects
        public bool IsAnyBossActive => currentActiveEvents.Any(e => IsBossEvent(e));
        public bool IsAnyMoonEventActive => currentActiveEvents.Any(e => IsMoonEvent(e));
        public bool IsAnyInvasionActive => currentActiveEvents.Any(e => IsInvasionEvent(e));
        public bool IsWeatherEventActive => currentActiveEvents.Any(e => IsWeatherEvent(e));
        public bool IsAntiGrindPenaltyActive => consecutiveCount.Values.Any(count => count > 1);
        public float CurrentExpMultiplier => GetLowestPenaltyMultiplier();
        
        /// <summary>
        /// Atualiza o tracker com os eventos ativos no momento
        /// </summary>
        public void Update(List<GameEventType> activeEvents)
        {
            var newActiveEvents = new HashSet<GameEventType>(activeEvents);
            
            // Detecta eventos que COMEÇARAM (estão ativos agora mas não estavam antes)
            var startedEvents = newActiveEvents.Except(currentActiveEvents).ToList();
            
            // Detecta eventos que TERMINARAM (estavam ativos mas não estão mais)
            var endedEvents = currentActiveEvents.Except(newActiveEvents).ToList();
            
            // Processa eventos que começaram
            foreach (var eventType in startedEvents)
            {
                OnEventStarted(eventType);
            }
            
            // Processa eventos que terminaram
            foreach (var eventType in endedEvents)
            {
                OnEventEnded(eventType);
            }
            
            // Verifica multi-boss penalty
            CheckMultiBossPenalty(activeEvents);
            
            currentActiveEvents = newActiveEvents;
        }
        
        /// <summary>
        /// Verifica se há 2 ou mais bosses ativos simultaneamente
        /// </summary>
        private void CheckMultiBossPenalty(List<GameEventType> activeEvents)
        {
            // Conta quantos bosses estão ativos
            int activeBossCount = activeEvents.Count(e => IsBossEvent(e));
            
            if (activeBossCount >= 2 && !multiBossPenaltyActive)
            {
                // Ativa penalidade multi-boss
                multiBossPenaltyActive = true;
                
                // Aplica penalidade de 50% a TODOS os bosses ativos
                foreach (var eventType in activeEvents.Where(e => IsBossEvent(e)))
                {
                    // Força penalidade mínima de 0.5 (50%)
                    if (!consecutiveCount.ContainsKey(eventType))
                        consecutiveCount[eventType] = 0;
                    
                    // Se não tem penalidade ainda, adiciona uma repetição para forçar 50%
                    if (consecutiveCount[eventType] == 1)
                        consecutiveCount[eventType] = 2;
                }
                
                EventNotificationSystem.NotifyMultiBossPenalty(activeEvents.Where(e => IsBossEvent(e)).ToList());
            }
            else if (activeBossCount < 2)
            {
                multiBossPenaltyActive = false;
            }
        }
        
        /// <summary>
        /// Verifica se um evento é de boss
        /// </summary>
        private bool IsBossEvent(GameEventType eventType)
        {
            return eventType switch
            {
                GameEventType.KingSlime => true,
                GameEventType.EyeOfCthulhu => true,
                GameEventType.EaterOfWorlds => true,
                GameEventType.BrainOfCthulhu => true,
                GameEventType.QueenBee => true,
                GameEventType.Skeletron => true,
                GameEventType.Deerclops => true,
                GameEventType.WallOfFlesh => true,
                GameEventType.QueenSlime => true,
                GameEventType.TheTwins => true,
                GameEventType.TheDestroyer => true,
                GameEventType.SkeletronPrime => true,
                GameEventType.Plantera => true,
                GameEventType.Golem => true,
                GameEventType.EmpressOfLight => true,
                GameEventType.DukeFishron => true,
                GameEventType.LunaticCultist => true,
                GameEventType.MoonLord => true,
                _ => false
            };
        }
        
        /// <summary>
        /// Verifica se um evento é de lua (moon events)
        /// </summary>
        private bool IsMoonEvent(GameEventType eventType)
        {
            return eventType switch
            {
                GameEventType.BloodMoon => true,
                GameEventType.PumpkinMoon => true,
                GameEventType.FrostMoon => true,
                _ => false
            };
        }
        
        /// <summary>
        /// Verifica se um evento é invasão
        /// </summary>
        private bool IsInvasionEvent(GameEventType eventType)
        {
            return eventType switch
            {
                GameEventType.GoblinArmy => true,
                GameEventType.PirateInvasion => true,
                GameEventType.FrostLegion => true,
                GameEventType.MartianMadness => true,
                GameEventType.LunarEvent => true,
                _ => false
            };
        }
        
        /// <summary>
        /// Verifica se um evento é de clima
        /// </summary>
        private bool IsWeatherEvent(GameEventType eventType)
        {
            return eventType switch
            {
                GameEventType.Rain => true,
                GameEventType.Sandstorm => true,
                GameEventType.Blizzard => true,
                _ => false
            };
        }
        
        /// <summary>
        /// Obtém o menor multiplicador de penalidade atual (mais severo)
        /// </summary>
        private float GetLowestPenaltyMultiplier()
        {
            if (consecutiveCount.Count == 0)
                return 1f;
            
            float lowestMultiplier = 1f;
            foreach (var kvp in consecutiveCount)
            {
                float multiplier = GetPenaltyMultiplier(kvp.Key);
                if (multiplier < lowestMultiplier)
                    lowestMultiplier = multiplier;
            }
            return lowestMultiplier;
        }
        
        /// <summary>
        /// Chamado quando um evento começa
        /// </summary>
        private void OnEventStarted(GameEventType eventType)
        {
            // Incrementa contador de repetições consecutivas
            if (!consecutiveCount.ContainsKey(eventType))
                consecutiveCount[eventType] = 0;
            
            consecutiveCount[eventType]++;
            
            // Marca para notificação
            notifiedEvents.Add(eventType);
        }
        
        /// <summary>
        /// Chamado quando um evento termina
        /// </summary>
        private void OnEventEnded(GameEventType eventType)
        {
            notifiedEvents.Remove(eventType);
            
            // Adiciona ao histórico de eventos TERMINADOS
            endedEventsHistory.Enqueue(eventType);
            
            // Limita histórico a últimos 10 eventos terminados
            while (endedEventsHistory.Count > 10)
                endedEventsHistory.Dequeue();
            
            // Verifica se deve resetar penalidades
            CheckAndResetPenalties();
        }
        
        /// <summary>
        /// Verifica se algum evento deve ter sua penalidade resetada
        /// (quando 3 eventos DIFERENTES terminaram desde a última vez que ele foi acionado)
        /// </summary>
        private void CheckAndResetPenalties()
        {
            var eventsToReset = new List<GameEventType>();
            
            // Para cada evento com penalidade
            foreach (var kvp in consecutiveCount.Where(x => x.Value > 1).ToList())
            {
                var eventType = kvp.Key;
                
                // Conta quantos eventos DIFERENTES terminaram desde este evento
                var uniqueEndedEvents = new HashSet<GameEventType>();
                
                foreach (var endedEvent in endedEventsHistory)
                {
                    if (endedEvent != eventType)
                        uniqueEndedEvents.Add(endedEvent);
                }
                
                // Se 3 ou mais eventos diferentes terminaram, reseta
                if (uniqueEndedEvents.Count >= 3)
                {
                    eventsToReset.Add(eventType);
                }
            }
            
            // Reseta penalidades
            foreach (var eventType in eventsToReset)
            {
                consecutiveCount[eventType] = 0;
                endedEventsHistory.Clear(); // Limpa histórico após reset
                EventNotificationSystem.NotifyPenaltyReset(eventType);
            }
        }
        
        /// <summary>
        /// Obtém a penalidade atual de um evento (multiplicador entre 0.0 e 1.0)
        /// 0 repetições = 1.0 (100%)
        /// 1 repetição = 0.5 (50%)
        /// 2 repetições = 0.25 (25%)
        /// 3 repetições = 0.125 (12.5%)
        /// </summary>
        public float GetPenaltyMultiplier(GameEventType eventType)
        {
            if (!consecutiveCount.ContainsKey(eventType))
                return 1f;
            
            int count = consecutiveCount[eventType];
            if (count <= 1)
                return 1f;
            
            // Penalidade exponencial: 0.5^(count-1)
            return (float)System.Math.Pow(0.5, count - 1);
        }
        
        /// <summary>
        /// Obtém o número de repetições consecutivas de um evento
        /// </summary>
        public int GetConsecutiveCount(GameEventType eventType)
        {
            return consecutiveCount.ContainsKey(eventType) ? consecutiveCount[eventType] : 0;
        }
        
        /// <summary>
        /// Limpa todos os dados (útil para reset)
        /// </summary>
        public void Clear()
        {
            endedEventsHistory.Clear();
            consecutiveCount.Clear();
            currentActiveEvents.Clear();
            notifiedEvents.Clear();
        }
    }
}
