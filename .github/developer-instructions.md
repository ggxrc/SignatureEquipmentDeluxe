# Developer Instructions for SignatureEquipmentDeluxe

## Visão Geral do Projeto

**SignatureEquipmentDeluxe** é um mod complexo para Terraria que combina dois sistemas principais:

1. **Sistema de Equipamentos Assinados**: Progressão individual de itens com XP, níveis, prestígio e habilidades especiais
2. **Sistema de Zonas Radioativas**: Mecânicas dinâmicas de combate com inimigos nivelados e zonas de perigo crescente

O mod é desenvolvido com tModLoader e segue princípios de modularidade e separação de responsabilidades.

## Estrutura do Código

### Organização por Responsabilidades

#### Common/GlobalItems/
- **`SignatureGlobalItem.cs`**: Core do sistema de assinatura. Gerencia vinculação, progressão e aplicação de bônus
- **`GlobalItems.cs`**: Extensões adicionais para itens globais

#### Common/Players/
- **`SignaturePlayer.cs`**: Dados persistentes do jogador, salvamento e sincronização multiplayer
- **`SignatureInputPlayer.cs`**: Gerenciamento de input do jogador (tecla K para UI)

#### Common/Systems/
- **`LeveledEnemySystem.cs`**: Sistema completo de zonas radioativas e inimigos nivelados
- **`RadioactiveZoneVisuals.cs`**: Todos os efeitos visuais das zonas (partículas, cores, animações)
- **`RadioactiveZoneDebuffs.cs`**: Aplicação de debuffs baseada em proximidade e tier da zona
- **`CurseDeathSystem.cs`**: Sistema de morte com drops de armas amaldiçoadas
- **`SignatureProgressionSystem.cs`**: Lógica de ganho de XP e progressão
- **`SignatureAbilitySystem.cs`**: Implementação das habilidades especiais

#### Common/UI/
- **`SignatureManagementUI.cs`**: Interface gráfica completa para gerenciamento de equipamentos

#### Content/Items/
- **`SignatureManagementTome.cs`**: Item para abrir a UI de gerenciamento
- **`XPBooster.cs`**: Item consumível que dá XP bônus
- **`Runes/`**: Sistema de runas elementais e maldições

#### Content/Projectiles/
- **`WeaponAscensionProjectile.cs`**: Animação épica de criação de zona radioativa
- **`ZoneUpgradeVisual.cs`**: Efeitos visuais de upgrade de zona

## Convenções de Código

### Nomenclatura
- **Classes**: PascalCase, sufixo descritivo (e.g., `System`, `Player`, `Item`)
- **Métodos**: PascalCase, verbos imperativos
- **Propriedades**: PascalCase
- **Campos privados**: camelCase com underscore prefix (e.g., `_privateField`)
- **Constantes**: UPPER_SNAKE_CASE

### Estrutura de Arquivos
```csharp
// Cabeçalho de arquivo
using System;
using Terraria;
using Terraria.ModLoader;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    /// <summary>
    /// Descrição detalhada da classe e suas responsabilidades
    /// </summary>
    public class ClassName : BaseClass
    {
        // Constantes
        private const float SOME_CONSTANT = 1.0f;

        // Campos
        private int _privateField;

        // Propriedades
        public int PublicProperty { get; set; }

        // Construtores
        public ClassName()
        {
            // Inicialização
        }

        // Métodos públicos (ordem: mais importantes primeiro)
        public void ImportantMethod()
        {
            // Implementação
        }

        // Métodos privados
        private void HelperMethod()
        {
            // Implementação
        }

        // Overrides (ModPlayer, GlobalItem, etc.)
        public override void SetDefaults()
        {
            base.SetDefaults();
            // Customizações
        }
    }
}
```

### Documentação
- **Todos os métodos públicos** devem ter `<summary>` XML documentation
- **Parâmetros e retornos** devem ser documentados
- **Lógica complexa** deve ter comentários explicativos
- **TODOs** devem ser marcados com `// TODO: descrição`

## Fluxos de Desenvolvimento

### Adicionando um Novo Item
1. Criar classe em `Content/Items/`
2. Herdar de `ModItem`
3. Implementar `SetDefaults()`, `SetStaticDefaults()`
4. Adicionar receita se necessário
5. Registrar texturas (40x40 pixels)

### Adicionando uma Nova Habilidade
1. Definir em `SignatureAbilitySystem.cs`
2. Adicionar configuração em `ServerConfig.cs`
3. Implementar lógica de ativação
4. Adicionar efeitos visuais em `SignatureGlobalItem.cs`
5. Testar balanceamento

### Modificando Zonas Radioativas
1. **Lógica**: `LeveledEnemySystem.cs`
2. **Visuais**: `RadioactiveZoneVisuals.cs`
3. **Debuffs**: `RadioactiveZoneDebuffs.cs`
4. Testar todos os tiers (1-5)
5. Verificar countdown final

### Sistema de Rede (Multiplayer)
- Usar `Mod.GetPacket()` para enviar dados
- Implementar handlers em `SignatureEquipmentDeluxe.cs`
- Sincronizar apenas dados essenciais
- Testar em servidor dedicado

## Debugging e Testes

### Ferramentas de Debug
```csharp
// Logs no console
Main.NewText("Debug message", Color.Yellow);

// Verificar valores em tempo real
if (Main.netMode == NetmodeID.Server)
    Console.WriteLine($"Value: {someValue}");

// Breakpoints no IDE
System.Diagnostics.Debugger.Break();
```

### Testes Essenciais
1. **Singleplayer**: Todas as funcionalidades básicas
2. **Multiplayer**: Sincronização de dados
3. **Edge Cases**: Máximo de itens, overflow de XP
4. **Performance**: Sem lag em combate intenso
5. **Save/Load**: Persistência de dados

### Comandos de Teste
```csharp
// Spawn itens de teste
/give SignatureEquipmentDeluxe:SignatureManagementTome
/give SignatureEquipmentDeluxe:XPBooster

// Debug XP
// Implementar comando customizado se necessário
```

## Balanceamento

### Valores Base (Configuráveis)
- **XP por dano**: 0.1f (configurável)
- **XP por kill**: 10-100 baseado no nível do inimigo
- **XP por boss**: 500-2000
- **Bônus por nível**: +1% dano, +0.2% velocidade
- **Bônus por prestígio**: +5% dano, +2% defesa

### Tier de Zonas
- **Duração**: 10 minutos fixos
- **Tiers**: 1-5, cada 2 minutos
- **Raio**: +10% por tier
- **Partículas**: +35% por tier

### Runas
- **Drop chance**: 5% base + 0.2% por nível
- **Tipos**: 7 runas elementais + maldições

## Build e Deploy

### Build Local
```bash
cd "mod directory"
dotnet build
```

### Build via tModLoader
1. Abrir tModLoader
2. Workshop → Develop Mods
3. Selecionar SignatureEquipmentDeluxe
4. Build + Reload

### Publicação
1. Testar extensivamente
2. Atualizar versão em `build.txt`
3. Comprimir arquivos
4. Upload para Steam Workshop

## Troubleshooting Comum

### Mod não carrega
- Verificar dependências do tModLoader
- Checar erros de compilação
- Limpar cache do mod

### XP não ganha
- Confirmar item vinculado (tooltip)
- Verificar `ServerConfig.EnableSignatureSystem`
- Debug com logs

### Zonas não funcionam
- Verificar `ServerConfig.EnableLeveledEnemies`
- Checar se item tem runas de maldição
- Testar em singleplayer

### Multiplayer desync
- Verificar sincronização de pacotes
- Testar em servidor local
- Debug com logs de rede

## Boas Práticas

### Performance
- Evitar loops em `Update()` methods
- Usar timers para efeitos periódicos
- Cache de cálculos caros
- Limpar referências quando não usadas

### Manutenibilidade
- Código modular e reutilizável
- Configurações externalizáveis
- Documentação atualizada
- Testes automatizados quando possível

### Jogabilidade
- Balanceamento cuidadoso
- Feedback visual/sonoro claro
- Tutoriais implícitos
- Opções de acessibilidade

## Contribuição

### Processo
1. Criar issue descrevendo a feature/bug
2. Criar branch `feature/nome-da-feature`
3. Implementar com testes
4. Pull request com descrição detalhada
5. Code review e merge

### Guidelines
- Commits pequenos e descritivos
- Branches limpas (rebase se necessário)
- Testes antes de commit
- Documentação atualizada

## Referências

### APIs do tModLoader
- [Documentação Oficial](https://github.com/tModLoader/tModLoader/wiki)
- [Exemplos](https://github.com/tModLoader/tModLoader/tree/1.4/ExampleMod)

### Terraria Internals
- [Wiki do tModLoader](https://github.com/tModLoader/tModLoader/wiki)
- [Fórum da comunidade](https://forums.terraria.org/index.php?forums/modding.71/)

### Ferramentas
- **IDE**: Visual Studio 2022 ou Rider
- **Version Control**: Git
- **Asset Editor**: Paint.NET ou Aseprite para texturas

---

*Estas instruções são mantidas atualizadas. Para mudanças recentes, consulte o changelog e commits.*