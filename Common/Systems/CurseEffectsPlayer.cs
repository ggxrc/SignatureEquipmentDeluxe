using Terraria;
using Terraria.ModLoader;
using SignatureEquipmentDeluxe.Common.Data;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// ModPlayer que aplica penalidades de maldições
    /// </summary>
    public class CurseEffectsPlayer : ModPlayer
    {
        public override void PostUpdateEquips()
        {
            // Verifica item sendo segurado
            if (Player.HeldItem != null && !Player.HeldItem.IsAir)
            {
                var globalItem = Player.HeldItem.GetGlobalItem<GlobalItems.SignatureGlobalItem>();
                if (globalItem.EquippedRunes.Count > 0)
                {
                    ApplyCursePenalties(globalItem.EquippedRunes);
                }
            }
        }
        
        private void ApplyCursePenalties(System.Collections.Generic.List<EquippedRune> runes)
        {
            int defensePenalty = 0;
            bool setDefenseToZero = false;
            
            foreach (var rune in runes)
            {
                switch (rune.Type)
                {
                    case RuneType.CurseBerserker:
                        // -30% defesa
                        defensePenalty += (int)(Player.statDefense * 0.3f);
                        break;
                    
                    case RuneType.CurseAnnihilation:
                        // Defesa = 0 (sobrescreve tudo)
                        setDefenseToZero = true;
                        break;
                }
            }
            
            if (setDefenseToZero)
            {
                Player.statDefense -= Player.statDefense; // Zera defesa
            }
            else if (defensePenalty > 0)
            {
                Player.statDefense -= defensePenalty;
            }
        }
    }
}
