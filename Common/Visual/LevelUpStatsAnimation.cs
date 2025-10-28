using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace SignatureEquipmentDeluxe.Common.Visual
{
    /// <summary>
    /// Sistema de animação de status ao fazer level up
    /// </summary>
    public class LevelUpStatsAnimation
    {
        private static Queue<StatIncrease> pendingStats = new Queue<StatIncrease>();
        private static StatIncrease currentStat = null;
        private static int animationTimer = 0;
        private static int delayBetweenStats = 39; // Frames entre cada stat (0.65 segundos a 60fps)
        private static int initialDelay = 39; // Delay inicial antes do primeiro stat (0.65s para dar tempo do "Level Up!" desaparecer)
        private static bool isWaitingForInitialDelay = false;
        
        /// <summary>
        /// Inicia uma nova sequência de animações de level up
        /// </summary>
        public static void StartLevelUpSequence()
        {
            isWaitingForInitialDelay = true;
            animationTimer = 0;
        }
        
        /// <summary>
        /// Adiciona um stat para ser animado
        /// </summary>
        public static void AddStatIncrease(string statName, float oldValue, float newValue, Color color)
        {
            pendingStats.Enqueue(new StatIncrease
            {
                StatName = statName,
                OldValue = oldValue,
                NewValue = newValue,
                Color = color
            });
        }
        
        /// <summary>
        /// Atualiza as animações de stats (deve ser chamado todo frame)
        /// </summary>
        public static void Update()
        {
            // Se está no delay inicial, aguarda antes de processar stats
            if (isWaitingForInitialDelay)
            {
                animationTimer++;
                if (animationTimer >= initialDelay)
                {
                    isWaitingForInitialDelay = false;
                    animationTimer = 0;
                }
                return; // Não processa stats durante o delay inicial
            }
            
            // Se não há stat atual mas há na fila, pega o próximo
            if (currentStat == null && pendingStats.Count > 0)
            {
                currentStat = pendingStats.Dequeue();
                animationTimer = 0;
                ShowStatIncrease(currentStat);
            }
            
            // Se há um stat sendo animado, conta o tempo
            if (currentStat != null)
            {
                animationTimer++;
                
                // Após o delay, libera para o próximo
                if (animationTimer >= delayBetweenStats)
                {
                    currentStat = null;
                    animationTimer = 0;
                }
            }
        }
        
        /// <summary>
        /// Mostra o aumento de stat na tela com som
        /// </summary>
        private static void ShowStatIncrease(StatIncrease stat)
        {
            Player player = Main.LocalPlayer;
            
            // Posição acima da cabeça do jogador, levemente variável
            Vector2 textPosition = player.Top + new Vector2(Main.rand.NextFloat(-30f, 30f), -60f);
            
            // Calcula a diferença
            float difference = stat.NewValue - stat.OldValue;
            string differenceText = difference > 0 ? $"+{difference:F1}" : $"{difference:F1}";
            
            // Texto do stat com valor aumentado
            string displayText = $"{stat.StatName}: {differenceText}";
            
            // Cria combat text animado
            int textIndex = CombatText.NewText(
                new Rectangle((int)textPosition.X, (int)textPosition.Y, 100, 20),
                stat.Color,
                displayText,
                dramatic: true,
                dot: false
            );
            
            // Som de "ding" para cada stat
            SoundEngine.PlaySound(SoundID.Item4 with 
            { 
                Volume = 0.5f, 
                Pitch = 0.5f + (pendingStats.Count * 0.1f),  // Pitch aumenta com cada stat
                PitchVariance = 0.1f
            }, player.Center);
        }
        
        /// <summary>
        /// Limpa toda a fila de animações
        /// </summary>
        public static void Clear()
        {
            pendingStats.Clear();
            currentStat = null;
            animationTimer = 0;
        }
        
        /// <summary>
        /// Verifica se ainda há animações pendentes
        /// </summary>
        public static bool HasPendingAnimations()
        {
            return currentStat != null || pendingStats.Count > 0;
        }
        
        private class StatIncrease
        {
            public string StatName { get; set; }
            public float OldValue { get; set; }
            public float NewValue { get; set; }
            public Color Color { get; set; }
        }
    }
}
