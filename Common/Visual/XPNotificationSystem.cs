using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace SignatureEquipmentDeluxe.Common.Visual
{
    /// <summary>
    /// Sistema que consolida notificações de XP separadamente por tipo (armor/weapon) com cooldown
    /// </summary>
    public static class XPNotificationSystem
    {
        private class XPAccumulator
        {
            public int TotalAmount { get; set; }
            public int Count { get; set; }
            public Color Color { get; set; }
            public int Timer { get; set; }
        }

        private static XPAccumulator weaponAccumulator = null;
        private static XPAccumulator armorAccumulator = null;
        private const int WEAPON_COOLDOWN = 6;  // 0.1 segundos (6 frames a 60fps)
        private const int ARMOR_COOLDOWN = 12;  // 0.2 segundos (12 frames a 60fps) - apenas um popup para as 3 peças

        /// <summary>
        /// Atualiza o sistema de notificações (deve ser chamado todo frame)
        /// </summary>
        public static void Update()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // Atualiza acumulador de weapon
            if (weaponAccumulator != null)
            {
                weaponAccumulator.Timer--;
                if (weaponAccumulator.Timer <= 0)
                {
                    ShowConsolidatedNotification(weaponAccumulator);
                    weaponAccumulator = null;
                }
            }

            // Atualiza acumulador de armor
            if (armorAccumulator != null)
            {
                armorAccumulator.Timer--;
                if (armorAccumulator.Timer <= 0)
                {
                    ShowConsolidatedNotification(armorAccumulator);
                    armorAccumulator = null;
                }
            }
        }

        /// <summary>
        /// Adiciona XP ao acumulador apropriado (weapon ou armor separadamente)
        /// </summary>
        public static void AddXPNotification(int xpAmount, Vector2 position, Color color, bool isArmor)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            if (isArmor)
            {
                // Acumulador de ARMOR
                if (armorAccumulator == null)
                {
                    armorAccumulator = new XPAccumulator
                    {
                        TotalAmount = xpAmount,
                        Count = 1,
                        Color = color,
                        Timer = ARMOR_COOLDOWN
                    };
                }
                else
                {
                    armorAccumulator.TotalAmount += xpAmount;
                    armorAccumulator.Count++;
                    armorAccumulator.Timer = ARMOR_COOLDOWN; // Reseta timer
                }
            }
            else
            {
                // Acumulador de WEAPON
                if (weaponAccumulator == null)
                {
                    weaponAccumulator = new XPAccumulator
                    {
                        TotalAmount = xpAmount,
                        Count = 1,
                        Color = color,
                        Timer = WEAPON_COOLDOWN
                    };
                }
                else
                {
                    weaponAccumulator.TotalAmount += xpAmount;
                    weaponAccumulator.Count++;
                    weaponAccumulator.Timer = WEAPON_COOLDOWN; // Reseta timer
                }
            }
        }

        /// <summary>
        /// Mostra a notificação consolidada
        /// </summary>
        private static void ShowConsolidatedNotification(XPAccumulator accumulator)
        {
            string text;
            
            if (accumulator.Count > 1)
            {
                // Múltiplas notificações consolidadas
                text = $"+{accumulator.TotalAmount} EXP (x{accumulator.Count})";
            }
            else
            {
                // Notificação única
                text = $"+{accumulator.TotalAmount} EXP";
            }

            // Posição aleatória ao redor do player
            Player player = Main.LocalPlayer;
            Vector2 randomOffset = new Vector2(
                Main.rand.Next(-80, 81),
                Main.rand.Next(-80, 81)
            );
            Vector2 finalPosition = player.Center + randomOffset;

            Rectangle rect = new Rectangle((int)finalPosition.X, (int)finalPosition.Y, 10, 10);
            int combatTextID = CombatText.NewText(rect, accumulator.Color, text);
            
            // Reduz o tempo de tela pela metade (padrão é ~60 frames, reduzindo para 30)
            if (combatTextID >= 0 && combatTextID < Main.maxCombatText)
            {
                Main.combatText[combatTextID].lifeTime = 30;
            }
        }

        /// <summary>
        /// Limpa todos os acumuladores (útil para reset)
        /// </summary>
        public static void Clear()
        {
            weaponAccumulator = null;
            armorAccumulator = null;
        }

        /// <summary>
        /// Força a exibição imediata de ambos os acumuladores (se houver)
        /// </summary>
        public static void Flush()
        {
            if (weaponAccumulator != null && weaponAccumulator.TotalAmount > 0)
            {
                ShowConsolidatedNotification(weaponAccumulator);
                weaponAccumulator = null;
            }

            if (armorAccumulator != null && armorAccumulator.TotalAmount > 0)
            {
                ShowConsolidatedNotification(armorAccumulator);
                armorAccumulator = null;
            }
        }
    }
}
