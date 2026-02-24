# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a text-based dungeon RPG built with C# .NET 9.0 and Blazor WebAssembly. The game uses an Entity-Component-System (ECS) architecture with a command-driven player interface.

## Project Structure

- **ProtoEngine** (`src/ProtoEngine/`): Core game logic library
  - Platform-agnostic, contains all game systems, components, and commands
  - `Commands/` — Player commands, EntityReference, OutputLine for rich output, CommandClassifier for UI routing
  - `Components/` — Data components including EquipmentComponent (11 slots), StatsComponent (8 stats), ExerciseComponent, ExploredRoomsComponent, StatType enum
  - `Systems/` — Game systems including ActionLogSystem, NpcSystem, WorldSystem, InventorySystem, CombatSystem, StatGrowthSystem
  - `Data/` — Content structures including NpcDisposition, RoomData with ExitPreviews
  - `Persistence/` — Save/load: SaveData, SaveDataMapper, ISaveLoadService
- **ProtoMud** (`src/ProtoMud/`): Blazor WebAssembly UI layer
  - Hosts the engine and provides the browser-based interface
  - 3-column layout: PlayerInfo/Map (left split), Terminal/ActionBar (center), Action Log (right)
  - Left sidebar split: PlayerInfoPanel (top) with Equipment/Stats tabs, MapPanel (bottom)
  - Terminal split: Persistent room description panel (top), action/conversation panel (bottom)

## Development Commands

### Running the Game
```bash
cd src/ProtoMud
dotnet run
```

### Building
```bash
# Build entire solution
dotnet build

# Build specific project
cd src/ProtoEngine
dotnet build
```

## Core Architecture

### Entity-Component-System (ECS) Pattern

**Entity** (`Core/Entity.cs`): Generic container identified by ID and Tag
- Holds a dictionary of components
- Player, NPCs, and items are all entities
- Use `Add<T>()`, `Get<T>()`, `Has<T>()` to manage components

**Component** (implement `IComponent`): Pure data structures
- Examples: `PositionComponent`, `HealthComponent`, `StatsComponent`, `InventoryComponent`, `EquipmentComponent`, `ExploredRoomsComponent`, `ExerciseComponent`
- Stored in `Components/` directory
- No logic - just data

**System** (implement `IGameSystem`): Logic processors
- Operate on entities with specific components
- Examples: `WorldSystem`, `PlayerSystem`, `CombatSystem`, `InventorySystem`, `ActionLogSystem`, `NpcSystem`, `StatGrowthSystem`
- Registered with `GameSession` at startup
- Implement `ITickable` to receive per-turn updates
- Initialize via `Initialize(GameState state)` method

### Command System

**Commands** (implement `ICommand`):
- Handle player input (look, move, take, attack, etc.)
- Located in `Commands/Commands/`
- Each command has a verb, aliases, description, and Execute method
- Registered with `CommandRegistry` at startup
- Return `CommandResult` with success/failure and output message

**Command Flow**:
1. Player types command in Terminal
2. `CommandParser` splits input into verb and args
3. `CommandRegistry` resolves verb to command instance
4. Command's `Execute()` method runs with `CommandContext` (contains GameState and EventBus)
5. `CommandResult` returned and displayed
6. GameClock advances, all `ITickable` systems tick

### Event System

**EventBus** (`Events/EventBus.cs`):
- Pub/sub pattern for game events
- Events defined as records in `Events/GameEvents.cs`
- Systems can subscribe to events and react accordingly
- Examples: `RoomEnteredEvent`, `EntityDiedEvent`, `ItemPickedUpEvent`, `StatExercisedEvent`, `StatIncreasedEvent`

### Game Session

**GameSession** (`Session/GameSession.cs`):
- Central orchestrator that ties everything together
- Owns `GameState`, `CommandRegistry`, and all registered systems
- `ProcessCommand()` executes commands and advances game time
- All systems are registered here at initialization

**GameSessionHost** (`ProtoMud/Services/GameSessionHost.cs`):
- Blazor service that hosts the engine
- Loads game content from JSON files in `wwwroot/data/`
- Creates and wires up all systems and commands via `RegisterSystems()`/`RegisterCommands()`
- Delegates save/load mapping to `SaveDataMapper`
- Handles auto-save after each command
- Exposes systems to UI for rendering (World, Player, Inventory, etc.)

### Game Content

**Content Data** (`Data/` directory):
- `RoomData`, `ItemData`, `NpcData`, `QuestData`, `RecipeData`, `DialogueData`
- All loaded from JSON files in `src/ProtoMud/wwwroot/data/`
- Managed by `ContentManifest` and used by systems to instantiate game entities

**Content Files**:
- `rooms.json`: Room definitions with exits, exit previews, items, and NPCs
- `items.json`: Item properties, stats, effects
- `npcs.json`: NPC stats, dialogue, loot tables
- `quests.json`: Quest objectives and rewards
- `recipes.json`: Crafting recipes
- `dialogue.json`: NPC conversation trees

## Key Subsystems

### Room Exploration Tracking
- Tracks which rooms the player has visited (permanent, no degradation)
- Component: `ExploredRoomsComponent` (HashSet of visited room IDs)
- Updated by `MoveCommand` when entering new rooms
- Used by `LookCommand` to show explored vs unexplored exits
- Simple and lightweight - just tracks presence, no timestamps or visit counts

### Equipment System
- **11 Total Slots**: Head, Body, Arms, Belt, Legs, Feet, WieldLeft, WieldRight, Accessory1, Accessory2, Accessory3
- **Simplified Design**: One item per slot, no layering
- **Wear vs Wield**: `WearCommand` for armor/clothing (worn on body), `WieldCommand` for weapons/shields (held in hands)
- **Unarmed Combat**: Empty wielding slots = hand-to-hand attacks
- Component: `EquipmentComponent` with Dictionary<EquipmentSlot, EquippedItem?>

### Character Stats (8 Attributes)
- **STR** (Strength): Physical power, melee damage
- **DEX** (Dexterity): Fine motor control, ranged attacks
- **FOR** (Fortitude): Endurance, health, resistance
- **AGI** (Agility): Speed, dodge, initiative
- **WIL** (Willpower): Mental fortitude, magic resistance
- **INT** (Intelligence): Knowledge, magic power
- **PER** (Perception): Awareness, detection
- **CHA** (Charisma): Social influence, persuasion
- All stats start at 10; displayed with 3-letter abbreviations
- Component: `StatsComponent` with `GetStat(StatType)`/`SetStat(StatType, int)` generic accessors
- Enum: `StatType` for type-safe stat references

### Exercise-Based Stat Growth
- Stats grow through accumulated exercise, not leveling
- **Growth curve**: Geometric compounding — `threshold(n) = baseCost * ratio^n`
  - `baseCost` = 100 (exercise needed for first point)
  - `ratio` = 1.15 (15% compounding per subsequent point)
  - Per-stat overrides supported via `StatGrowthConfig.Overrides`
- **Exercise seam**: `StatGrowthSystem.Exercise(state, stat, amount)` — single API any command/system calls
  - Adds progress to `ExerciseComponent`, publishes `StatExercisedEvent`
  - Checks threshold, increments stat if met, publishes `StatIncreasedEvent`
  - Excess exercise rolls over (loop handles multiple level-ups)
- Component: `ExerciseComponent` — `Dictionary<StatType, double>` tracking progress per stat
- System: `StatGrowthSystem` — exposed as `GameSessionHost.StatGrowth`
- Exercise progress persisted in save/load

### Action Log System
- Records all player actions in a rolling log (max 100 entries)
- Displayed in right sidebar
- Component: `ActionLogComponent`
- System: `ActionLogSystem`
- Categories: Movement, Combat, Items, Interactions, System

### Terminal Behavior
- **Split Panels**: Room description (persistent) + action/conversation output (clears on each command)
- **Command Routing**: `CommandClassifier` (in ProtoEngine) classifies commands; Terminal's `RouteCommandResult()` directs output to the correct panel
- **Room-viewing commands** (look, move, directional): Update room description panel
- **Other commands**: Display results in action panel
- **Auto-look**: Navigation commands can auto-execute "look" (toggleable checkbox)
- **Real-time updates**: Commands can set `RefreshRoomDescription = true` to update room panel immediately
- **Public API**: `Terminal.ExecuteCommand(string)` — single entry point for external callers (ActionBar, MapPanel)

### Interactive Buttons (Rich Output System)
- **EntityReference**: Metadata for clickable entities (items, NPCs, exits) with available actions and optional tooltip text
- **OutputLine**: Rich output structure containing text + entity references
- **EntityButton**: Blazor component rendering clickable entities
  - Items/NPCs: Show context menu on click
  - Exits: Direct navigation on click, preview on hover
- **CommandResult.RichOutput**: Backwards-compatible rich output (falls back to plain text)
- Commands can return `CommandResult.OkRich()` with OutputLine arrays
- Terminal parses entity references and renders EntityButton components inline

### Exit Previews
- Each room can define contextual preview text for each exit direction
- Previews stored in `RoomData.ExitPreviews` dictionary (direction → description)
- Displayed as tooltip when hovering over exit buttons
- Enables context-aware descriptions (e.g., "From the top of the hill you can see...")
- Example in rooms.json:
  ```json
  {
    "id": "town_square",
    "exits": { "north": "tavern" },
    "exitPreviews": {
      "north": "The warm glow of lantern light spills from the tavern's windows..."
    }
  }
  ```

### NPC System
- **Name vs Title**: NPCs have separate name ("Aldric") and title ("merchant")
- **Disposition System**: NpcDisposition defines personality (friendly, standoffish, hostile)
- NPCs always show their names (no discovery mechanic)
- Component: `NpcComponent` with name, title, disposition

### Compass Rose Map
- **Visual Layout**: SVG-based orthogonal compass showing immediately adjacent rooms
- **Positioning**: 8 cardinal/ordinal directions (N, NE, E, SE, S, SW, W, NW) + Up (above N) and Down (below S)
- **Node Display**: Direction label above each node, room name below (if explored)
- **Color Coding**:
  - Current room: Light blue fill with pulsing animation, labeled "YOU"
  - Explored rooms: Grey fill with room name displayed
  - Unexplored rooms: Dark grey fill, no room name shown
- **Component**: `MapPanel.razor` with configurable compass radius and spacing
- **Data Source**: Uses `ExploredRoomsComponent` to determine which rooms have been visited

## Adding New Features

### Adding a New Component
1. Create class in `src/ProtoEngine/Components/` implementing `IComponent`
2. Define data fields needed
3. Add to entities via `entity.Add<YourComponent>(new YourComponent { ... })`

### Adding a New System
1. Create class in `src/ProtoEngine/Systems/` implementing `IGameSystem`
2. Optionally implement `ITickable` for per-turn updates
3. Initialize entities/components in `Initialize(GameState state)`
4. Register in `GameSessionHost.InitializeAsync()`
5. Expose publicly in `GameSessionHost` if UI needs access

### Adding a New Command
1. Create class in `src/ProtoEngine/Commands/Commands/` implementing `ICommand`
2. Define Verb, Aliases, and Description
3. Inject required systems via constructor
4. Implement `Execute(CommandContext context, string[] args)` logic
5. Register in `GameSessionHost.InitializeAsync()`
6. Return `CommandResult.Ok()` or `CommandResult.Fail()` with message
   - For rich output: Return `CommandResult.OkRich()` with OutputLine[] containing EntityReferences
   - For real-time room updates: Set `RefreshRoomDescription = true` in result

**Command Types**:
- **UseCommand**: Consumables only (potions, food) — removes item from inventory
- **WearCommand**: Armor and clothing — equips to body slots
- **WieldCommand**: Weapons and shields — equips to WieldLeft/WieldRight slots
- **UnwieldCommand**: Stop wielding — removes from wielding slots

### Adding a New UI Component
1. Create `.razor` file in `src/ProtoMud/Components/`
2. Inject `GameSessionHost` to access game state
3. Subscribe to system state changes if needed
4. Add to layout in `Game.razor` or `MainLayout.razor`

**Key UI Components**:
- **Terminal**: Split into room-description-panel and action-output-panel
- **EntityButton**: Renders clickable entities; context menus for items/NPCs, direct navigation for exits, tooltips for previews
- **PlayerInfoPanel**: Tabbed interface with Equipment and Stats tabs
- **EquipmentPanel**: Displays all 11 equipment slots in simple list (Right/Left wield slots at top)
- **StatsPanel**: Shows all 8 character attributes with 3-letter abbreviations
- **ActionLogPanel**: Rolling log of player actions (max 100 entries)
- **MapPanel**: SVG-based compass rose showing immediately adjacent rooms in all cardinal/ordinal directions plus Up/Down
- **TimeWidget**: Displays current time of day (Dawn/Morning/Midday/Afternoon/Evening/Night) and day number based on game clock

## Important Patterns

### Accessing Game State
```csharp
// In a command or system
var player = context.State.Player;
var position = player.Get<PositionComponent>();
var health = player.Get<HealthComponent>();

// Query entities
var npcsInRoom = context.State.EntitiesWith<NpcComponent>()
    .Where(e => e.Get<PositionComponent>()?.RoomId == currentRoomId);
```

### Publishing Events
```csharp
context.EventBus.Publish(new ItemPickedUpEvent(playerId, itemId));
```

### Terminal Output Modes
Terminal has two panels that update differently:
- **Room description panel**: Updated by look/move commands, persists until next room-viewing command
- **Action output panel**: Clears on every command, shows immediate action results
- Commands can set `RefreshRoomDescription = true` to trigger automatic room description update

### Room Exploration Tracking
Commands that navigate to new rooms should track visits:
```csharp
// In MoveCommand - track visited room
var explored = context.State.Player.Get<ExploredRoomsComponent>();
if (explored == null)
{
    explored = new ExploredRoomsComponent();
    context.State.Player.Add(explored);
}
explored.VisitedRoomIds.Add(newRoomId);

// In LookCommand - check if exit leads to visited room
var explored = context.State.Player.Get<ExploredRoomsComponent>();
var isExplored = explored?.VisitedRoomIds.Contains(exit.Value) == true;
```

### Exercising Stats
Commands or systems that should contribute to stat growth call the exercise seam:
```csharp
// In a command — e.g. melee attack exercises Strength
var statGrowth = /* injected StatGrowthSystem */;
statGrowth.Exercise(context.State, StatType.Strength, 10.0);

// Fractional amounts are supported
statGrowth.Exercise(context.State, StatType.Perception, 0.5);
```

## Coding Standards

### SOLID & Method Design
- **Single Responsibility**: Each method does one thing. When a method exceeds ~25 lines, treat it as a smell and extract well-named helper methods.
- **Self-Documenting Code**: Choose descriptive names for methods, variables, and parameters so the code reads clearly without inline comments. Avoid code comments — if logic isn't obvious from naming alone, rename or restructure until it is.
- **XML Documentation**: Use `<summary>` XML docs on public and internal methods to describe *what* and *why*, not *how*. Skip XML docs on private methods unless the purpose is non-obvious from the name.
- **No Dead Code**: Remove unused methods, variables, and using statements. Don't leave commented-out code.

### Seams & Testability
- **Internal over Private**: When extracting helper methods from a refactor, mark them `internal` rather than `private` so tests can exercise them directly.
- **InternalsVisibleTo**: Both `ProtoEngine` and `ProtoMud` expose internals to their respective test projects (`ProtoEngine.Tests`, `ProtoMud.Tests`).
- **Constructor Injection**: Systems and commands receive dependencies through constructors. Prefer injecting interfaces over concrete types where feasible.

### General Conventions
- **No Magic Numbers**: Extract constants or config values for thresholds, limits, and tuning parameters.
- **Early Return**: Prefer guard clauses and early returns over deeply nested conditionals.
- **Immutable Data Where Practical**: Use records for events, DTOs, and value objects. Components are mutable by design (ECS pattern).
- **Naming**: PascalCase for public/internal members, camelCase for locals and parameters, `_camelCase` for private fields.

## Testing

### Test Projects
- **ProtoEngine.Tests** (`tests/ProtoEngine.Tests/`): xUnit, 184 tests
  - Commands: CommandClassifierTests, CommandParserTests, CommandRegistryTests, LookCommandTests, MoveCommandTests, StatusCommandTests
  - Components: StatsComponentTests, ExerciseComponentTests
  - Core: EntityTests, GameStateTests, GameClockTests
  - Events: EventBusTests
  - Persistence: SaveDataRoundTripTests
  - Session: GameSessionTests
  - Systems: StatGrowthSystemTests
- **ProtoMud.Tests** (`tests/ProtoMud.Tests/`): bUnit + xUnit, 8 tests
  - Components: EntityButtonTests

### Running Tests
```bash
# All tests
dotnet test

# Specific project
dotnet test tests/ProtoEngine.Tests
dotnet test tests/ProtoMud.Tests
```

### Test Conventions
- Test class naming: `{ClassUnderTest}Tests` (e.g., `StatGrowthSystemTests`)
- Test method naming: `MethodName_Scenario_ExpectedResult` (e.g., `Exercise_ExceedsThreshold_IncrementsStat`)
- Arrange/Act/Assert structure
- One assertion concept per test (multiple asserts are fine if they verify the same logical outcome)

## Dependencies

- .NET 9.0
- Blazor WebAssembly
- Blazored.LocalStorage (for save/load persistence)
- No external game frameworks - custom ECS implementation
- xUnit, bUnit (test projects only)
