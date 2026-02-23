# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a text-based dungeon RPG built with C# .NET 9.0 and Blazor WebAssembly. The game uses an Entity-Component-System (ECS) architecture with a command-driven player interface.

## Project Structure

- **ProtoEngine** (`src/ProtoEngine/`): Core game logic library
  - Platform-agnostic, contains all game systems, components, and commands
- **ProtoMud** (`src/ProtoMud/`): Blazor WebAssembly UI layer
  - Hosts the engine and provides the browser-based interface
  - 3-column layout: Stats/Map (left), Terminal/ActionBar (center), Action Log (right)

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
- Examples: `PositionComponent`, `HealthComponent`, `StatsComponent`, `InventoryComponent`
- Stored in `Components/` directory
- No logic - just data

**System** (implement `IGameSystem`): Logic processors
- Operate on entities with specific components
- Examples: `WorldSystem`, `PlayerSystem`, `CombatSystem`, `InventorySystem`
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
- Examples: `RoomEnteredEvent`, `EntityDiedEvent`, `ItemPickedUpEvent`

### Game Session

**GameSession** (`Session/GameSession.cs`):
- Central orchestrator that ties everything together
- Owns `GameState`, `CommandRegistry`, and all registered systems
- `ProcessCommand()` executes commands and advances game time
- All systems are registered here at initialization

**GameSessionHost** (`ProtoMud/Services/GameSessionHost.cs`):
- Blazor service that hosts the engine
- Loads game content from JSON files in `wwwroot/data/`
- Creates and wires up all systems and commands
- Handles auto-save after each command
- Exposes systems to UI for rendering (World, Player, Inventory, etc.)

### Game Content

**Content Data** (`Data/` directory):
- `RoomData`, `ItemData`, `NpcData`, `QuestData`, `RecipeData`, `DialogueData`
- All loaded from JSON files in `src/ProtoMud/wwwroot/data/`
- Managed by `ContentManifest` and used by systems to instantiate game entities

**Content Files**:
- `rooms.json`: Room definitions with exits, items, and NPCs
- `items.json`: Item properties, stats, effects
- `npcs.json`: NPC stats, dialogue, loot tables
- `quests.json`: Quest objectives and rewards
- `recipes.json`: Crafting recipes
- `dialogue.json`: NPC conversation trees

## Key Subsystems

### Memory System
- Tracks player's visited rooms, discovered items, NPC encounters
- Enables contextual room descriptions with timestamps
- Component: `RoomMemoryComponent`
- System: `MemorySystem`
- Commands reference this for "remembered" text

### Action Log System
- Records all player actions in a rolling log (max 100 entries)
- Displayed in right sidebar
- Component: `ActionLogComponent`
- System: `ActionLogSystem`
- Categories: Movement, Combat, Items, Interactions, System

### Terminal Behavior
- **Room-viewing commands** (look, move, directional): Clear terminal and show only current room state
- **Other commands**: Append output to terminal with command echo
- Action log persists across all commands for historical reference

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
6. Return `CommandResult.Success()` or `CommandResult.Fail()` with message

### Adding a New UI Component
1. Create `.razor` file in `src/ProtoMud/Components/`
2. Inject `GameSessionHost` to access game state
3. Subscribe to system state changes if needed
4. Add to layout in `Game.razor` or `MainLayout.razor`

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
Commands should check if they're "room-viewing" commands:
- **Clear mode**: Set `ClearsTerminal = true` in CommandResult
- **Append mode**: Default, just return messages normally

### Memory Integration
Commands that discover or interact with entities should record memories:
```csharp
memorySystem.RecordItemFound(state, itemId, roomId);
memorySystem.RecordExitExplored(state, roomId, direction);
```

## Dependencies

- .NET 9.0
- Blazor WebAssembly
- Blazored.LocalStorage (for save/load persistence)
- No external game frameworks - custom ECS implementation
