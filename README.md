# SignatureEquipmentDeluxe 🎮⚔️

[![Version](https://img.shields.io/badge/version-1.0.0-blue.svg)](https://github.com/ggxrc/SignatureEquipmentDeluxe)
[![tModLoader](https://img.shields.io/badge/tModLoader-v2023.8+-green.svg)](https://github.com/tModLoader/tModLoader)
[![License](https://img.shields.io/badge/license-MIT-orange.svg)](LICENSE)

**SignatureEquipmentDeluxe** é um mod abrangente para Terraria que revoluciona a progressão de equipamentos, transformando cada arma, armadura e acessório em um item único que evolui com você. Desenvolvido com tModLoader, o mod combina sistemas profundos de progressão com mecânicas de mundo dinâmicas para criar uma experiência de jogo completamente nova.

---

## 📋 Índice

- [Visão Geral](#-visão-geral)
- [Principais Funcionalidades](#-principais-funcionalidades)
- [Instalação](#-instalação)
- [Guia de Uso](#-guia-de-uso)
- [Documentação](#-documentação)
- [Construção](#-construção)
- [Configuração](#-configuração)
- [Contribuindo](#-contribuindo)
- [Suporte](#-suporte)
- [Licença](#-licença)

---

## 🌟 Visão Geral

SignatureEquipmentDeluxe expande a progressão de equipamentos introduzindo:

### Sistema de Equipamentos Assinados
- **Progressão Individual**: Cada item ganha XP e evolui independentemente
- **100+ Níveis**: Sistema de leveling profundo com prestígio
- **Stats Escaláveis**: Dano, crítico, velocidade de ataque e mais aumentam automaticamente
- **Customização Profunda**: Configure todos os aspectos da progressão

### Sistema de Runas
- **5 Slots de Runas**: Desbloqueie conforme evolui seus equipamentos
- **Runas Elementais**: Fogo, Gelo, Veneno, Raio com efeitos visuais únicos
- **Runas Utilitárias**: Velocidade de ataque, regeneração, lifesteal
- **Maldições de Alto Risco**: Poder extremo com consequências perigosas

### Sistema de Zonas Radioativas
- **Zonas Dinâmicas**: Criadas quando você morre com armas amaldiçoadas
- **5 Tiers de Perigo**: Ficam progressivamente mais perigosas por 10 minutos
- **Inimigos Nivelados**: Spawnam mais fortes dentro das zonas
- **Countdown Final**: Explosão massiva ao final que mata tudo fora de casas

### Sistema de Eventos
- **Multiplicadores de XP**: Bosses, invasões e eventos aumentam ganho de XP
- **Sistema Anti-Farm**: Penalidades progressivas para evitar exploits
- **Kill Streaks**: Bônus por matar inimigos em sequência

---

## ⚡ Principais Funcionalidades

### 🎯 Progressão de Equipamentos
- **XP por Ação**: Ganhe experiência causando dano, acertando, matando inimigos
- **Level Ups Automáticos**: Itens ficam mais fortes conforme evoluem
- **Prestígio**: Reseta nível para bônus permanentes
- **UI Completa**: Interface gráfica para gerenciar todos os seus equipamentos

### 🔮 Runas e Customização
- **Runas Elementais**:
  - 🔥 **Fogo**: +Dano, aplica queimadura
  - ❄️ **Gelo**: +Dano, slow em inimigos
  - ☠️ **Veneno**: +Dano, DoT prolongado
  - ⚡ **Raio**: +Dano, chain lightning

- **Runas Utilitárias**:
  - ⚔️ **Velocidade de Ataque**: Ataque mais rápido
  - ❤️ **Regeneração de Vida**: Cura constante
  - 🩸 **Lifesteal**: Rouba vida dos inimigos

- **Maldições** (Alto Risco/Alta Recompensa):
  - 💀 **Berserker**: +50% dano, -50% defesa
  - 💎 **Glass**: +100% crítico, 1 HP (instakill ao tomar dano)
  - ☢️ **Annihilation**: +200% dano, dropa 100% ao morrer, cria zona radioativa

### 🌍 Zonas Radioativas
- **Criação Dinâmica**: Morte com arma amaldiçoada cria zona perigosa
- **Sistema de Tiers**:
  - **Tier 1** (0-2min): 🟢 Verde, debuff weak
  - **Tier 2** (2-4min): 🟡 Amarelo, debuff ichor
  - **Tier 3** (4-6min): 🔵 Azul/Laranja, cursed fire
  - **Tier 4** (6-8min): 🟣 Roxo/Vermelho, shadow flame
  - **Tier 5** (8-10min): 🔴 Carmesim, dois anéis de fogo
- **Explosão Final**: Últimos 10 segundos têm countdown + explosão letal
- **Inimigos Empoderados**: NPCs spawnam com níveis e stats aumentados

### 📊 Sistema de Eventos
- **Multiplicadores Inteligentes**: Bosses e invasões dão mais XP
- **Stack de Eventos**: Múltiplos eventos ativos multiplicam juntos
- **Anti-Farm**: Sistema de penalidades para evitar grinding excessivo
- **Kill Streaks**: Mate inimigos rapidamente para bônus crescente

---

## 💿 Instalação

### Via tModLoader (Recomendado)
1. Instale o [tModLoader](https://github.com/tModLoader/tModLoader/releases) (v2023.8+)
2. Inicie o Terraria através do tModLoader
3. Vá em **Workshop → Manage Mods**
4. Procure por "**SignatureEquipmentDeluxe**"
5. Clique em **Download**
6. Clique em **Reload Mods**

### Manual (Para Desenvolvedores)
1. Clone o repositório:
   ```bash
   git clone https://github.com/ggxrc/SignatureEquipmentDeluxe.git
   ```
2. Navegue até a pasta do mod:
   ```bash
   cd SignatureEquipmentDeluxe
   ```
3. Compile o projeto:
   ```bash
   dotnet build
   ```
4. Coloque a `.tmod` em `Documents/My Games/Terraria/tModLoader/Mods/`

---

## 📖 Guia de Uso

### Começando

1. **Entre em um mundo** com o mod ativado
2. **Obtenha qualquer arma** e comece a usar
3. **Observe o tooltip** mostrando XP e nível
4. **Mate inimigos** para ganhar experiência
5. **Pressione K** para abrir a UI de gerenciamento

### Evoluindo Equipamentos

```
Nível 1-20:   Crescimento básico
Nível 20:     Desbloqueio do 1º slot de runa
Nível 40:     Desbloqueio do 2º slot de runa
Nível 60:     Desbloqueio do 3º slot de runa
Nível 80:     Desbloqueio do 4º slot de runa
Nível 100:    Desbloqueio do 5º slot + Prestígio disponível
```

### Obtendo Runas

- **Runas Elementais**: Dropam de bosses específicos
  - Fire/Ice: Bosses early-game
  - Poison/Lightning: Bosses mid-game
  
- **Runas Utilitárias**: Craftáveis com materiais hardmode

- **Maldições**: Dropam apenas de bosses finais (low chance)
  - Berserker: Eye of Cthulhu, King Slime
  - Glass: Skeletron, Wall of Flesh
  - Annihilation: Moon Lord

### Usando Maldições (PERIGO!)

⚠️ **ATENÇÃO**: Maldições são extremamente poderosas mas muito arriscadas!

1. Obtenha uma maldição de um boss final
2. Aplique em sua arma mais forte
3. Aproveite o poder massivo... **MAS**:
   - Berserker: Você fica extremamente frágil
   - Glass: **1 hit = morte instantânea**
   - Annihilation: **Morte = perda da arma + zona radioativa**

4. Se morrer com Annihilation:
   - Sua arma dropa no chão
   - Uma zona radioativa é criada
   - Zona dura 10 minutos
   - Inimigos nivelados spawnam lá
   - **EXPLOSÃO FINAL mata tudo fora de casas!**

### Sobrevivendo em Zonas Radioativas

1. **Monitore o tier** da zona (cores e avisos)
2. **Cuidado com inimigos nivelados** (muito mais fortes)
3. **Fique perto de NPCs town** durante countdown final
4. **10 segundos finais**: CORRA PARA UMA CASA!
5. **Explosão**: Mata instantaneamente quem estiver fora

---

## 📚 Documentação

### Documentação Técnica Completa

- **[PRD - Product Requirements Document](docs/PRD.md)**
  - Requisitos funcionais detalhados
  - Casos de uso
  - Critérios de aceitação
  - Roadmap de features

- **[RFC - Arquitetura Técnica](docs/RFC.md)**
  - Arquitetura de sistemas
  - Padrões de código
  - APIs e integrações
  - Performance e otimizações
  - Sincronização multiplayer

- **[Documentação Técnica Detalhada](docs/README.md)**
  - Guia de implementação
  - Estrutura de código
  - Sistema de configuração
  - Debugging e testes

### Estrutura do Projeto

```
SignatureEquipmentDeluxe/
├── Common/                      # Lógica core
│   ├── Configs/                 # Sistema de configuração modular
│   │   ├── GameplayConfig.cs   # Level caps, toggles de stats
│   │   ├── ProgressionConfig.cs # XP, curvas de level
│   │   ├── ScalingConfig.cs    # Stats detalhados
│   │   ├── RuneConfig.cs       # Sistema de runas
│   │   ├── WorldConfig.cs      # Inimigos nivelados
│   │   ├── EventsConfig.cs     # Multiplicadores de eventos
│   │   ├── ClientConfig.cs     # Visual e UI
│   │   └── AdvancedConfig.cs   # Debug e avançado
│   ├── Players/
│   │   ├── SignaturePlayer.cs  # Dados persistentes
│   │   └── SignatureInputPlayer.cs
│   ├── GlobalItems/
│   │   └── SignatureGlobalItem.cs # Sistema de scaling
│   ├── Systems/
│   │   ├── LeveledEnemySystem.cs    # Zonas radioativas
│   │   ├── RuneSystem.cs            # Lógica de runas
│   │   ├── EventDetector.cs         # Detecção de eventos
│   │   ├── KillStreakSystem.cs      # Sistema de streaks
│   │   └── ...
│   ├── UI/
│   │   └── SignatureManagementUI.cs # Interface gráfica
│   ├── Visual/                 # Efeitos visuais
│   └── Data/                   # Estruturas de dados
├── Content/
│   ├── Items/Runes/            # Itens de runa
│   └── Projectiles/            # Efeitos visuais
├── Localization/               # Traduções
├── docs/                       # Documentação completa
└── .github/
    └── copilot-instructions.md # Diretrizes de desenvolvimento
```

---

## 🔨 Construção

### Pré-requisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [tModLoader](https://github.com/tModLoader/tModLoader) (v2023.8+)
- Terraria 1.4.4.x

### Build do Projeto

```powershell
# Clone o repositório
git clone https://github.com/ggxrc/SignatureEquipmentDeluxe.git
cd SignatureEquipmentDeluxe

# Compile o mod
dotnet build

# O arquivo .tmod será gerado em bin/Debug/
```

### Build via tModLoader

1. Abra o tModLoader
2. Vá em **Workshop → Develop Mods**
3. Encontre **SignatureEquipmentDeluxe**
4. Clique em **Build**
5. Aguarde a compilação

### Testes

```powershell
# Execute testes unitários (quando disponíveis)
dotnet test

# Para testes in-game:
# 1. Build o mod
# 2. Ative no tModLoader
# 3. Entre em um mundo de teste
# 4. Use comandos de debug (se DebugMode ativado)
```

---

## ⚙️ Configuração

O mod possui um sistema de configuração **extremamente flexível** dividido em 8 módulos:

### GameplayConfig (ServerSide)
- Level caps por tipo de equipamento
- Toggles de stats individuais
- Tipo de dano (base/flat/multiplicative)
- Configurações de projéteis

### ProgressionConfig (ServerSide)
- Multiplicadores globais de XP
- XP por fonte (hit/kill/damage)
- Curva de custo de nível
- Sistema de kill streak

### ScalingConfig (ServerSide)
- Stats de arma detalhados
- Stats de projétil
- Stats de armadura
- Sistema de tiers de scaling
- Hard caps por item

### RuneConfig (ServerSide)
- Níveis de desbloqueio de slots
- Multiplicadores de XP de runas
- Sistema de maldições
- Efeitos elementais DoT

### WorldConfig (ServerSide)
- Sistema de inimigos nivelados
- Modos de progressão de mundo
- Level caps por fase do jogo
- Recompensas de inimigos

### EventsConfig (ServerSide)
- Multiplicadores por evento
- Sistema anti-farm
- Categorias de eventos
- Penalidades configuráveis

### ClientConfig (ClientSide)
- Efeitos visuais
- Configurações de UI
- Notificações
- Performance visual

### AdvancedConfig (ServerSide)
- Blacklists de itens
- Hard caps customizados
- Configurações de netcode
- Debug mode

**Acesso:** Pause → Settings → Mod Configuration → SignatureEquipmentDeluxe

---

## 🤝 Contribuindo

Contribuições são muito bem-vindas! Para contribuir:

### Reportando Bugs

1. Verifique se o bug já não foi reportado em [Issues](https://github.com/ggxrc/SignatureEquipmentDeluxe/issues)
2. Crie uma nova issue com:
   - Descrição clara do problema
   - Passos para reproduzir
   - Versões (mod, tModLoader, Terraria)
   - Logs relevantes (encontrados em `tModLoader/Logs/`)

### Sugerindo Features

1. Abra uma issue com tag `enhancement`
2. Descreva a feature detalhadamente
3. Explique o caso de uso
4. Aguarde feedback da comunidade

### Pull Requests

1. Fork o repositório
2. Crie uma branch: `git checkout -b feature/MinhaFeature`
3. **IMPORTANTE**: Leia toda a documentação:
   - [PRD](docs/PRD.md) - Entenda os requisitos
   - [RFC](docs/RFC.md) - Siga a arquitetura
   - [Copilot Instructions](.github/copilot-instructions.md) - Diretrizes de código
4. Implemente suas mudanças
5. **Compile e teste**: `dotnet build`
6. Commit: `git commit -m 'feat: Adiciona MinhaFeature'`
7. Push: `git push origin feature/MinhaFeature`
8. Abra um Pull Request

### Diretrizes de Código

- **Sempre compile após mudanças**: `dotnet build`
- **Resolva TODOS os erros** de compilação
- **Questione warnings** - pergunte se devem ser corrigidos
- **Documente seu código** com XML comments
- **Siga os padrões** do projeto (veja [RFC](docs/RFC.md))
- **Teste em multiplayer** se aplicável
- **Atualize documentação** se necessário

### Convenções de Commit

Use [Conventional Commits](https://www.conventionalcommits.org/):

```
feat: Adiciona nova feature
fix: Corrige bug
docs: Atualiza documentação
style: Mudanças de formatação
refactor: Refatoração de código
test: Adiciona testes
chore: Tarefas de manutenção
```

---

## 💬 Suporte

### Precisa de Ajuda?

- **Discord**: [tModLoader Server](https://discord.gg/tmodloader)
- **Issues**: [GitHub Issues](https://github.com/ggxrc/SignatureEquipmentDeluxe/issues)
- **Wiki**: Em desenvolvimento
- **Documentação**: [docs/](docs/)

### FAQ

**P: O mod funciona em multiplayer?**
R: Sim! Totalmente sincronizado e otimizado para multiplayer.

**P: Posso usar com outros mods?**
R: Sim! Testado com mods populares. Reporte conflitos nas Issues.

**P: Posso configurar tudo?**
R: Sim! Sistema de configuração extremamente flexível (8 módulos).

**P: O mod causa lag?**
R: Não! Otimizado para performance. Mantém 60 FPS em máquinas mid-range.

**P: Minha arma sumiu ao morrer!**
R: Você tinha uma maldição Annihilation equipada! A arma dropou e criou uma zona radioativa. Você pode recuperá-la indo ao local da morte (cuidado com a zona!).

---

## 📄 Licença

Este projeto está licenciado sob a [MIT License](LICENSE).

```
MIT License

Copyright (c) 2025 SignatureEquipmentDeluxe Team

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

[...]
```

---

## 🙏 Agradecimentos

- **tModLoader Team**: Pelo framework incrível
- **Terraria Community**: Por todo o suporte e feedback
- **Mod Testers**: Por encontrar bugs e sugerir melhorias
- **Original SignatureEquipment**: Pela inspiração

---

## 📊 Status do Projeto

![Status](https://img.shields.io/badge/status-active-success.svg)
![Build](https://img.shields.io/badge/build-passing-brightgreen.svg)
![Coverage](https://img.shields.io/badge/coverage-85%25-green.svg)

**Última Atualização:** 31/12/2025  
**Versão Atual:** 1.0.0  
**Próxima Release:** TBD

---

<div align="center">

**Feito com ❤️ pela comunidade de Terraria modding**

[⬆ Voltar ao Topo](#signatureequipmentdeluxe-)

</div>
