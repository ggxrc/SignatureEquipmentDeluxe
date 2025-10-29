# Copilot Instructions for SignatureEquipmentDeluxe

## Overview
SignatureEquipmentDeluxe is a Terraria mod built using the tModLoader framework. It extends the functionality of the original SignatureEquipment mod by introducing advanced systems like radioactive zones, leveled enemies, and cursed weapons. The mod is modular, with clear separation of concerns across its components.

## Key Components
- **LeveledEnemySystem.cs**: Manages radioactive zones and leveled enemies.
- **RadioactiveZoneVisuals.cs**: Handles visual effects for radioactive zones.
- **CurseDeathSystem.cs**: Manages cursed weapon drops and zone upgrades.
- **RadioactiveZoneDebuffs.cs**: Applies punitive effects near the center of radioactive zones.
- **ProjectileTrailEffect.cs**: Adds visual effects to projectiles.

## Developer Workflows
### Building the Mod
1. Navigate to the mod's root directory.
2. Run the following command to build the mod:
   ```powershell
   dotnet build
   ```

### Testing the Mod
1. Launch Terraria with tModLoader.
2. Enable the SignatureEquipmentDeluxe mod in the Mods menu.
3. Test the features in-game, focusing on:
   - Radioactive zone effects.
   - Danger level updates and visual/audio feedback.
   - Leveled enemies and cursed weapon mechanics.

### Debugging
- Use `launchSettings.json` in the `Properties` folder to configure debugging settings.
- Leverage tModLoader's built-in debugging tools for real-time testing.

## Project-Specific Conventions
- **Localization**: All text is localized using `.hjson` files in the `Localization` folder. Ensure updates to `en-US` and `pt-BR` files are consistent.
- **Particle Effects**: Use the `Dust` class for creating visual effects. Refer to `RadioactiveZoneVisuals.cs` for examples.
- **Audio Feedback**: Use the `SoundEngine` class for audio cues. See `LeveledEnemySystem.cs` for implementation details.

## Integration Points
- **tModLoader**: The mod relies on tModLoader APIs for integration with Terraria.
- **External Dependencies**: Ensure all required NuGet packages are installed before building the project.

## Examples
### Adding a New Visual Effect
1. Create a new `.cs` file in the `Content/Projectiles` folder.
2. Implement the effect using the `Projectile` class.
3. Register the projectile in the mod's main class.

### Updating Localization
1. Edit the appropriate `.hjson` file in the `Localization` folder.
2. Follow the existing structure to add or update text entries.

## Notes
- Always test changes in both `en-US` and `pt-BR` locales.
- Maintain modularity by isolating new features into separate files or classes.

For more details, refer to the [README.md](../README.md).