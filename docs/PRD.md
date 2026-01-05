# PRD - Product Requirements Document
## SignatureEquipmentDeluxe

**Versão:** 1.0  
**Data:** 31/12/2025  
**Autor:** Equipe SignatureEquipmentDeluxe  
**Status:** Em Desenvolvimento

---

## 1. Visão Geral

### 1.1 Propósito do Produto
SignatureEquipmentDeluxe é um mod para Terraria que revoluciona a progressão de equipamentos, transformando cada arma, armadura e acessório em um item único que evolui com o jogador. O mod introduz múltiplos sistemas integrados que recompensam o uso contínuo de equipamentos, criando um jogo de longo prazo mais engajador.

### 1.2 Objetivos do Produto
- **Progressão Significativa**: Criar um sistema de evolução de equipamentos que faça cada item único e valioso
- **Recompensa por Investimento**: Incentivar o jogador a se dedicar a equipamentos específicos através de recompensas tangíveis
- **Variedade Tática**: Oferecer múltiplas rotas de customização através de runas e maldições
- **Desafio Dinâmico**: Introduzir sistemas de mundo que aumentam a dificuldade e recompensas
- **Rejogabilidade**: Criar razões para múltiplas partidas através de diferentes builds e estratégias

### 1.3 Público-Alvo
- **Jogadores Casuais**: Que querem progressão contínua sem complexidade excessiva
- **Jogadores Hardcore**: Que buscam otimização e min-maxing de builds
- **Completionistas**: Que querem colecionar e maximizar todos os equipamentos
- **Multiplicayer**: Grupos que querem sincronização perfeita de progressão

---

## 2. Requisitos Funcionais

### 2.1 Sistema de Equipamentos Assinados

#### RF-001: Progressão de Nível
**Prioridade:** Crítica  
**Descrição:** Equipamentos ganham níveis através do uso e combate.

**Critérios de Aceitação:**
- [ ] Armas ganham XP ao causar dano, acertar e matar inimigos
- [ ] Armaduras ganham XP ao receber dano e bloquear ataques
- [ ] Acessórios ganham XP passivamente quando equipados
- [ ] Níveis máximos configuráveis por tipo de equipamento (padrão: 100+)
- [ ] Sistema de prestígio após nível máximo
- [ ] XP visível em tooltips em tempo real
- [ ] Sincronização multiplayer de XP e níveis

**Dependências:** Nenhuma

---

#### RF-002: Sistema de Stats Escaláveis
**Prioridade:** Crítica  
**Descrição:** Equipamentos ganham bônus de stats conforme sobem de nível.

**Critérios de Aceitação:**
- [ ] **Armas:**
  - Dano base aumenta por nível
  - Chance de crítico aumenta progressivamente
  - Use Time/Animation reduz (ataque mais rápido)
  - Tamanho de armas melee aumenta
  - Redução de custo de mana e consumo de munição
- [ ] **Armaduras:**
  - Defesa aumenta por nível
  - Vida máxima aumenta
  - Regeneração melhorada
- [ ] **Acessórios:**
  - Velocidade de movimento aumenta
  - Bônus específicos ao tipo de acessório
- [ ] **Projéteis:**
  - Tamanho aumenta
  - Velocidade aumenta
  - Penetração aumenta
  - Tempo de vida aumenta
  - Chance de projéteis adicionais
- [ ] Sistema de tiers de scaling configurável
- [ ] Hard caps por item configuráveis
- [ ] Blacklist de itens que não devem receber bônus

**Dependências:** RF-001

---

#### RF-003: Interface de Gerenciamento
**Prioridade:** Alta  
**Descrição:** UI completa para visualizar e gerenciar todos os equipamentos assinados.

**Critérios de Aceitação:**
- [ ] Tecla de atalho configurável (padrão: K)
- [ ] Lista de todos equipamentos assinados
- [ ] Visualização detalhada de stats por item
- [ ] Barra de progresso de XP visual
- [ ] Informações de runas equipadas
- [ ] Indicador de maldições ativas
- [ ] Sistema de filtros (armas/armaduras/acessórios)
- [ ] Botão de remoção de assinatura (com confirmação)
- [ ] Design responsivo e intuitivo

**Dependências:** RF-001, RF-002

---

### 2.2 Sistema de Runas

#### RF-004: Sistema de Slots de Runas
**Prioridade:** Alta  
**Descrição:** Equipamentos desbloqueiam slots de runas conforme sobem de nível.

**Critérios de Aceitação:**
- [ ] 5 slots de runas desbloqueáveis
- [ ] Desbloqueio progressivo (níveis 20, 40, 60, 80, 100 configuráveis)
- [ ] Cada runa tem nível próprio (evolui com uso)
- [ ] Runas ganham XP junto com o equipamento
- [ ] Sistema de XP separado por runa
- [ ] Máximo de nível da runa vinculado ao nível da arma
- [ ] Visualização de slots na UI
- [ ] Indicação clara de slots disponíveis vs. usados

**Dependências:** RF-001

---

#### RF-005: Runas Elementais
**Prioridade:** Alta  
**Descrição:** Runas que adicionam efeitos elementais aos ataques.

**Critérios de Aceitação:**
- [ ] **FireRune:**
  - Adiciona dano de fogo escalável
  - Aplica debuff de queimadura (DoT)
  - Efeitos visuais de fogo em projéteis
  - Resistência a fogo aumentada
- [ ] **IceRune:**
  - Adiciona dano de gelo escalável
  - Aplica slow em inimigos
  - Efeitos visuais de gelo em projéteis
  - Chance de congelar inimigos
- [ ] **PoisonRune:**
  - Adiciona dano venenoso escalável
  - Aplica debuff de veneno (DoT)
  - Efeitos visuais de veneno em projéteis
  - Dano over time prolongado
- [ ] **LightningRune:**
  - Adiciona dano elétrico escalável
  - Chain lightning (pula entre inimigos)
  - Efeitos visuais de raio em projéteis
  - Stun temporário em inimigos
- [ ] Drops de bosses específicos
- [ ] Efeitos visuais distintos por elemento
- [ ] Som de aplicação de runa

**Dependências:** RF-004

---

#### RF-006: Runas Utilitárias
**Prioridade:** Média  
**Descrição:** Runas que melhoram aspectos gerais do combate.

**Critérios de Aceitação:**
- [ ] **AttackSpeedRune:**
  - Aumenta velocidade de ataque
  - Reduz use time/animation
  - Bônus escalável por nível
- [ ] **LifeRegenRune:**
  - Aumenta regeneração de vida
  - Cura passiva constante
  - Escala com nível da runa
- [ ] **LifestealRune:**
  - Rouba vida ao causar dano
  - Percentual configurável
  - Cooldown entre ativações
  - Efeito visual de roubo de vida
- [ ] Drops balanceados ao longo do jogo
- [ ] Tooltips informativos

**Dependências:** RF-004

---

#### RF-007: Sistema de Maldições
**Prioridade:** Alta  
**Descrição:** Maldições são runas poderosas com efeitos negativos.

**Critérios de Aceitação:**
- [ ] **CurseBerserker:**
  - +50% dano
  - -50% defesa
  - Efeitos visuais agressivos (aura vermelha)
- [ ] **CurseGlass:**
  - +100% critical strike chance
  - Dano recebido mata instantaneamente (1 HP)
  - Efeitos visuais frágeis (aura cristalina)
- [ ] **CurseAnnihilation:**
  - +200% dano
  - Arma dropa 100% ao morrer
  - Cria zona radioativa ao morrer
  - Efeitos visuais apocalípticos (aura roxa/negra)
- [ ] Drops exclusivos de bosses finais
- [ ] Avisos claros sobre perigos
- [ ] Sistema de remoção de maldições (com custo)

**Dependências:** RF-004

---

#### RF-008: Sistema de Remoção de Runas
**Prioridade:** Média  
**Descrição:** Ferramenta para remover runas de equipamentos.

**Critérios de Aceitação:**
- [ ] Item "Rune Remover" craftável
- [ ] Modo de seleção com click em equipamentos
- [ ] UI de seleção de runa a remover
- [ ] Chance de perder níveis ao remover maldições (20% padrão)
- [ ] Perda de níveis configurável (12.5% dos níveis padrão)
- [ ] Mensagens de confirmação antes de remover
- [ ] Cancelamento com ESC
- [ ] Efeitos visuais de remoção

**Dependências:** RF-004, RF-005, RF-006, RF-007

---

### 2.3 Sistema de Zonas Radioativas

#### RF-009: Criação de Zonas
**Prioridade:** Alta  
**Descrição:** Zonas radioativas são criadas quando jogador morre com maldição.

**Critérios de Aceitação:**
- [ ] Trigger: Morte com arma amaldiçoada
- [ ] Arma dropa no local da morte
- [ ] Animação de explosão inicial
- [ ] Partículas radioativas visíveis
- [ ] Raio inicial de 375 tiles (configurável)
- [ ] Duração fixa de 10 minutos
- [ ] Sincronização multiplayer (servidor autoritativo)
- [ ] Máximo de zonas ativas simultâneas
- [ ] Efeito sonoro de criação

**Dependências:** RF-007

---

#### RF-010: Sistema de Tiers Progressivos
**Prioridade:** Alta  
**Descrição:** Zonas ficam progressivamente mais perigosas com o tempo.

**Critérios de Aceitação:**
- [ ] **Tier 1 (0-2min):**
  - Cor: Verde
  - Debuff: Weak
  - Multiplicador: 1.0x
- [ ] **Tier 2 (2-4min):**
  - Cor: Amarelo
  - Debuff: Ichor
  - Multiplicador: 1.1x
  - Raio +10%
- [ ] **Tier 3 (4-6min):**
  - Cor: Azul/Laranja
  - Debuff: Cursed Fire (epicentro)
  - Multiplicador: 1.2x
  - Raio +20%
- [ ] **Tier 4 (6-8min):**
  - Cor: Roxo/Vermelho
  - Debuff: Shadow Flame (antes do epicentro)
  - Multiplicador: 1.3x
  - Raio +30%
- [ ] **Tier 5 (8-10min):**
  - Cor: Carmesim intenso
  - Debuff: Dois anéis de fogo
  - Multiplicador: 1.5x
  - Raio +40%
- [ ] Transições visuais suaves entre tiers
- [ ] Avisos sonoros de mudança de tier
- [ ] Notificações de texto para jogadores na zona

**Dependências:** RF-009

---

#### RF-011: Countdown Final
**Prioridade:** Alta  
**Descrição:** Últimos 10 segundos da zona com mecânicas especiais.

**Critérios de Aceitação:**
- [ ] Aviso em texto vermelho: "10 segundos restantes!"
- [ ] Todas as partículas se movem para o centro
- [ ] Efeito de implosão visual
- [ ] Som de contagem regressiva
- [ ] Explosão massiva ao final
- [ ] Mata todos os jogadores fora de casas
- [ ] Detecção de "casa" via NPCs town próximos
- [ ] Dano escalado com tier da zona
- [ ] Efeito de câmera shake
- [ ] Clear de todas as partículas após explosão

**Dependências:** RF-009, RF-010

---

#### RF-012: Inimigos Nivelados
**Prioridade:** Alta  
**Descrição:** Inimigos dentro da zona ganham níveis e ficam mais fortes.

**Critérios de Aceitação:**
- [ ] 15% de chance de spawn nivelado (configurável)
- [ ] Nível baseado no tier da zona
- [ ] +35% vida base
- [ ] Dano escalado dinamicamente
- [ ] Resistência baseada em nível
- [ ] Penetração de defesa (ignora % baseado em diferença de nível)
- [ ] Inimigos confinados à zona (não podem sair)
- [ ] Indicador visual de nível acima do inimigo
- [ ] Cor diferenciada por tier
- [ ] Drops aumentados:
  - XP bônus (5% por nível configurável)
  - Chance de dropar runas
  - Loot table melhorada
- [ ] Efeitos visuais de poder (aura)

**Dependências:** RF-009

---

#### RF-013: Debuffs por Proximidade
**Prioridade:** Média  
**Descrição:** Jogadores sofrem debuffs baseados na proximidade do epicentro.

**Critérios de Aceitação:**
- [ ] Sistema de anéis concêntricos
- [ ] Debuffs mais fortes perto do centro
- [ ] Debuffs variam por tier
- [ ] Indicação visual de intensidade
- [ ] Tooltip mostrando debuffs ativos
- [ ] Imunidade a debuffs com certas runas
- [ ] Redução de debuffs com resistências

**Dependências:** RF-009, RF-010

---

### 2.4 Sistema de Eventos e XP

#### RF-014: Multiplicadores de Evento
**Prioridade:** Média  
**Descrição:** Eventos do jogo modificam ganho de XP.

**Critérios de Aceitação:**
- [ ] **Bosses Pre-Hardmode:** 1.25x - 1.75x
- [ ] **Bosses Hardmode:** 1.5x - 2.5x
- [ ] **Invasões:** 1.25x - 2.0x
- [ ] **Luas Especiais:** 1.25x - 2.0x
- [ ] **Eventos Climáticos:** 1.1x - 1.2x
- [ ] **Eventos Especiais:** 1.25x
- [ ] Stack de múltiplos eventos
- [ ] Notificações de multiplicadores ativos
- [ ] Efeitos visuais indicando evento ativo
- [ ] Configurável por tipo de evento

**Dependências:** RF-001

---

#### RF-015: Sistema Anti-Farm
**Prioridade:** Média  
**Descrição:** Penalidade progressiva para evitar farming excessivo de eventos.

**Critérios de Aceitação:**
- [ ] Tracker de repetição de eventos
- [ ] Penalidade acumulativa por categoria:
  - Bosses: -10% por repetição (máx -50%)
  - Invasões: -10% por repetição (máx -50%)
  - Outros: Opcional por categoria
- [ ] Reset de penalidades após tempo (configurável)
- [ ] Notificações de penalidade ativa
- [ ] Indicador visual de eficiência reduzida
- [ ] Configurável por categoria de evento
- [ ] Salvamento de dados de tracker

**Dependências:** RF-014

---

#### RF-016: Sistema de Kill Streak
**Prioridade:** Baixa  
**Descrição:** Bônus de XP por matar inimigos em sequência.

**Critérios de Aceitação:**
- [ ] Contagem de kills consecutivos
- [ ] Bônus de XP crescente (+1% por kill, máx 50%)
- [ ] Reset após tempo sem kills (5 segundos configurável)
- [ ] HUD visual mostrando streak atual
- [ ] Efeitos visuais de streak ativo
- [ ] Som especial em marcos (10x, 25x, 50x)
- [ ] Mensagens de conquista
- [ ] Configurável (on/off)

**Dependências:** RF-001

---

### 2.5 Sistema de Configuração

#### RF-017: Configurações Modulares
**Prioridade:** Crítica  
**Descrição:** Sistema de configuração completo e modular.

**Critérios de Aceitação:**
- [ ] **GameplayConfig:**
  - Level caps por tipo
  - Toggles de stats
  - Tipo de scaling (base/flat/mult)
- [ ] **ProgressionConfig:**
  - Multiplicadores globais
  - XP por ação
  - Curva de custo de nível
  - Kill streak settings
- [ ] **ScalingConfig:**
  - Stats de arma detalhados
  - Stats de projétil
  - Stats de armadura
  - Tiers de scaling
  - Hard caps por item
- [ ] **RuneConfig:**
  - Níveis de desbloqueio de slots
  - Multiplicadores de XP de runas
  - Settings de maldições
  - Efeitos elementais
- [ ] **WorldConfig:**
  - Sistema de inimigos nivelados
  - Modos de progressão de mundo
  - Level caps por fase
  - Recompensas
- [ ] **EventsConfig:**
  - Multiplicadores por evento
  - Sistema anti-farm
  - Categorias de eventos
- [ ] **ClientConfig:**
  - Efeitos visuais
  - UI settings
  - Notificações
- [ ] **AdvancedConfig:**
  - Blacklists
  - Hard caps customizados
  - Netcode settings
  - Debug mode
- [ ] Sincronização server-side
- [ ] Validação de valores
- [ ] Reset para padrões

**Dependências:** Todos os RF anteriores

---

## 3. Requisitos Não-Funcionais

### 3.1 Performance
- **RNF-001:** Não causar lag perceptível (>60 FPS consistente)
- **RNF-002:** Cálculos de stats otimizados (cache quando possível)
- **RNF-003:** Sincronização de rede eficiente (<50ms latência adicional)
- **RNF-004:** Partículas otimizadas (máximo 100 partículas por fonte)

### 3.2 Compatibilidade
- **RNF-005:** Compatível com tModLoader mais recente
- **RNF-006:** Compatível com Terraria 1.4.4.x
- **RNF-007:** Não conflitar com mods populares
- **RNF-008:** Suporte a multiplayer (2-8 jogadores)

### 3.3 Usabilidade
- **RNF-009:** Interface intuitiva e fácil de usar
- **RNF-010:** Tooltips informativos e completos
- **RNF-011:** Feedback visual claro para todas as ações
- **RNF-012:** Localização em inglês e português

### 3.4 Confiabilidade
- **RNF-013:** Salvamento automático e seguro de dados
- **RNF-014:** Recuperação de erros sem perda de dados
- **RNF-015:** Sincronização robusta em multiplayer
- **RNF-016:** Sem crashes conhecidos

---

## 4. Casos de Uso

### 4.1 CU-001: Jogador Iniciante
**Ator:** Jogador novo no mod  
**Objetivo:** Entender e começar a usar o sistema

**Fluxo Principal:**
1. Jogador cria/entra em um mundo
2. Obtém primeira arma
3. Usa a arma e percebe tooltip de XP
4. Arma sobe de nível
5. Vê notificação de level up
6. Sente diferença na power do item
7. Continua usando e evoluindo

**Fluxo Alternativo:**
- Jogador abre UI com K para ver detalhes
- Consulta tooltips para entender mecânicas
- Experimenta diferentes equipamentos

---

### 4.2 CU-002: Construindo uma Build de Runas
**Ator:** Jogador experiente  
**Objetivo:** Criar build otimizada com runas

**Fluxo Principal:**
1. Evolui arma até nível 20
2. Derrota boss e obtém primeira runa
3. Aplica runa no equipamento
4. Arma ganha efeitos elementais
5. Continua evoluindo para desbloquear mais slots
6. Experimenta combinações de runas
7. Otimiza build para estilo de jogo

**Fluxo Alternativo:**
- Testa maldições para poder extremo
- Gerencia riscos das maldições
- Remove runas insatisfatórias

---

### 4.3 CU-003: Sobrevivendo em Zona Radioativa
**Ator:** Jogador em multiplayer  
**Objetivo:** Sobreviver e farmar em zona perigosa

**Fluxo Principal:**
1. Colega morre com arma amaldiçoada
2. Zona radioativa se forma
3. Jogador entra na zona
4. Enfrenta inimigos nivelados
5. Ganha XP bônus pelos inimigos
6. Monitora countdown e tier
7. Evacua antes da explosão final

**Fluxo Alternativo:**
- Fica em casa durante explosão
- Usa resistências para mitigar debuffs
- Coleta arma dropada do colega

---

## 5. Cronograma e Entregas

### Fase 1: Core Systems (Completo) ✅
- Sistema de equipamentos assinados
- Progressão de XP e níveis
- Stats escaláveis
- UI de gerenciamento
- Salvamento e sincronização

### Fase 2: Rune System (Completo) ✅
- Sistema de slots
- Runas elementais
- Runas utilitárias
- Sistema de maldições
- Remoção de runas

### Fase 3: World Systems (Completo) ✅
- Zonas radioativas
- Sistema de tiers
- Inimigos nivelados
- Countdown final
- Debuffs por proximidade

### Fase 4: Polish & Balance (Em Andamento) 🔄
- Balanceamento fino de valores
- Otimizações de performance
- Correção de bugs
- Feedback da comunidade
- Testes extensivos

### Fase 5: Expansão Futura (Planejado) 📋
- Mais tipos de runas
- Boss exclusivo do mod
- Sistema de conquistas
- Missões diárias
- Leaderboards

---

## 6. Métricas de Sucesso

### 6.1 Métricas de Engajamento
- **Taxa de Adoção:** >70% dos jogadores usando o sistema após 1 hora de jogo
- **Tempo de Sessão:** Aumento de 30% no tempo médio de jogo
- **Retenção:** 60% dos jogadores retornam após primeira sessão

### 6.2 Métricas de Satisfação
- **Avaliações Positivas:** >85% de reviews positivas no Steam Workshop
- **Bugs Reportados:** <5 bugs críticos não resolvidos
- **Feedback da Comunidade:** >4.5 estrelas em média

### 6.3 Métricas Técnicas
- **Performance:** Mantém 60 FPS em máquinas mid-range
- **Crash Rate:** <0.1% de taxa de crashes
- **Multiplayer Sync:** <100ms de latência adicional

---

## 7. Riscos e Mitigações

### 7.1 Riscos Técnicos
| Risco | Probabilidade | Impacto | Mitigação |
|-------|---------------|---------|-----------|
| Performance em zonas radioativas | Média | Alto | Otimizar partículas, limitar quantidade |
| Desincronia em multiplayer | Alta | Crítico | Servidor autoritativo, validação constante |
| Bugs de salvamento | Baixa | Crítico | Backups automáticos, validação de dados |
| Conflitos com outros mods | Média | Médio | Testes de compatibilidade, hooks defensivos |

### 7.2 Riscos de Design
| Risco | Probabilidade | Impacto | Mitigação |
|-------|---------------|---------|-----------|
| Farming excessivo | Alta | Alto | Sistema anti-farm, balanceamento |
| Maldições muito arriscadas | Média | Médio | Avisos claros, sistema de remoção |
| Curva de aprendizado íngreme | Média | Médio | Tutoriais in-game, tooltips detalhados |
| Desbalanceamento PvE | Alta | Alto | Testes extensivos, ajustes configuráveis |

---

## 8. Apêndices

### 8.1 Glossário
- **Signature Item:** Equipamento que foi assinado e pode evoluir
- **XP:** Experience Points, pontos de experiência
- **Tier:** Nível de perigo/poder de uma zona radioativa
- **Rune:** Modificador aplicável a equipamentos
- **Curse:** Runa de alto risco e alta recompensa
- **Epicentro:** Centro de uma zona radioativa

### 8.2 Referências
- Terraria Wiki: https://terraria.wiki.gg/
- tModLoader Documentation: https://github.com/tModLoader/tModLoader/wiki
- Signature Equipment Original: [mod predecessor]

---

**Aprovações:**

| Nome | Papel | Data | Assinatura |
|------|-------|------|------------|
| | Tech Lead | | |
| | Product Owner | | |
| | QA Lead | | |

---

**Histórico de Revisões:**

| Versão | Data | Autor | Alterações |
|--------|------|-------|------------|
| 1.0 | 31/12/2025 | Equipe Dev | Criação inicial do documento |
