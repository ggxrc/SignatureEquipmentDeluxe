namespace SignatureEquipmentDeluxe.Common.Data
{
    /// <summary>
    /// Tipos de runas disponíveis no sistema
    /// </summary>
    public enum RuneType
    {
        None = 0,
        
        // === ELEMENTAIS ===
        Fire,       // Dano de fogo + queima
        Ice,        // Dano de gelo + slow
        Poison,     // Dano de veneno + DoT
        Lightning,  // Dano elétrico + chain
        
        // === UTILITÁRIOS ===
        AttackSpeed,    // Velocidade de ataque
        LifeRegen,      // Regeneração de vida
        Lifesteal,      // Vampirismo
        
        // === MALDIÇÕES ===
        CurseBerserker,     // +50% dano, -30% defesa
        CurseGlass,         // +100% crit, -50% vida
        CurseAnnihilation   // +150% dano, -35% vida, defesa = 0
    }
}
