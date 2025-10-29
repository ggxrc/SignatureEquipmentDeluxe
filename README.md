# SignatureEquipmentDeluxe

SignatureEquipmentDeluxe é um mod para Terraria, desenvolvido com o framework tModLoader. Ele expande o sistema de progressão de equipamentos do mod original SignatureEquipment, introduzindo mecânicas avançadas como zonas radioativas, inimigos com níveis, efeitos visuais dinâmicos e armas amaldiçoadas.

## Principais Funcionalidades

- **Zonas Radioativas**: Áreas onde inimigos ganham níveis e efeitos visuais intensos são aplicados.
- **Inimigos com Níveis**: Inimigos podem ganhar níveis, aumentando sua força e recompensas.
- **Armas Amaldiçoadas**: Armas amaldiçoadas podem ser dropadas ao morrer, criando zonas radioativas.
- **Efeitos Visuais e Sonoros**: Feedback visual e sonoro dinâmico baseado em eventos do jogo.
- **Runas e Maldições**: Sistema de runas que adiciona habilidades e maldições aos equipamentos.

## Estrutura do Projeto

- **Common/**: Contém sistemas principais como `LeveledEnemySystem`, `RadioactiveZoneVisuals`, e `CurseDeathSystem`.
- **Content/**: Inclui itens, projéteis e outros conteúdos específicos do jogo.
- **Localization/**: Arquivos `.hjson` para localização em diferentes idiomas.
- **Properties/**: Configurações do projeto, como `launchSettings.json`.

## Como Construir o Mod

1. Navegue até o diretório raiz do mod.
2. Execute o comando:
   ```powershell
   dotnet build
   ```

## Testando o Mod

1. Inicie o Terraria com o tModLoader.
2. Ative o mod SignatureEquipmentDeluxe no menu Mods.
3. Teste as funcionalidades no jogo, como zonas radioativas e inimigos com níveis.

## Contribuindo

Contribuições são bem-vindas! Certifique-se de seguir as convenções do projeto e testar suas alterações antes de enviar um pull request.
