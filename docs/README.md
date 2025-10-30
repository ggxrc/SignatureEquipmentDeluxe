# SignatureEquipmentDeluxe - Documentação Técnica

## Visão Geral

**SignatureEquipmentDeluxe** é um mod avançado para Terraria desenvolvido com tModLoader que expande significativamente o sistema de progressão de equipamentos. O mod combina dois sistemas principais:

1. **Sistema de Equipamentos Assinados** - Progressão individual de itens com XP, níveis e habilidades especiais
2. **Sistema de Zonas Radioativas** - Mecânicas de combate dinâmico com inimigos nivelados e zonas de perigo crescente

## Arquitetura do Mod

### Estrutura de Diretórios

```
SignatureEquipmentDeluxe/
├── Common/
│   ├── GlobalItems/
│   │   ├── SignatureGlobalItem.cs      # Sistema de assinatura e progressão de itens
│   │   └── GlobalItems.cs              # Itens globais adicionais
│   ├── Players/
│   │   ├── SignaturePlayer.cs          # Dados persistentes do jogador
│   │   └── SignatureInputPlayer.cs     # Gerenciamento de input
│   ├── Systems/
│   │   ├── LeveledEnemySystem.cs       # Sistema de zonas radioativas e inimigos nivelados
│   │   ├── RadioactiveZoneVisuals.cs   # Efeitos visuais das zonas
│   │   ├── RadioactiveZoneDebuffs.cs   # Debuffs aplicados nas zonas
│   │   ├── CurseDeathSystem.cs         # Sistema de armas amaldiçoadas
│   │   ├── SignatureProgressionSystem.cs # Sistema de progressão de XP
│   │   └── SignatureAbilitySystem.cs   # Sistema de habilidades especiais
│   ├── UI/
│   │   └── SignatureManagementUI.cs    # Interface gráfica de gerenciamento
│   ├── Configs/
│   │   └── ServerConfig.cs             # Configurações do mod
│   └── SignatureKeybinds.cs            # Sistema de atalhos
├── Content/
│   ├── Items/
│   │   ├── SignatureManagementTome.cs  # Item para abrir UI
│   │   ├── XPBooster.cs                # Item consumível de XP
│   │   └── Runes/                      # Sistema de runas
│   │       ├── FireRune.cs
│   │       ├── IceRune.cs
│   │       └── ...
│   ├── Projectiles/
│   │   ├── WeaponAscensionProjectile.cs # Animação de criação de zona
│   │   └── ZoneUpgradeVisual.cs        # Efeitos visuais de upgrade
│   └── NPCs/                           # NPCs customizados (futuro)
├── Localization/
│   ├── en-US_Mods.SignatureEquipmentDeluxe.hjson
│   └── pt-BR_Mods.SignatureEquipmentDeluxe.hjson
├── Properties/
│   └── launchSettings.json
├── docs/
│   └── README.md                        # Esta documentação
└── .github/
    └── copilot-instructions.md          # Instruções para IA
```

## Sistema de Equipamentos Assinados

### Funcionalidades Core

#### Vinculação de Itens
- **Método**: Clique direito em qualquer item equipável
- **Persistência**: Dados salvos no mundo e sincronizados em multiplayer
- **Limitações**: Máximo de itens ativos baseado em configuração

#### Sistema de Progressão
- **Níveis**: 1-100 por item
- **Prestígio**: 10 níveis de prestígio (reset de nível com bônus permanentes)
- **XP**: Ganho baseado em:
  - Dano causado (escalado)
  - Acertos críticos
  - Kills de inimigos
  - Kills de bosses (bônus massivo)
  - Tempo de uso (passivo)

#### Bônus Escaláveis
```csharp
// Por nível
+1% dano
+1% crítico a cada 5 níveis
+0.2% velocidade de movimento

// Por prestígio
+5% dano
+2% defesa (armaduras)
+1% vida máxima (armaduras)
+1% velocidade de movimento (acessórios)
```

#### Habilidades Especiais
- **Nível 25**: Lifesteal em críticos (5% + escala)
- **Nível 50**: 20% chance de projétil extra
- **Nível 75**: Aura de dano (machuca inimigos próximos)
- **Prestígio 1+**: 2% esquiva por nível
- **Prestígio 3+**: Explosão ao matar
- **Prestígio 5+**: Regeneração acelerada

### Implementação Técnica

#### SignatureGlobalItem.cs
```csharp
public class SignatureGlobalItem : GlobalItem
{
    public bool IsSigned { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int Prestige { get; set; }

    // Métodos de progressão
    public void AddExperience(int amount)
    public int GetRequiredXP(int level)
    public void LevelUp()
    public void PrestigeUp()

    // Aplicação de bônus
    public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
    public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
    // ... outros modificadores
}
```

#### SignaturePlayer.cs
```csharp
public class SignaturePlayer : ModPlayer
{
    public Dictionary<int, SignatureItemData> SignedItems { get; set; }

    // Salvamento e carregamento
    public override void SaveData(TagCompound tag)
    public override void LoadData(TagCompound tag)

    // Sincronização multiplayer
    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
}
```

## Sistema de Zonas Radioativas

### Mecânicas Principais

#### Criação de Zonas
- **Trigger**: Morte do jogador com equipamento amaldiçoado
- **Duração**: 10 minutos fixos
- **Raio**: Base 375 tiles (150 * 2.5), cresce +10% por tier

#### Sistema de Tiers por Tempo
- **Tier 1** (0-2min): Verde, debuff weak
- **Tier 2** (2-4min): Amarelo, debuff ichor
- **Tier 3** (4-6min): Azul/Laranja, cursed fire no epicentro
- **Tier 4** (6-8min): Roxo/Vermelho claro, shadow flame antes do epicentro
- **Tier 5** (8-10min): Carmesim, dois anéis de fogo

#### Inimigos Nivelados
- **Scaling**: +35% vida, dano dinâmico, resistência baseada em nível
- **Penetração**: Ignora defesa baseada na diferença de nível
- **Confinamento**: Inimigos ficam presos na zona
- **Drops**: Runas e XP bônus ao morrer

#### Countdown Final
- **10 segundos finais**: Aviso vermelho
- **Atração de partículas**: Todas as partículas vão para o centro
- **Explosão**: Mata tudo fora de casas (detecção por NPCs town próximos)

### Implementação Técnica

#### LeveledEnemySystem.cs
```csharp
public class RadioactiveZone
{
    public Vector2 Position { get; set; }
    public float Radius { get; set; }
    public int DangerLevel { get; set; } // 1-5 baseado em tempo
    public bool IsFinalCountdown => isFinalCountdown;

    public void UpdateDangerLevel()
    {
        // Calcula tier baseado em tempo decorrido
        int elapsed = initialTime - TimeLeft;
        DangerLevel = 1 + (elapsed / (2 * 60 * 60));

        // Ajusta raio: +10% por tier
        Radius = InitialRadius * (1f + (DangerLevel - 1) * 0.1f);
    }

    public void TriggerFinalExplosion()
    {
        // Explosão massiva com proteção por casas
    }
}
```

#### RadioactiveZoneDebuffs.cs
```csharp
public class RadioactiveZoneDebuffs : ModPlayer
{
    public override void PostUpdate()
    {
        // Aplica debuffs baseados no tier e proximidade
        ApplyCenterDebuffs(intensity, zoneLevel);
    }
}
```

## Sistema de Runas

### Tipos de Runas
- **FireRune**: Dano de fogo, resistência a fogo
- **IceRune**: Dano de gelo, slow
- **PoisonRune**: Dano venenoso, debuff poison
- **LightningRune**: Dano elétrico, chain lightning
- **AttackSpeedRune**: + velocidade de ataque
- **LifeRegenRune**: + regeneração de vida
- **LifestealRune**: Roubo de vida

### Implementação
```csharp
public class EquippedRune
{
    public RuneType Type { get; set; }
    public int Level { get; set; }
    public bool IsCurse() => Type >= RuneType.Fire && Type <= RuneType.Lightning;
}
```

## Configurações e Balanceamento

### ServerConfig.cs
```csharp
public class ServerConfig : ModConfig
{
    // Sistema de equipamentos
    public bool EnableSignatureSystem { get; set; } = true;
    public int MaxSignedItems { get; set; } = 50;
    public float XPModifier { get; set; } = 1.0f;

    // Sistema de zonas
    public bool EnableLeveledEnemies { get; set; } = true;
    public float LeveledEnemySpawnChance { get; set; } = 0.15f;
    public float RadioactiveZoneRadius { get; set; } = 150f;

    // Sistema de runas
    public bool EnableRuneSystem { get; set; } = true;
    public float RuneDropChance { get; set; } = 0.05f;
}
```

## Sistema de Rede (Multiplayer)

### MessageType Enum
```csharp
public enum MessageType : byte
{
    ProjectileSizeSync,
    ItemLevelSync,
    ItemExperienceSync,
    SignaturePlayerSync,
    SignatureItemUpdate,
    SignaturePrestige
}
```

### Sincronização
- **Itens assinados**: Sincronizados ao equipar/usar
- **Progressão**: Atualizada em tempo real
- **Zonas**: Gerenciadas pelo servidor
- **Runas**: Drops sincronizados

## Desenvolvimento e Build

### Dependências
- **tModLoader**: Framework principal
- **Terraria**: Jogo base
- **.NET 8.0**: Runtime

### Build Commands
```bash
# Build do mod
dotnet build

# Build + Reload no tModLoader
# Use a interface gráfica do tModLoader
```

### Testes Recomendados
1. **Sistema de equipamentos**:
   - Vincular itens diferentes
   - Ganhar XP em combate
   - Testar habilidades especiais
   - Prestígio e bônus

2. **Sistema de zonas**:
   - Criar zona morrendo com item amaldiçoado
   - Observar progressão de tiers
   - Testar countdown final
   - Verificar proteção por casas

3. **Multiplayer**:
   - Sincronização de progressão
   - Zonas compartilhadas
   - Drops de runas

## Troubleshooting

### Problemas Comuns

#### Mod não compila
- Verificar versão do tModLoader
- Checar erros de sintaxe
- Limpar cache do projeto

#### Itens não ganham XP
- Confirmar vinculação (tooltip mostra nível)
- Verificar se está causando dano
- Checar configurações de XP

#### Zonas não criam
- Verificar se item tem runas de maldição
- Checar configuração EnableLeveledEnemies
- Testar em modo singleplayer primeiro

#### UI não abre
- Confirmar craft do SignatureManagementTome
- Pressionar tecla K
- Recarregar mods

### Logs e Debug
- Ver console do tModLoader para erros
- Checar arquivos de log em Documents/My Games/Terraria/Logs
- Usar breakpoints no IDE para debug

## Roadmap Futuro

### Melhorias Planejadas
- Sistema de conquistas
- Mais tipos de runas
- Boss específico do mod
- Sistema de missões
- Sinergias entre equipamentos
- Efeitos sonoros customizados

### Compatibilidade
- Testes com mods populares
- API para mods externos
- Sistema de blacklist de itens

## Contribuição

### Guidelines
1. Seguir convenções de código C#
2. Documentar métodos públicos
3. Testar mudanças extensivamente
4. Manter compatibilidade multiplayer

### Pull Requests
1. Criar branch para feature
2. Implementar com testes
3. Documentar mudanças
4. Submeter PR com descrição detalhada

## Créditos

**Desenvolvido por**: Kenflesh & ggxrc
**Versão**: 0.1 (Release Inicial)
**Data**: Outubro 2025
**Framework**: tModLoader
**Inspirado em**: Signature Weapon (mod original)

---

*Esta documentação é mantida atualizada com o código. Para mudanças recentes, consulte o git log.*