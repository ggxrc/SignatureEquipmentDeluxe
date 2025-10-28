using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace SignatureEquipmentDeluxe.Common.Commands
{
    /// <summary>
    /// Comandos para manipular níveis de items
    /// </summary>
    public class SetItemLevelCommand : ModCommand
    {
        public override string Command => "setitemlevel";
        
        public override CommandType Type => CommandType.Chat;
        
        public override string Usage => "/setitemlevel <level> - Define o nível do item segurado";
        
        public override string Description => "Define o nível do item que você está segurando";
        
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length < 1)
            {
                caller.Reply("Uso: /setitemlevel <level>", Microsoft.Xna.Framework.Color.Red);
                return;
            }
            
            if (!int.TryParse(args[0], out int level) || level < 0)
            {
                caller.Reply("Nível inválido! Use um número maior ou igual a 0.", Microsoft.Xna.Framework.Color.Red);
                return;
            }
            
            Player player = caller.Player;
            Item heldItem = player.HeldItem;
            
            if (heldItem == null || heldItem.IsAir)
            {
                caller.Reply("Você precisa estar segurando um item!", Microsoft.Xna.Framework.Color.Red);
                return;
            }
            
            var globalItem = heldItem.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
            if (globalItem == null)
            {
                caller.Reply("Item não pode ganhar níveis!", Microsoft.Xna.Framework.Color.Red);
                return;
            }
            
            // Define nível e XP necessário para o próximo nível
            globalItem.Level = level;
            globalItem.Experience = 0;
            
            caller.Reply($"Nível do {heldItem.Name} definido para {level}!", Microsoft.Xna.Framework.Color.Green);
        }
    }
    
    /// <summary>
    /// Comando para resetar níveis de todos os items no inventário
    /// </summary>
    public class ResetInventoryLevelsCommand : ModCommand
    {
        public override string Command => "resetinventorylevels";
        
        public override CommandType Type => CommandType.Chat;
        
        public override string Usage => "/resetinventorylevels - Reseta o nível de todos os items no inventário";
        
        public override string Description => "Reseta o nível de todos os items no seu inventário para 0";
        
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Player player = caller.Player;
            int resetCount = 0;
            
            // Reseta items do inventário principal
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];
                if (item != null && !item.IsAir)
                {
                    var globalItem = item.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
                    if (globalItem != null && globalItem.Level > 0)
                    {
                        globalItem.Level = 0;
                        globalItem.Experience = 0;
                        resetCount++;
                    }
                }
            }
            
            // Reseta armadura
            for (int i = 0; i < player.armor.Length; i++)
            {
                Item item = player.armor[i];
                if (item != null && !item.IsAir)
                {
                    var globalItem = item.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
                    if (globalItem != null && globalItem.Level > 0)
                    {
                        globalItem.Level = 0;
                        globalItem.Experience = 0;
                        resetCount++;
                    }
                }
            }
            
            // Reseta acessórios
            for (int i = 0; i < player.miscEquips.Length; i++)
            {
                Item item = player.miscEquips[i];
                if (item != null && !item.IsAir)
                {
                    var globalItem = item.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
                    if (globalItem != null && globalItem.Level > 0)
                    {
                        globalItem.Level = 0;
                        globalItem.Experience = 0;
                        resetCount++;
                    }
                }
            }
            
            caller.Reply($"Resetados {resetCount} items!", Microsoft.Xna.Framework.Color.Green);
        }
    }
    
    /// <summary>
    /// Comando para adicionar XP ao item segurado
    /// </summary>
    public class AddItemXPCommand : ModCommand
    {
        public override string Command => "additemxp";
        
        public override CommandType Type => CommandType.Chat;
        
        public override string Usage => "/additemxp <amount> - Adiciona XP ao item segurado";
        
        public override string Description => "Adiciona experiência ao item que você está segurando";
        
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length < 1)
            {
                caller.Reply("Uso: /additemxp <amount>", Microsoft.Xna.Framework.Color.Red);
                return;
            }
            
            if (!int.TryParse(args[0], out int xp) || xp <= 0)
            {
                caller.Reply("Quantidade inválida! Use um número maior que 0.", Microsoft.Xna.Framework.Color.Red);
                return;
            }
            
            Player player = caller.Player;
            Item heldItem = player.HeldItem;
            
            if (heldItem == null || heldItem.IsAir)
            {
                caller.Reply("Você precisa estar segurando um item!", Microsoft.Xna.Framework.Color.Red);
                return;
            }
            
            var globalItem = heldItem.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
            if (globalItem == null)
            {
                caller.Reply("Item não pode ganhar XP!", Microsoft.Xna.Framework.Color.Red);
                return;
            }
            
            int levelBefore = globalItem.Level;
            globalItem.AddExperience(xp, heldItem);
            int levelAfter = globalItem.Level;
            
            if (levelAfter > levelBefore)
            {
                caller.Reply($"Adicionados {xp} XP! {heldItem.Name} subiu para o nível {levelAfter}!", Microsoft.Xna.Framework.Color.Gold);
            }
            else
            {
                caller.Reply($"Adicionados {xp} XP ao {heldItem.Name} (Nível {levelAfter})!", Microsoft.Xna.Framework.Color.Green);
            }
        }
    }
    
    /// <summary>
    /// Comando para definir nível de todos os items no inventário
    /// </summary>
    public class SetAllInventoryLevelsCommand : ModCommand
    {
        public override string Command => "setallinventorylevels";
        
        public override CommandType Type => CommandType.Chat;
        
        public override string Usage => "/setallinventorylevels <level> - Define o nível de todos os items no inventário";
        
        public override string Description => "Define o mesmo nível para todos os items no seu inventário";
        
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length < 1)
            {
                caller.Reply("Uso: /setallinventorylevels <level>", Microsoft.Xna.Framework.Color.Red);
                return;
            }
            
            if (!int.TryParse(args[0], out int level) || level < 0)
            {
                caller.Reply("Nível inválido! Use um número maior ou igual a 0.", Microsoft.Xna.Framework.Color.Red);
                return;
            }
            
            Player player = caller.Player;
            int setCount = 0;
            
            // Define nível dos items do inventário principal
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];
                if (item != null && !item.IsAir)
                {
                    var globalItem = item.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
                    if (globalItem != null)
                    {
                        globalItem.Level = level;
                        globalItem.Experience = 0;
                        setCount++;
                    }
                }
            }
            
            // Define nível da armadura
            for (int i = 0; i < player.armor.Length; i++)
            {
                Item item = player.armor[i];
                if (item != null && !item.IsAir)
                {
                    var globalItem = item.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
                    if (globalItem != null)
                    {
                        globalItem.Level = level;
                        globalItem.Experience = 0;
                        setCount++;
                    }
                }
            }
            
            // Define nível dos acessórios
            for (int i = 0; i < player.miscEquips.Length; i++)
            {
                Item item = player.miscEquips[i];
                if (item != null && !item.IsAir)
                {
                    var globalItem = item.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
                    if (globalItem != null)
                    {
                        globalItem.Level = level;
                        globalItem.Experience = 0;
                        setCount++;
                    }
                }
            }
            
            caller.Reply($"{setCount} items definidos para o nível {level}!", Microsoft.Xna.Framework.Color.Green);
        }
    }
}
