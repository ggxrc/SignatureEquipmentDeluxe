# RFC - Request for Comments
## SignatureEquipmentDeluxe - Arquitetura Técnica

**RFC ID:** RFC-001  
**Título:** Arquitetura de Sistemas de Progressão e Mundo Dinâmico  
**Autor:** Equipe Técnica SignatureEquipmentDeluxe  
**Status:** Aceito e Implementado  
**Data Criação:** 31/12/2025  
**Última Atualização:** 31/12/2025

---

## 1. Resumo Executivo

Este RFC descreve a arquitetura técnica completa do mod SignatureEquipmentDeluxe para Terraria/tModLoader. O mod implementa um sistema de progressão de equipamentos com múltiplas camadas de complexidade, incluindo:

- Sistema de assinatura e evolução de itens
- Sistema de runas e maldições
- Zonas radioativas dinâmicas
- Inimigos nivelados
- Sistema de eventos e multiplicadores

A arquitetura segue princípios de modularidade, separação de responsabilidades, e otimização para performance em multiplayer.

---

## 2. Arquitetura Geral

### 2.1 Visão de Alto Nível

```
┌─────────────────────────────────────────────────────────────┐
│                    SignatureEquipmentDeluxe                  │
│                         (Mod Principal)                      │
└─────────────────┬───────────────────────────────────────────┘
                  │
    ┌─────────────┴─────────────┐
    │                           │
    ▼                           ▼
┌─────────────────┐    ┌──────────────────┐
│  Common Layer   │    │  Content Layer   │
│                 │    │                  │
│ • Configs       │    │ • Items          │
│ • Systems       │    │ • Projectiles    │
│ • Players       │    │ • NPCs           │
│ • GlobalItems   │    │ • Buffs          │
│ • UI            │    └──────────────────┘
│ • Visual        │
│ • Data          │
└─────────────────┘
```

### 2.2 Camadas da Aplicação

#### Common Layer
- **Responsabilidade:** Lógica core, sistemas compartilhados, dados persistentes
- **Componentes:** Configs, Systems, Players, GlobalItems, UI, Visual Effects

#### Content Layer
- **Responsabilidade:** Conteúdo jogável (itens, projéteis, NPCs)
- **Componentes:** Items (Runas, Consumíveis), Projectiles (Efeitos), NPCs (futuro)

---

## 3. Módulos Principais

### 3.1 Sistema de Configuração

#### Arquitetura de Configs

```csharp
ModConfig (tModLoader)
    ↓
    ├── GameplayConfig (ServerSide)
    │   └── Level caps, toggles de stats, tipos de damage
    │
    ├── ProgressionConfig (ServerSide)
    │   └── XP sources, curvas de level, kill streaks
    │
    ├── ScalingConfig (ServerSide)
    │   └── Stats detalhados, scaling tiers, hard caps
    │
    ├── RuneConfig (ServerSide)
    │   └── Sistema de runas, maldições, efeitos elementais
    │
    ├── WorldConfig (ServerSide)
    │   └── Inimigos nivelados, progressão de mundo
    │
    ├── EventsConfig (ServerSide)
    │   └── Multiplicadores, anti-farm, categorias
    │
    ├── ClientConfig (ClientSide)
    │   └── Efeitos visuais, UI, notificações
    │
    └── AdvancedConfig (ServerSide)
        └── Blacklists, hard caps customizados, debug
```

**Decisões de Design:**
1. **Separação por Responsabilidade:** Cada config tem escopo bem definido
2. **Server-Side vs Client-Side:** Apenas ClientConfig é client-side para performance
3. **Validação:** Ranges e DefaultValues garantem valores válidos
4. **Serialização:** TagCompound para persistência
5. **Hot Reload:** Suporte a reload sem reiniciar o jogo

---

### 3.2 Sistema de Equipamentos Assinados

#### 3.2.1 SignatureGlobalItem

**Responsabilidades:**
- Armazena dados persistentes (Level, Experience, Runas)
- Calcula stats escalados
- Aplica modificadores em hooks do tModLoader
- Gerencia cache de configurações

**Estrutura de Dados:**

```csharp
public class SignatureGlobalItem : GlobalItem
{
    // Dados persistentes
    public int Level { get; set; }
    public int Experience { get; set; }
    public List<EquippedRune> EquippedRunes { get; set; }
    
    // Métodos de cálculo
    private int GetStatCapped(ItemStatInt config, Item item)
    private float GetStatCappedFloat(ItemStatFloat config, Item item)
    
    // Hooks de modificação
    public override void ModifyWeaponDamage(...)
    public override void ModifyWeaponCrit(...)
    public override void ModifyWeaponKnockback(...)
    // ... outros hooks
    
    // Serialização
    public override bool NeedsSaving(Item item)
    public override void SaveData(Item item, TagCompound tag)
    public override void LoadData(Item item, TagCompound tag)
}
```

**Fluxo de Cálculo de Stats:**

```
Item Usado
    ↓
ModifyWeaponDamage/Crit/etc (Hook)
    ↓
GetConfig() → Cache de Config
    ↓
GetStatCapped() → Calcula stat
    ↓
ScalingCalculator.CalculateStat()
    │
    ├─ Legacy Mode → PerLevel * Level * PerLevelMult
    │
    └─ Tiered Mode → Busca tier apropriado
                     ↓
                     Calcula baseado no tier
    ↓
Aplica Hard Caps (se existir)
    ↓
Aplica Global Cap (Max)
    ↓
Retorna valor final
```

**Otimizações:**
1. **Cache de Config:** Configs são pesadas, cache evita lookups repetidos
2. **Early Return:** Checks de Level == 0 e blacklist no início
3. **Lazy Evaluation:** Stats só calculados quando necessário
4. **Struct para ScalingTier:** Menos allocations

---

#### 3.2.2 ScalingCalculator

**Padrão:** Static Helper Class  
**Responsabilidade:** Cálculos matemáticos de scaling

```csharp
public static class ScalingCalculator
{
    // Calcula stat inteiro com base em modo de scaling
    public static int CalculateStatInt(
        int level,
        ScalingMode mode,
        List<ScalingTier> tiers,
        int perLevel,
        int perLevelMult)
    {
        return mode switch
        {
            ScalingMode.Legacy => CalculateLegacyInt(...),
            ScalingMode.Tiered => CalculateTieredInt(...),
            _ => 0
        };
    }
    
    // Calcula stat float (similar)
    public static float CalculateStat(...) { }
    
    // Encontra tier apropriado para o nível
    private static ScalingTier GetTierForLevel(...) { }
}
```

**Modos de Scaling:**

1. **Legacy Mode:**
   - Fórmula: `PerLevel * Level * PerLevelMult`
   - Simples, linear, previsível
   - Usado para backward compatibility

2. **Tiered Mode:**
   - Múltiplos tiers com breakpoints
   - Cada tier tem sua própria fórmula
   - Permite crescimento não-linear
   - Exemplo:
     ```
     Tier 1 (Lv 1-50):  +1 dano/nível
     Tier 2 (Lv 51-100): +2 dano/nível
     Tier 3 (Lv 101+):   +5 dano/nível
     ```

---

#### 3.2.3 SignaturePlayer

**Responsabilidades:**
- Rastreia multiplicadores de XP
- Gerencia sistema de eventos
- Detecta e notifica mudanças de estado
- Controla idle state para auras

**Estrutura:**

```csharp
public class SignaturePlayer : ModPlayer
{
    // Estado do jogador
    public float xpMultiplier { get; set; }
    private int idleFrameCounter { get; set; }
    public bool IsIdleForAura => idleFrameCounter >= 600;
    
    // Cache de eventos
    private float cachedEventsMultiplier { get; set; }
    private List<GameEventType> previousActiveEvents { get; set; }
    
    // Lifecycle
    public override void PostUpdate()
    {
        UpdateIdleTracking();
        UpdateEventTracking();
        UpdateVisualEffects();
    }
    
    // Cálculo de multiplicadores
    private float CalculateEventsMultiplier(...)
    {
        // Combina multiplicadores de todos eventos ativos
        // Aplica penalidades de anti-farm
        // Retorna multiplicador final
    }
}
```

**Sistema de Cache de Eventos:**

```
Frame 1:
    └─ DetectEvents() → [Boss, Night]
    └─ Calculate → 1.5x * 1.1x = 1.65x
    └─ Cache resultado

Frames 2-59:
    └─ Usa cache (1.65x)

Frame 60:
    └─ Re-detecta eventos → [Boss] (Night acabou)
    └─ Notifica mudança
    └─ Recalcula → 1.5x
    └─ Atualiza cache
```

**Benefícios:**
- Evita cálculos a cada frame
- Detecta mudanças de estado
- Notificações precisas
- Performance otimizada

---

### 3.3 Sistema de Runas

#### 3.3.1 Arquitetura de Dados

```csharp
// Enum de tipos
public enum RuneType
{
    None = 0,
    // Elementais
    Fire = 1,
    Ice = 2,
    Poison = 3,
    Lightning = 4,
    // Utilitárias
    AttackSpeed = 5,
    LifeRegen = 6,
    Lifesteal = 7,
    // Maldições
    CurseBerserker = 100,
    CurseGlass = 101,
    CurseAnnihilation = 102
}

// Runa equipada
public class EquippedRune
{
    public RuneType Type { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int MaxLevel { get; set; }
    
    public bool IsCurse() => Type >= RuneType.CurseBerserker;
    
    public int GetXPForNextLevel()
    {
        // Runas precisam de 2x XP da arma
        return ProgressionSystem.GetRequiredXP(Level) * 2;
    }
    
    public bool AddExperience(int amount)
    {
        Experience += amount;
        return CheckLevelUp();
    }
}

// Definições estáticas
public static class RuneDefinitions
{
    public static float GetDamageBonus(RuneType type, int level)
    {
        return type switch
        {
            RuneType.Fire => 5f + (level * 0.5f),      // +5% base, +0.5% por nível
            RuneType.Ice => 4f + (level * 0.4f),       // +4% base, +0.4% por nível
            RuneType.Poison => 3f + (level * 0.3f),    // +3% base, +0.3% por nível
            RuneType.Lightning => 6f + (level * 0.6f), // +6% base, +0.6% por nível
            _ => 0f
        };
    }
    
    // Outros getters para diferentes stats...
}
```

#### 3.3.2 RuneSystem

**Padrão:** Static Facade  
**Responsabilidade:** Lógica central de runas

```csharp
public static class RuneSystem
{
    // Verificações
    public static bool CanHaveRunes(Item item) { }
    public static int GetMaxRuneSlots(int weaponLevel) { }
    
    // Progressão
    public static void AddXPToRunes(
        List<EquippedRune> runes, 
        int amount, 
        bool isKill, 
        int weaponMaxLevel)
    {
        var config = GetConfig();
        float multiplier = isKill 
            ? config.RuneXPPerKillMultiplier 
            : config.RuneXPPerHitMultiplier;
        
        int runeXP = (int)(amount * multiplier);
        
        foreach (var rune in runes)
        {
            rune.MaxLevel = weaponMaxLevel; // Atualiza cap
            bool leveledUp = rune.AddExperience(runeXP);
            
            if (leveledUp)
                NotifyRuneLevelUp(rune);
        }
    }
    
    // Aplicação de efeitos
    public static void ApplyRuneDamageBonus(
        List<EquippedRune> runes, 
        ref StatModifier damage)
    {
        foreach (var rune in runes)
        {
            float bonus = RuneDefinitions.GetDamageBonus(
                rune.Type, 
                rune.Level
            );
            damage += bonus / 100f; // Converte % para multiplicador
        }
    }
    
    // Efeitos on-hit
    public static void ProcessRuneOnHitEffects(
        List<EquippedRune> runes,
        NPC target,
        int damage,
        Player player)
    {
        foreach (var rune in runes)
        {
            ProcessSingleRuneOnHit(rune, target, damage, player);
        }
    }
    
    private static void ProcessSingleRuneOnHit(...)
    {
        switch (rune.Type)
        {
            case RuneType.Fire:
                ApplyFireEffect(target, rune.Level);
                break;
            case RuneType.Ice:
                ApplyIceEffect(target, rune.Level);
                break;
            // ... outros elementos
        }
    }
}
```

#### 3.3.3 RuneElementalEffects

**Responsabilidade:** Efeitos visuais e DoT de runas elementais

```csharp
public static class RuneElementalEffects
{
    public static void ApplyFireTrail(Projectile projectile, int runeLevel)
    {
        if (!ShouldSpawnTrail()) return;
        
        int dustCount = 1 + (runeLevel / 20); // Mais dust em níveis altos
        
        for (int i = 0; i < dustCount; i++)
        {
            Dust dust = Dust.NewDustDirect(
                projectile.position,
                projectile.width,
                projectile.height,
                DustID.Torch, // Fogo laranja
                0f, 0f, 100,
                default, 1.0f + (runeLevel * 0.01f) // Escala com nível
            );
            dust.noGravity = true;
            dust.velocity *= 0.3f;
        }
    }
    
    private static bool ShouldSpawnTrail()
    {
        // Throttle: apenas 1 a cada 3 frames
        return Main.GameUpdateCount % 3 == 0;
    }
    
    public static void ApplyFireDoT(NPC target, int runeLevel, Player source)
    {
        var config = GetRuneConfig();
        
        int damage = (int)(config.FireDoTDamagePerLevel * runeLevel);
        int duration = config.FireDoTDuration; // 180 frames = 3 segundos
        
        target.AddBuff(BuffID.OnFire, duration);
        
        // DoT customizado adicional
        ApplyCustomDoT(target, damage, duration, DamageType.Fire);
    }
}
```

**Otimizações de Partículas:**
1. **Throttling:** Spawn apenas a cada N frames
2. **Level-based Scaling:** Mais partículas em níveis altos, mas com cap
3. **No Gravity:** Menos cálculos de física
4. **Distance Culling:** Não spawna se muito longe do jogador

---

### 3.4 Sistema de Zonas Radioativas

#### 3.4.1 LeveledEnemySystem

**Padrão:** ModSystem + GlobalNPC  
**Responsabilidade:** Gerenciamento de zonas e inimigos nivelados

```csharp
public class LeveledEnemySystem : ModSystem
{
    // Lista de zonas ativas
    public static List<RadioactiveZone> ActiveZones { get; set; }
    
    // Constantes
    private const int ZONE_DURATION = 36000; // 10 minutos (60fps * 600s)
    private const float BASE_RADIUS = 375f;   // 150 tiles * 2.5
    
    public override void PostUpdateEverything()
    {
        UpdateAllZones();
        CleanupExpiredZones();
    }
    
    private void UpdateAllZones()
    {
        foreach (var zone in ActiveZones)
        {
            zone.Update();
            
            if (zone.IsFinalCountdown)
            {
                zone.UpdateFinalCountdown();
            }
        }
    }
    
    public static void CreateZone(
        Vector2 position, 
        int weaponLevel, 
        List<EquippedRune> runes)
    {
        var zone = new RadioactiveZone
        {
            Position = position,
            InitialRadius = BASE_RADIUS,
            Radius = BASE_RADIUS,
            TimeLeft = ZONE_DURATION,
            WeaponLevel = weaponLevel,
            InheritedRunes = new List<EquippedRune>(runes)
        };
        
        ActiveZones.Add(zone);
        
        // Sincroniza em multiplayer
        if (Main.netMode == NetmodeID.Server)
        {
            NetMessage.SendData(...); // Sync para todos os clientes
        }
    }
}

public class RadioactiveZone
{
    public Vector2 Position { get; set; }
    public float Radius { get; set; }
    public float InitialRadius { get; set; }
    public int TimeLeft { get; set; }
    public int DangerLevel { get; set; } // 1-5
    public int WeaponLevel { get; set; }
    public List<EquippedRune> InheritedRunes { get; set; }
    
    private bool isFinalCountdown = false;
    public bool IsFinalCountdown => isFinalCountdown;
    
    public void Update()
    {
        TimeLeft--;
        
        UpdateDangerLevel();
        UpdateRadius();
        SpawnParticles();
        
        if (TimeLeft <= 600 && !isFinalCountdown) // 10 segundos
        {
            StartFinalCountdown();
        }
        
        if (TimeLeft <= 0)
        {
            TriggerFinalExplosion();
        }
    }
    
    private void UpdateDangerLevel()
    {
        int elapsed = 36000 - TimeLeft; // Tempo decorrido
        int newLevel = 1 + (elapsed / 7200); // 7200 frames = 2 minutos
        
        if (newLevel != DangerLevel && newLevel <= 5)
        {
            DangerLevel = newLevel;
            OnTierChanged();
        }
    }
    
    private void UpdateRadius()
    {
        // +10% por tier
        Radius = InitialRadius * (1f + (DangerLevel - 1) * 0.1f);
    }
    
    private void OnTierChanged()
    {
        // Notificação visual
        CreateTierChangeEffect();
        
        // Som de aviso
        SoundEngine.PlaySound(SoundID.Roar with { 
            Volume = 0.7f,
            Pitch = 0.3f 
        });
        
        // Mensagem para jogadores na zona
        foreach (var player in Main.player)
        {
            if (IsPlayerInZone(player))
            {
                player.NewText(
                    $"Danger Level: {DangerLevel}/5", 
                    GetTierColor()
                );
            }
        }
    }
    
    public void TriggerFinalExplosion()
    {
        // Efeito visual massivo
        CreateExplosionVisuals();
        
        // Dano a todos fora de casas
        foreach (var player in Main.player)
        {
            if (!IsPlayerInSafeHouse(player))
            {
                player.Hurt(PlayerDeathReason.ByCustomReason(
                    $"{player.name} was annihilated by radioactive explosion"),
                    9999, // Dano letal
                    0
                );
            }
        }
        
        // Remove a zona
        LeveledEnemySystem.ActiveZones.Remove(this);
    }
    
    private bool IsPlayerInSafeHouse(Player player)
    {
        // Verifica se há NPCs town próximos (indicador de casa válida)
        int searchRadius = 500; // pixels
        
        foreach (var npc in Main.npc)
        {
            if (!npc.active || !npc.townNPC) continue;
            
            float distance = Vector2.Distance(player.Center, npc.Center);
            if (distance < searchRadius)
            {
                return true; // Está perto de NPC town, logo em casa
            }
        }
        
        return false;
    }
}
```

#### 3.4.2 LeveledEnemyGlobalNPC

**Responsabilidade:** Aplica efeitos de zona em NPCs

```csharp
public class LeveledEnemyGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    
    // Dados do NPC
    public bool IsLeveled { get; set; }
    public int Level { get; set; }
    public RadioactiveZone SourceZone { get; set; }
    
    public override void OnSpawn(NPC npc, IEntitySource source)
    {
        // Verifica se spawnou em zona radioativa
        var zone = LeveledEnemySystem.GetZoneAtPosition(npc.Center);
        
        if (zone != null)
        {
            float chance = GetLeveledChance();
            
            if (Main.rand.NextFloat() < chance)
            {
                ApplyLeveledStats(npc, zone);
            }
        }
    }
    
    private void ApplyLeveledStats(NPC npc, RadioactiveZone zone)
    {
        IsLeveled = true;
        Level = zone.WeaponLevel + Main.rand.Next(-2, 3); // Variance
        SourceZone = zone;
        
        // +35% vida
        npc.lifeMax = (int)(npc.lifeMax * 1.35f);
        npc.life = npc.lifeMax;
        
        // Dano escalado
        float damageMultiplier = 1f + (Level * 0.02f); // +2% por nível
        npc.damage = (int)(npc.damage * damageMultiplier);
        
        // Defesa escalada
        npc.defense += Level / 5; // +1 defesa a cada 5 níveis
        
        // Confina à zona
        npc.noTileCollide = false; // Força a respeitar tiles
    }
    
    public override void AI(NPC npc)
    {
        if (!IsLeveled || SourceZone == null) return;
        
        // Confina NPC à zona
        float distance = Vector2.Distance(npc.Center, SourceZone.Position);
        
        if (distance > SourceZone.Radius)
        {
            // Empurra de volta para o centro
            Vector2 direction = Vector2.Normalize(
                SourceZone.Position - npc.Center
            );
            npc.velocity += direction * 0.5f;
        }
    }
    
    public override void ModifyHitPlayer(
        NPC npc, 
        Player target, 
        ref Player.HurtModifiers modifiers)
    {
        if (!IsLeveled) return;
        
        // Penetração de defesa baseada em diferença de nível
        int playerLevel = GetPlayerAverageLevel(target);
        int levelDiff = Level - playerLevel;
        
        if (levelDiff > 0)
        {
            // Ignora 5% de defesa por nível de diferença
            float penetration = levelDiff * 0.05f;
            penetration = Math.Min(penetration, 0.75f); // Cap de 75%
            
            modifiers.DefenseEffectiveness *= (1f - penetration);
        }
    }
    
    public override void OnKill(NPC npc)
    {
        if (!IsLeveled) return;
        
        // XP bônus
        float xpMultiplier = 1f + (Level * 0.05f); // +5% por nível
        
        // Drop de runas (chance baixa)
        if (Main.rand.NextFloat() < 0.05f) // 5%
        {
            DropRandomRune(npc);
        }
    }
    
    // Renderização de indicador de nível
    public override void DrawEffects(NPC npc, ref Color drawColor)
    {
        if (!IsLeveled) return;
        
        // Aura baseada em tier
        Color auraColor = GetTierColor(SourceZone.DangerLevel);
        Lighting.AddLight(npc.Center, auraColor.ToVector3() * 0.5f);
    }
    
    public override bool PreDraw(
        NPC npc, 
        SpriteBatch spriteBatch, 
        Vector2 screenPos, 
        Color drawColor)
    {
        if (!IsLeveled) return true;
        
        // Desenha texto de nível acima do NPC
        DrawLevelIndicator(npc, spriteBatch, screenPos);
        
        return true;
    }
}
```

**Sistema de Confinamento:**

```
NPC Spawna
    ↓
Está em zona? → Sim
    ↓
Roll de chance (15%)
    ↓
Sucesso → ApplyLeveledStats()
    ↓
A cada frame (AI):
    ↓
Calcula distância do centro
    ↓
Distância > Raio? → Sim
    ↓
Aplica força em direção ao centro
    ↓
NPC não consegue sair da zona
```

---

### 3.5 Sistema de Eventos

#### 3.5.1 EventDetector

**Padrão:** Static Utility  
**Responsabilidade:** Detecta eventos ativos do jogo

```csharp
public static class EventDetector
{
    public static List<GameEventType> GetActiveEvents()
    {
        var events = new List<GameEventType>();
        
        // Bosses
        DetectBosses(events);
        
        // Invasões
        if (Main.invasionType > 0)
            events.Add(GetInvasionType(Main.invasionType));
        
        // Luas
        if (Main.bloodMoon)
            events.Add(GameEventType.BloodMoon);
        
        // Tempo
        if (!Main.dayTime)
            events.Add(GameEventType.Night);
        else
            events.Add(GameEventType.Day);
        
        // Clima
        if (Main.raining)
            events.Add(GameEventType.Rain);
        if (Sandstorm.Happening)
            events.Add(GameEventType.Sandstorm);
        
        // Especiais
        if (BirthdayParty.PartyIsUp)
            events.Add(GameEventType.PartyEvent);
        if (LanternNight.LanternsUp)
            events.Add(GameEventType.LanternNight);
        
        return events;
    }
    
    private static void DetectBosses(List<GameEventType> events)
    {
        foreach (var npc in Main.npc)
        {
            if (!npc.active || !npc.boss) continue;
            
            GameEventType? bossType = npc.type switch
            {
                NPCID.KingSlime => GameEventType.KingSlime,
                NPCID.EyeofCthulhu => GameEventType.EyeOfCthulhu,
                NPCID.EaterofWorldsHead => GameEventType.EaterOfWorlds,
                NPCID.BrainofCthulhu => GameEventType.BrainOfCthulhu,
                // ... todos os bosses
                _ => null
            };
            
            if (bossType.HasValue && !events.Contains(bossType.Value))
            {
                events.Add(bossType.Value);
            }
        }
    }
}
```

#### 3.5.2 EventTracker

**Padrão:** Singleton  
**Responsabilidade:** Rastreia repetições de eventos para anti-farm

```csharp
public class EventTracker
{
    private static EventTracker _instance;
    public static EventTracker Instance => _instance ??= new EventTracker();
    
    // Contadores de repetição
    private Dictionary<GameEventType, int> repetitionCounts;
    
    // Timestamps de último reset
    private Dictionary<GameEventType, DateTime> lastResets;
    
    // Configuração de decay
    private TimeSpan decayPeriod = TimeSpan.FromHours(1);
    
    public void Update(List<GameEventType> activeEvents)
    {
        // Incrementa contadores de eventos ativos
        foreach (var eventType in activeEvents)
        {
            if (!repetitionCounts.ContainsKey(eventType))
            {
                repetitionCounts[eventType] = 0;
            }
            
            // Apenas incrementa na primeira detecção
            // (evita incrementar a cada frame)
        }
        
        // Processa decays
        ProcessDecay();
    }
    
    public void OnEventCompleted(GameEventType eventType)
    {
        // Incrementa contador
        if (!repetitionCounts.ContainsKey(eventType))
        {
            repetitionCounts[eventType] = 0;
        }
        
        repetitionCounts[eventType]++;
        lastResets[eventType] = DateTime.Now;
        
        // Notifica usuário se penalidade significativa
        float penalty = GetPenaltyMultiplier(eventType);
        if (penalty < 0.75f) // < 75%
        {
            NotifyPenalty(eventType, penalty);
        }
    }
    
    public float GetPenaltyMultiplier(GameEventType eventType)
    {
        if (!repetitionCounts.ContainsKey(eventType))
            return 1f;
        
        int count = repetitionCounts[eventType];
        
        // -10% por repetição, mínimo 50%
        float penalty = 1f - (count * 0.1f);
        return Math.Max(penalty, 0.5f);
    }
    
    private void ProcessDecay()
    {
        var now = DateTime.Now;
        var toRemove = new List<GameEventType>();
        
        foreach (var kvp in lastResets)
        {
            if (now - kvp.Value > decayPeriod)
            {
                toRemove.Add(kvp.Key);
            }
        }
        
        foreach (var eventType in toRemove)
        {
            repetitionCounts.Remove(eventType);
            lastResets.Remove(eventType);
        }
    }
    
    // Serialização
    public TagCompound Save()
    {
        return new TagCompound
        {
            ["counts"] = repetitionCounts.Select(kvp => 
                new TagCompound
                {
                    ["event"] = (int)kvp.Key,
                    ["count"] = kvp.Value
                }).ToList(),
            ["resets"] = lastResets.Select(kvp =>
                new TagCompound
                {
                    ["event"] = (int)kvp.Key,
                    ["time"] = kvp.Value.Ticks
                }).ToList()
        };
    }
    
    public void Load(TagCompound tag) { /* ... */ }
}
```

**Fluxo de Penalidade:**

```
Boss Ativo
    ↓
EventDetector detecta
    ↓
SignaturePlayer recebe notificação
    ↓
Boss Derrotado
    ↓
EventTracker.OnEventCompleted()
    ↓
Incrementa contador (Boss X: 3 vezes)
    ↓
Próxima vez que Boss X aparecer:
    ↓
GetPenaltyMultiplier() → 0.7 (70%)
    ↓
Multiplicador base 1.5x * 0.7 = 1.05x efetivo
    ↓
Jogador recebe menos XP bônus
```

---

### 3.6 Sistema de UI

#### 3.6.1 SignatureManagementUI

**Padrão:** UIState (tModLoader)  
**Responsabilidade:** Interface de gerenciamento de equipamentos

```csharp
public class SignatureManagementUI : UIState
{
    // Componentes da UI
    private UIPanel mainPanel;
    private UIList itemList;
    private UIScrollbar scrollbar;
    private UIText titleText;
    private UIButton closeButton;
    
    // Estado
    private List<ItemDisplay> displayedItems;
    private Item selectedItem;
    
    public override void OnInitialize()
    {
        CreateMainPanel();
        CreateTitle();
        CreateItemList();
        CreateScrollbar();
        CreateCloseButton();
    }
    
    private void CreateItemList()
    {
        itemList = new UIList();
        itemList.SetScrollbar(scrollbar);
        
        // Popula com itens do jogador
        PopulateItemList();
    }
    
    private void PopulateItemList()
    {
        displayedItems.Clear();
        itemList.Clear();
        
        var player = Main.LocalPlayer;
        var sigPlayer = player.GetModPlayer<SignaturePlayer>();
        
        // Itera sobre todos os itens do inventário
        for (int i = 0; i < player.inventory.Length; i++)
        {
            var item = player.inventory[i];
            if (item.IsAir) continue;
            
            var sigItem = item.GetGlobalItem<SignatureGlobalItem>();
            if (sigItem.Level <= 0) continue; // Não assinado
            
            var display = new ItemDisplay(item, sigItem);
            displayedItems.Add(display);
            itemList.Add(display);
        }
        
        // Ordena por nível (descendente)
        displayedItems.Sort((a, b) => 
            b.SignatureItem.Level.CompareTo(a.SignatureItem.Level)
        );
    }
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        // Recarrega lista se mudou
        if (ShouldRefreshList())
        {
            PopulateItemList();
        }
    }
}

public class ItemDisplay : UIElement
{
    private Item item;
    private SignatureGlobalItem sigItem;
    
    // Componentes visuais
    private UIImage itemIcon;
    private UIText levelText;
    private UIText xpText;
    private UIProgressBar xpBar;
    private UIRuneDisplay runeDisplay;
    
    public ItemDisplay(Item item, SignatureGlobalItem sigItem)
    {
        this.item = item;
        this.sigItem = sigItem;
        
        CreateComponents();
    }
    
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        // Desenha fundo
        DrawBackground(spriteBatch);
        
        // Desenha ícone do item
        DrawItemIcon(spriteBatch);
        
        // Desenha textos
        DrawTexts(spriteBatch);
        
        // Desenha barra de XP
        DrawXPBar(spriteBatch);
        
        // Desenha runas
        DrawRunes(spriteBatch);
    }
    
    public override void Click(UIMouseEvent evt)
    {
        // Expande detalhes ao clicar
        ExpandDetails();
    }
}
```

**Layout da UI:**

```
┌────────────────────────────────────────────────┐
│  Signature Equipment Management           [X]  │
├────────────────────────────────────────────────┤
│  ┌──────────────────────────────────────────┐ │
│  │ [Item Icon] Terra Blade        Lv. 87   │ │
│  │             ████████████░░░░   15230 XP │ │
│  │             🔥 Fire Lv.20  ⚡ Lightning  │ │
│  ├──────────────────────────────────────────┤ │
│  │ [Item Icon] Meowmere          Lv. 100   │ │
│  │             █████████████░   98750 XP   │ │
│  │             ❄️ Ice Lv.35  💀 Berserker  │ │
│  ├──────────────────────────────────────────┤ │
│  │ ...                                      │ │
│  └──────────────────────────────────────────┘ │
│                                           [▲]  │
│                                           [▼]  │
└────────────────────────────────────────────────┘
```

---

## 4. Sincronização Multiplayer

### 4.1 Arquitetura de Rede

**Modelo:** Server Autoritativo  
**Protocolo:** tModLoader's ModPacket

```csharp
public enum MessageType : byte
{
    ProjectileSizeSync,      // 0
    ItemLevelSync,           // 1
    ItemExperienceSync,      // 2
    SignaturePlayerSync,     // 3
    SignatureItemUpdate,     // 4
    SignaturePrestige,       // 5
    RadioactiveZoneCreate,   // 6
    RadioactiveZoneUpdate,   // 7
    RadioactiveZoneDestroy,  // 8
    LeveledEnemySync         // 9
}
```

### 4.2 Fluxos de Sincronização

#### Item Ganha XP

```
CLIENT                      SERVER                    OTHER CLIENTS
  │                           │                            │
  ├─ Hit Enemy ──────────────>│                            │
  │                           ├─ Valida Hit                │
  │                           ├─ Calcula XP                │
  │                           ├─ Atualiza Item             │
  │                           │                            │
  │<── ItemExperienceSync ────┤                            │
  │                           ├── ItemExperienceSync ─────>│
  │                           │                            │
  ├─ Atualiza UI Local        ├─ (Autoritativo)           ├─ Atualiza Cache
  │                           │                            │
```

#### Criação de Zona Radioativa

```
CLIENT (Player Dies)        SERVER                    ALL CLIENTS
  │                           │                            │
  ├─ Death Event ────────────>│                            │
  │                           ├─ Check Curse               │
  │                           ├─ Create Zone               │
  │                           ├─ Add to List               │
  │                           │                            │
  │<── ZoneCreate Packet ─────┤                            │
  │                           ├── ZoneCreate Packet ──────>│
  │                           │                            │
  ├─ Spawn Visual             ├─ (Autoritativo)           ├─ Spawn Visual
  ├─ Play Sound               │                            ├─ Play Sound
  │                           │                            │
```

### 4.3 Sincronização de Estado

**Quando Sincronizar:**
1. **Item Equipado/Desequipado:** Sync completo
2. **Level Up:** Sync imediato
3. **Runa Adicionada/Removida:** Sync imediato
4. **Zona Criada/Destruída:** Broadcast para todos
5. **Enemy Spawna Nivelado:** Sync apenas para quem está perto

**Otimizações:**
- **Delta Updates:** Apenas envia o que mudou
- **Batching:** Agrupa múltiplas atualizações
- **Priority Queue:** Sync críticos têm prioridade
- **Distance Culling:** Não sync eventos muito distantes

---

## 5. Performance e Otimização

### 5.1 Hotspots Identificados

1. **Cálculo de Stats a Cada Frame**
   - **Problema:** ModifyWeaponDamage chamado frequentemente
   - **Solução:** Cache de valores calculados
   
2. **Spawn de Partículas**
   - **Problema:** Muitas partículas = FPS drop
   - **Solução:** Throttling (spawn a cada 3 frames)
   
3. **Verificação de Zonas**
   - **Problema:** Distance checks custosos
   - **Solução:** Spatial hashing, cache de zonas próximas
   
4. **Sincronização de Rede**
   - **Problema:** Packets demais = lag
   - **Solução:** Delta updates, batching

### 5.2 Estratégias de Otimização

#### Cache de Stats

```csharp
public class SignatureGlobalItem : GlobalItem
{
    // Cache
    private int cachedDamage = -1;
    private int cachedLevel = -1;
    private bool cacheValid = false;
    
    public override void ModifyWeaponDamage(...)
    {
        // Invalida cache se nível mudou
        if (Level != cachedLevel)
        {
            cacheValid = false;
        }
        
        // Usa cache se válido
        if (cacheValid)
        {
            damage += cachedDamage;
            return;
        }
        
        // Calcula e atualiza cache
        int calculatedDamage = CalculateDamageBonus(item);
        cachedDamage = calculatedDamage;
        cachedLevel = Level;
        cacheValid = true;
        
        damage += calculatedDamage;
    }
}
```

#### Spatial Hashing para Zonas

```csharp
public class ZoneSpatialHash
{
    private Dictionary<Point, List<RadioactiveZone>> grid;
    private const int CELL_SIZE = 1000; // pixels
    
    public void AddZone(RadioactiveZone zone)
    {
        Point cell = GetCell(zone.Position);
        
        if (!grid.ContainsKey(cell))
            grid[cell] = new List<RadioactiveZone>();
        
        grid[cell].Add(zone);
    }
    
    public List<RadioactiveZone> GetNearbyZones(Vector2 position, float radius)
    {
        var result = new List<RadioactiveZone>();
        Point center = GetCell(position);
        
        // Verifica células vizinhas
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Point checkCell = new Point(center.X + x, center.Y + y);
                
                if (grid.TryGetValue(checkCell, out var zones))
                {
                    result.AddRange(zones.Where(z => 
                        Vector2.Distance(z.Position, position) <= radius
                    ));
                }
            }
        }
        
        return result;
    }
    
    private Point GetCell(Vector2 position)
    {
        return new Point(
            (int)(position.X / CELL_SIZE),
            (int)(position.Y / CELL_SIZE)
        );
    }
}
```

#### Object Pooling para Partículas

```csharp
public class ParticlePool
{
    private Queue<Particle> availableParticles;
    private List<Particle> activeParticles;
    
    public Particle GetParticle()
    {
        if (availableParticles.Count > 0)
        {
            var particle = availableParticles.Dequeue();
            activeParticles.Add(particle);
            return particle;
        }
        
        // Cria nova se pool vazio
        var newParticle = new Particle();
        activeParticles.Add(newParticle);
        return newParticle;
    }
    
    public void ReturnParticle(Particle particle)
    {
        activeParticles.Remove(particle);
        particle.Reset();
        availableParticles.Enqueue(particle);
    }
    
    public void UpdateAll()
    {
        for (int i = activeParticles.Count - 1; i >= 0; i--)
        {
            var particle = activeParticles[i];
            particle.Update();
            
            if (particle.IsDead)
            {
                ReturnParticle(particle);
            }
        }
    }
}
```

---

## 6. Padrões de Código

### 6.1 Convenções de Nomenclatura

- **Classes:** PascalCase (`SignatureGlobalItem`)
- **Métodos:** PascalCase (`CalculateDamageBonus`)
- **Propriedades:** PascalCase (`Level`, `Experience`)
- **Campos Privados:** camelCase (`cachedDamage`)
- **Constantes:** UPPER_SNAKE_CASE (`BASE_RADIUS`)
- **Enums:** PascalCase com valores PascalCase

### 6.2 Organização de Arquivos

```
Sistema/
├── [Sistema]System.cs          # Lógica principal
├── [Sistema]Config.cs          # Configuração
├── [Sistema]Data.cs            # Estruturas de dados
├── [Sistema]Visuals.cs         # Efeitos visuais
└── [Sistema]Network.cs         # Sincronização (se aplicável)
```

### 6.3 Documentação de Código

**Sempre use XML Documentation:**

```csharp
/// <summary>
/// Calcula o dano bônus baseado no nível do item
/// </summary>
/// <param name="item">Item a ser calculado</param>
/// <param name="level">Nível atual do item</param>
/// <returns>Dano adicional em pontos</returns>
/// <remarks>
/// O cálculo usa a fórmula: Level * PerLevel * PerLevelMult
/// Caps são aplicados após o cálculo base
/// </remarks>
public int CalculateDamageBonus(Item item, int level)
{
    // Implementação...
}
```

### 6.4 Error Handling

```csharp
// BOM: Defensive checks
public void AddExperience(Item item, int amount)
{
    if (item == null || item.IsAir)
    {
        Logger.Warn("Tentativa de adicionar XP a item inválido");
        return;
    }
    
    if (amount <= 0)
    {
        Logger.Warn($"Quantidade de XP inválida: {amount}");
        return;
    }
    
    var sigItem = item.GetGlobalItem<SignatureGlobalItem>();
    sigItem.Experience += amount;
}

// RUIM: Assume inputs válidos
public void AddExperience(Item item, int amount)
{
    item.GetGlobalItem<SignatureGlobalItem>().Experience += amount;
    // Pode crashar se item for null!
}
```

---

## 7. Testes e Validação

### 7.1 Cenários de Teste

#### Teste de Progressão
```
1. Criar novo personagem
2. Obter arma inicial
3. Matar 100 slimes
4. Verificar:
   - XP ganho correto
   - Level ups aconteceram
   - Dano aumentou conforme esperado
   - UI mostra dados corretos
```

#### Teste de Runas
```
1. Evoluir arma até nível 20
2. Obter runa elemental
3. Aplicar runa
4. Verificar:
   - Runa aparece na UI
   - Efeitos visuais funcionam
   - DoT é aplicado em inimigos
   - XP da runa aumenta com uso
```

#### Teste de Zona Radioativa
```
1. Configurar mundo multiplayer
2. Jogador 1 obtém arma com maldição
3. Jogador 1 morre intencionalmente
4. Verificar:
   - Zona é criada
   - Ambos jogadores veem a zona
   - Inimigos nivelados spawnam
   - Countdown final funciona
   - Explosão mata jogadores fora
```

### 7.2 Benchmarks de Performance

**Target Specs:**
- CPU: Intel i5-8400 / Ryzen 5 2600
- RAM: 8GB
- GPU: GTX 1060 / RX 580

**Métricas:**
- FPS mínimo: 60 (em combate normal)
- FPS em zona radioativa: >45
- Latência multiplayer: <50ms adicional
- Uso de RAM: <500MB

---

## 8. Segurança e Anti-Cheat

### 8.1 Validações Server-Side

```csharp
// Servidor valida todos os XP gains
public void OnPlayerHitEnemy(Player player, NPC target, int damage)
{
    // Valida dados
    if (player == null || target == null)
        return;
    
    // Calcula XP esperado
    int expectedXP = CalculateExpectedXP(damage, target);
    
    // Verifica contra máximo razoável
    if (expectedXP > MAX_XP_PER_HIT)
    {
        Logger.Warn($"XP suspeito detectado: {expectedXP}");
        expectedXP = MAX_XP_PER_HIT;
    }
    
    // Aplica XP
    ApplyExperience(player, expectedXP);
    
    // Syncroniza
    SendXPUpdate(player, expectedXP);
}
```

### 8.2 Rate Limiting

```csharp
// Limita ganho de XP por segundo
public class XPRateLimiter
{
    private Dictionary<byte, Queue<DateTime>> playerXPTimestamps;
    private const int MAX_XP_GAINS_PER_SECOND = 10;
    
    public bool AllowXPGain(byte playerID)
    {
        if (!playerXPTimestamps.ContainsKey(playerID))
        {
            playerXPTimestamps[playerID] = new Queue<DateTime>();
        }
        
        var timestamps = playerXPTimestamps[playerID];
        var now = DateTime.Now;
        
        // Remove timestamps antigos (>1 segundo)
        while (timestamps.Count > 0 && 
               (now - timestamps.Peek()).TotalSeconds > 1)
        {
            timestamps.Dequeue();
        }
        
        // Verifica limite
        if (timestamps.Count >= MAX_XP_GAINS_PER_SECOND)
        {
            return false; // Bloqueado
        }
        
        timestamps.Enqueue(now);
        return true;
    }
}
```

---

## 9. Roadmap Técnico

### 9.1 Melhorias Planejadas

#### Curto Prazo (1-2 meses)
- [ ] Sistema de achievements
- [ ] Mais tipos de runas
- [ ] Otimização de partículas
- [ ] Melhoria de UI/UX

#### Médio Prazo (3-6 meses)
- [ ] Boss exclusivo do mod
- [ ] Sistema de missões diárias
- [ ] Leaderboards (opcional)
- [ ] Sistema de sinergia entre runas

#### Longo Prazo (6+ meses)
- [ ] Integração com outros mods populares
- [ ] Sistema de clãs/guildas
- [ ] Raids customizados
- [ ] Editor de builds compartilháveis

---

## 10. Referências

### 10.1 Documentação Externa
- [tModLoader Wiki](https://github.com/tModLoader/tModLoader/wiki)
- [Terraria Modding Discord](https://discord.gg/tmodloader)
- [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)

### 10.2 Mods de Referência
- Calamity Mod (sistema de progressão)
- Thorium Mod (sistema de classes)
- Fargo's Soul Mod (enchantments)

---

## 11. Aprovações

| Nome | Papel | Data | Status |
|------|-------|------|--------|
| | Tech Lead | 31/12/2025 | ✅ Aprovado |
| | Senior Dev | 31/12/2025 | ✅ Aprovado |

---

**Histórico de Revisões:**

| Versão | Data | Autor | Alterações |
|--------|------|-------|------------|
| 1.0 | 31/12/2025 | Equipe Dev | Criação inicial do RFC |

---

**Fim do RFC-001**
