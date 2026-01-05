# Copilot Instructions for SignatureEquipmentDeluxe

## Overview
SignatureEquipmentDeluxe is a Terraria mod built using the tModLoader framework. It extends the functionality of the original SignatureEquipment mod by introducing advanced systems like radioactive zones, leveled enemies, and cursed weapons. The mod is modular, with clear separation of concerns across its components.

## 📚 CRITICAL: Documentation-First Approach

### **ALWAYS Read Documentation BEFORE Making Changes**

Before modifying, adding, or removing ANY code, you **MUST**:

1. **Read the PRD (Product Requirements Document)**
   - Location: `docs/PRD.md`
   - Contains: Functional requirements, use cases, acceptance criteria
   - **Purpose**: Understand WHAT the system should do

2. **Read the RFC (Request for Comments)**
   - Location: `docs/RFC.md`
   - Contains: Technical architecture, design decisions, code patterns
   - **Purpose**: Understand HOW the system is built

3. **Read the Technical Documentation**
   - Location: `docs/README.md`
   - Contains: Implementation guide, configuration, troubleshooting
   - **Purpose**: Understand current implementation details

### **ALWAYS Update Documentation AFTER Making Changes**

After ANY code modification, you **MUST** update the appropriate documentation:

#### When Adding a New Feature:
1. ✅ **Update `docs/PRD.md`:**
   - Add new functional requirement (RF-XXX)
   - Document acceptance criteria
   - Add use cases if applicable
   
2. ✅ **Update `docs/RFC.md`:**
   - Document architecture decisions
   - Add class diagrams if needed
   - Explain design patterns used
   - Update code examples
   
3. ✅ **Update `docs/README.md`:**
   - Add to "Sistemas Principais" section
   - Update configuration section if new config added
   - Add troubleshooting tips
   
4. ✅ **Update `README.md` (root):**
   - Add to feature list if user-visible
   - Update usage guide if needed

#### When Modifying Existing Code:
1. ✅ **Update relevant sections in ALL affected documents**
2. ✅ **Mark changes with version/date**
3. ✅ **Update code examples if behavior changed**

#### When Removing Code:
1. ✅ **Mark as deprecated in PRD (don't delete history)**
2. ✅ **Remove from RFC architecture diagrams**
3. ✅ **Remove from technical docs**
4. ✅ **Update README if user-facing**

### Documentation CRUD Workflow

```
┌─────────────────────────────────────────┐
│  1. READ DOCUMENTATION                  │
│     - PRD.md (requirements)             │
│     - RFC.md (architecture)             │
│     - docs/README.md (implementation)   │
└──────────────┬──────────────────────────┘
               ↓
┌─────────────────────────────────────────┐
│  2. UNDERSTAND CONTEXT                  │
│     - What exists?                      │
│     - Why was it designed this way?     │
│     - What are the constraints?         │
└──────────────┬──────────────────────────┘
               ↓
┌─────────────────────────────────────────┐
│  3. PLAN CHANGES                        │
│     - How does it fit the architecture? │
│     - What needs to be updated?         │
│     - What are the impacts?             │
└──────────────┬──────────────────────────┘
               ↓
┌─────────────────────────────────────────┐
│  4. IMPLEMENT CODE                      │
│     - Follow RFC patterns               │
│     - Match PRD requirements            │
│     - Add XML comments                  │
└──────────────┬──────────────────────────┘
               ↓
┌─────────────────────────────────────────┐
│  5. UPDATE DOCUMENTATION                │
│     - PRD: Add/update RF-XXX            │
│     - RFC: Update architecture          │
│     - docs/README: Update implementation│
│     - README: Update user-facing info   │
└──────────────┬──────────────────────────┘
               ↓
┌─────────────────────────────────────────┐
│  6. COMMIT TOGETHER                     │
│     - Code + Documentation in same PR   │
│     - Never commit code without docs!   │
└─────────────────────────────────────────┘
```

### Documentation Quick Reference

| When you... | Update these docs... |
|-------------|---------------------|
| Add new feature | PRD (new RF-XXX), RFC (architecture), docs/README (usage), README (features) |
| Change existing feature | All docs that mention it |
| Add new system | PRD (requirements), RFC (full architecture section), docs/README (section) |
| Add configuration | RFC (config section), docs/README (configuration), README (configuration) |
| Fix bug | docs/README (troubleshooting if common), PRD (mark as resolved) |
| Refactor code | RFC (update architecture/patterns), keep PRD unchanged |
| Change API | RFC (update API section), docs/README (update examples) |
| Remove feature | Mark deprecated in PRD, remove from RFC/docs/README |
| Add/modify text | **ALWAYS update BOTH localization files (en-US AND pt-BR)** |

---

## Key Components

- **LeveledEnemySystem.cs**: Manages radioactive zones and leveled enemies
- **RadioactiveZoneVisuals.cs**: Handles visual effects for radioactive zones
- **CurseDeathSystem.cs**: Manages cursed weapon drops and zone upgrades
- **RadioactiveZoneDebuffs.cs**: Applies punitive effects near the center of radioactive zones
- **ProjectileTrailEffect.cs**: Adds visual effects to projectiles
- **SignatureGlobalItem.cs**: Core item progression system
- **RuneSystem.cs**: Rune and curse management
- **EventDetector.cs**: Game event detection and XP multipliers

---

## Developer Workflows

### **CRITICAL: Always Build After Changes**
**After ANY code modification, you MUST:**
1. Navigate to the mod's root directory
2. Run the build command:
   ```powershell
   dotnet build
   ```
3. **Fix ALL compilation errors** before proceeding
4. **Review and question ALL warnings** - ask the user if they should be fixed
5. Only proceed to testing after successful compilation with no errors

### Building the Mod
1. Navigate to the mod's root directory
2. Run the following command to build the mod:
   ```powershell
   dotnet build
   ```
3. Address any errors or warnings that appear during compilation

### Testing the Mod
1. Launch Terraria with tModLoader
2. Enable the SignatureEquipmentDeluxe mod in the Mods menu
3. Test the features in-game, focusing on:
   - Radioactive zone effects
   - Danger level updates and visual/audio feedback
   - Leveled enemies and cursed weapon mechanics
   - Item progression and XP gain
   - Rune system and effects
   - Event multipliers and anti-farm system

### Debugging
- Use `launchSettings.json` in the `Properties` folder to configure debugging settings
- Leverage tModLoader's built-in debugging tools for real-time testing
- Enable DebugMode in AdvancedConfig for detailed logs
- Check logs in `Documents/My Games/Terraria/tModLoader/Logs/`

---

## Project-Specific Conventions

### Code Style
- **Classes**: PascalCase (`SignatureGlobalItem`)
- **Methods**: PascalCase (`CalculateDamageBonus`)
- **Properties**: PascalCase (`Level`, `Experience`)
- **Private fields**: camelCase (`cachedDamage`)
- **Constants**: UPPER_SNAKE_CASE (`BASE_RADIUS`)

### Documentation
- **Always** add XML documentation comments to public methods/classes
- **Always** add inline comments for complex logic
- **Always** update docs when changing behavior

### Localization
- All text is localized using `.hjson` files in the `Localization` folder
- Ensure updates to `en-US` and `pt-BR` files are consistent
- Never hardcode text strings in code

### **CRITICAL: Localization Rules**
**SEMPRE que modificar código que contenha texto visível ao usuário, você DEVE:**

1. ✅ **Atualizar AMBOS os arquivos de localização:**
   - `Localization/en-US_Mods.SignatureEquipmentDeluxe.hjson` (Inglês)
   - `Localization/pt-BR_Mods.SignatureEquipmentDeluxe.hjson` (Português)

2. ✅ **Tipos de texto que SEMPRE requerem localização:**
   - DisplayName de itens, projéteis, NPCs
   - Tooltips de itens e configurações
   - Labels de configurações (ModConfig)
   - Mensagens de notificação ao jogador
   - Textos de UI/interface
   - Nomes de headers/seções em configs
   - Descrições de enums e opções

3. ✅ **Workflow de Localização:**
   ```
   Adicionar/Modificar Feature
       ↓
   Adicionar texto em en-US (inglês PRIMEIRO)
       ↓
   Traduzir e adicionar em pt-BR
       ↓
   Verificar formatação hjson (chaves, vírgulas)
       ↓
   Compilar e testar em ambos idiomas
   ```

4. ✅ **Checklist de Localização:**
   - [ ] Texto adicionado em `en-US_Mods.SignatureEquipmentDeluxe.hjson`
   - [ ] Texto traduzido em `pt-BR_Mods.SignatureEquipmentDeluxe.hjson`
   - [ ] Formatação hjson está correta (sem vírgulas extras, chaves balanceadas)
   - [ ] Não há hardcoded strings no código C#
   - [ ] Testado que o texto aparece corretamente in-game

5. ⚠️ **Erros Comuns a Evitar:**
   - ❌ Adicionar texto apenas em um idioma
   - ❌ Usar referências circulares (`{$Mods.SignatureEquipmentDeluxe...}`)
   - ❌ Esquecer chaves de fechamento `}` no final do arquivo
   - ❌ Deixar textos comentados quando deveriam estar ativos
   - ❌ Hardcoded strings no código C#

**LEMBRE-SE:** O mod é bilíngue! SEMPRE atualize os dois arquivos de localização!

### Particle Effects
- Use the `Dust` class for creating visual effects
- Refer to `RadioactiveZoneVisuals.cs` for examples
- Always implement throttling (spawn every N frames)
- Use distance culling to avoid off-screen particle spawns

### Audio Feedback
- Use the `SoundEngine` class for audio cues
- See `LeveledEnemySystem.cs` for implementation details
- Always provide volume and pitch variations

---

## Configuration System

The mod uses specialized configuration files in `Common/Configs/`:

- **GameplayConfig**: Core gameplay toggles and level caps
- **ProgressionConfig**: XP, leveling curves, and kill streaks
- **CombatConfig**: Weapon/armor stats and scaling tiers
- **WorldConfig**: Leveled enemies and world progression
- **RuneConfig**: Rune and curse systems
- **EventsConfig**: Event-based XP multipliers
- **ClientConfig**: Visual and UI settings (client-side)
- **AdvancedConfig**: Blacklists, netcode, and debug options

**Note**: All configs are modular and specialized. No monolithic ServerConfig.cs exists.

---

## Integration Points

### tModLoader
- The mod relies on tModLoader APIs for integration with Terraria
- Hooks used: `ModifyWeaponDamage`, `OnHitNPC`, `PostUpdate`, `AI`, etc.
- Network sync handled via `ModPacket` system

### External Dependencies
- Ensure all required NuGet packages are installed before building the project
- .NET 8.0 SDK required
- tModLoader v2023.8+ required

---

## Architecture Patterns

### Modular Design
- Systems are separated into distinct classes
- Each system has a single, well-defined responsibility
- Use dependency injection where possible (via ModContent.GetInstance)

### Static Helper Classes
- `RuneSystem`, `EventDetector`, `ScalingCalculator` are static helpers
- Stateless operations, pure functions
- No instance state

### Singleton Pattern
- `EventTracker` uses singleton pattern
- Persistent state across game sessions
- Thread-safe if needed for multiplayer

### Cache Pattern
- `SignaturePlayer` caches event multipliers
- `SignatureGlobalItem` caches stat calculations
- Invalidate cache when source data changes

---

## Examples

### Adding a New Visual Effect
1. **Read docs/RFC.md** - Section 3.6 (Visual Effects)
2. Create a new `.cs` file in `Common/Visual/`
3. Implement the effect using the `Dust` or custom particle system
4. Add throttling and distance culling
5. Register in appropriate system (e.g., `RuneElementalEffects`)
6. **Update docs/RFC.md** with new visual effect details
7. **Update docs/README.md** with usage example

### Updating Localization
1. Edit the appropriate `.hjson` file in the `Localization` folder
2. Follow the existing structure to add or update text entries
3. Ensure both `en-US` and `pt-BR` are updated
4. Test in-game to verify

### Adding a New Configuration
1. **Read docs/RFC.md** - Section 3.1 (Configuration)
2. Determine which config file it belongs to (or create new if needed)
3. Add property with attributes (Label, Tooltip, DefaultValue, etc.)
4. **Update docs/PRD.md** with new configuration requirement
5. **Update docs/RFC.md** with configuration architecture
6. **Update docs/README.md** configuration section
7. Test and document default values

### Creating a New System
1. **Read docs/PRD.md** - Add new RF-XXX requirement
2. **Read docs/RFC.md** - Plan architecture
3. Create system class in `Common/Systems/`
4. Implement core logic following existing patterns
5. Add XML documentation comments
6. **Update docs/RFC.md** - Add full architecture section
7. **Update docs/README.md** - Add usage guide
8. **Update README.md** - Add to feature list (if user-visible)
9. Test extensively
10. Commit code + documentation together

---

## Testing Checklist

### Before Committing:
- [ ] Read relevant documentation (PRD, RFC, docs/README)
- [ ] Code compiles without errors (`dotnet build`)
- [ ] All warnings reviewed and addressed/documented
- [ ] Tested in singleplayer
- [ ] Tested in multiplayer (if applicable)
- [ ] XML comments added to public APIs
- [ ] **PRD updated** (if new feature/requirement)
- [ ] **RFC updated** (if architecture change)
- [ ] **docs/README.md updated** (if implementation change)
- [ ] **README.md updated** (if user-visible change)
- [ ] Localization updated (both en-US and pt-BR)
- [ ] No hardcoded text strings
- [ ] Performance tested (60 FPS maintained)

### After Committing:
- [ ] Verify documentation is committed with code
- [ ] Verify documentation is up-to-date and accurate
- [ ] PR includes documentation changes summary

---

## Common Pitfalls to Avoid

1. ❌ **Modifying code without reading docs first**
   - ✅ Always read PRD and RFC before changes

2. ❌ **Committing code without updating documentation**
   - ✅ Update docs in the same commit/PR

3. ❌ **Hardcoding values that should be configurable**
   - ✅ Use appropriate config file

4. ❌ **Ignoring compilation warnings**
   - ✅ Review and fix or document why they're safe

5. ❌ **Not testing in multiplayer**
   - ✅ Always test sync-sensitive code in multiplayer

6. ❌ **Spawning particles every frame**
   - ✅ Implement throttling (every 3-5 frames)

7. ❌ **Not caching expensive calculations**
   - ✅ Cache when appropriate, invalidate when needed

8. ❌ **Using floating point for deterministic logic**
   - ✅ Use integers for game state, floats only for display/effects

---

## Documentation Maintenance Schedule

### Daily (During Development):
- Update inline code comments as you write code
- Add XML documentation to new public APIs

### Per Feature:
- Update PRD with new requirements
- Update RFC with architecture changes
- Update docs/README with implementation details
- Update README.md with user-visible changes

### Per Release:
- Review all documentation for accuracy
- Update version numbers
- Update changelog
- Review and consolidate deprecated features

### Quarterly:
- Full documentation audit
- Remove outdated information
- Improve unclear sections based on feedback
- Update examples and code snippets

---

## Quick Links

- **PRD**: [docs/PRD.md](../docs/PRD.md) - Product requirements and features
- **RFC**: [docs/RFC.md](../docs/RFC.md) - Technical architecture and design
- **Technical Docs**: [docs/README.md](../docs/README.md) - Implementation guide
- **User Guide**: [README.md](../README.md) - User-facing documentation

---

## Questions?

If you're unsure about:
- **What** to implement → Read PRD
- **How** to implement → Read RFC
- **Where** to implement → Read docs/README
- **Why** something exists → Read RFC (Design Decisions)

If still unclear, ask the team before proceeding!

---

**Remember: Code and documentation evolve together. Never commit one without the other!**

For more details, refer to:
- [Product Requirements Document](../docs/PRD.md)
- [Technical Architecture RFC](../docs/RFC.md)
- [README.md](../README.md)