# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a text-based dungeon RPG built with C# .NET 9.0 and Blazor WebAssembly. The game uses an Entity-Component-System (ECS) architecture with a command-driven player interface.

## Project Structure

- **ProtoEngine** (`src/ProtoEngine/`): Core game logic library
  - Platform-agnostic, contains all game systems, components, and commands
  - `Commands/` — Player commands including EntityReference and OutputLine for rich output
  - `Components/` — Data components including EquipmentComponent with 24 slots (16 body + 6 relic + 2 wielding)
  - `Systems/` — Game systems including MemorySystem, ActionLogSystem, NpcSystem
  - `Data/` — Content structures including NpcDisposition for personality traits
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
- Tracks player's visited rooms (with visit counts), items taken, NPC encounters
- **Visit-based recollection**: Items only remembered if picked up; rooms show memories on revisits
- Enables contextual room descriptions with timestamps
- Component: `RoomMemoryComponent` (with RoomVisitCounts, CurrentRoomId)
- System: `MemorySystem`
- Commands reference this for "remembered" text

### Equipment System
- **24 Total Slots**: 16 body + 6 relic + 2 wielding
- **Body Slots**: Head, Face, Neck, UpperTorso, LowerTorso, Waist, Shoulder, UpperArm, Elbow, Forearm, Wrist, Hand, Thigh, Knee, Calf, Ankle, Foot
- **Relic Slots**: Relic1-6 (for rings, amulets, accessories)
- **Wielding Slots**: WieldLeft, WieldRight (for weapons, shields, bows)
- **Layer Support**: Each slot can hold multiple items (layering for armor)
- **Wear vs Wield**: `WearCommand` for armor/clothing (worn on body), `WieldCommand` for weapons/shields (held in hands)
- **Unarmed Combat**: Empty wielding slots = hand-to-hand attacks
- Component: `EquipmentComponent` with Dictionary<EquipmentSlot, List<EquippedItem>>

### Character Stats (13 Attributes)
- **Physical**: Strength, Agility, Dexterity, Vitality
- **Mental**: Perception, Intelligence, Willpower, Memory
- **Social**: Charisma, Luck, Fate
- **Special**: Eldritch, Racial
- All stats start at 10; intended for action-based growth (not yet implemented)

### Action Log System
- Records all player actions in a rolling log (max 100 entries)
- Displayed in right sidebar
- Component: `ActionLogComponent`
- System: `ActionLogSystem`
- Categories: Movement, Combat, Items, Interactions, System

### Terminal Behavior
- **Split Panels**: Room description (persistent) + action/conversation output (clears on each command)
- **Room-viewing commands** (look, move, directional): Update room description panel
- **Other commands**: Display results in action panel
- **Auto-look**: Navigation commands can auto-execute "look" (toggleable checkbox)
- **Real-time updates**: Commands can set `RefreshRoomDescription` flag to update room panel immediately

### Interactive Buttons (Rich Output System)
- **EntityReference**: Metadata for clickable entities (items, NPCs, exits) with available actions
- **OutputLine**: Rich output structure containing text + entity references
- **EntityButton**: Blazor component rendering clickable entities with context menus
- **CommandResult.RichOutput**: Backwards-compatible rich output (falls back to plain text)
- Commands can return `CommandResult.OkRich()` with OutputLine arrays
- Terminal parses entity references and renders EntityButton components inline
- Click entity → context menu → execute action (e.g., click sword → "Take" → executes "take sword")

### NPC System
- **Name vs Title**: NPCs have separate name ("Aldric") and title ("merchant")
- **Disposition System**: NpcDisposition defines personality (friendly, standoffish, hostile)
- **Discovery**: NPCs show as "someone" until talked to; disposition affects introduction
- **Memory Integration**: MemorySystem tracks NPC meetings (requires talking, not just looking)

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
- **EntityButton**: Renders clickable entities with context menus (used by Terminal)
- **PlayerInfoPanel**: Tabbed interface with Equipment and Stats tabs
- **EquipmentPanel**: Displays all 24 equipment slots by region (Wielding, Head, Torso, Arms, Legs, Relics)
- **StatsPanel**: Shows all 13 character attributes
- **ActionLogPanel**: Rolling log of player actions (max 100 entries)
- **MapPanel**: Room navigation and map display (currently simple, placeholder for future visual map)
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
  - Example: TalkCommand sets this on first NPC meeting to update "someone" → "Name the Title"

### Memory Integration
Commands that interact with entities should record memories:
```csharp
// Items: Record when TAKEN, not just seen
memorySystem.RecordItemTaken(state, itemId, roomId);

// NPCs: Record when TALKED to (requires TalkCommand)
memorySystem.RecordNpcMeeting(state, npcId, roomId);

// Exits: Record when explored
memorySystem.RecordExitExplored(state, roomId, direction);

// Rooms: Visit tracking happens automatically in MoveCommand
// Use GetCurrentVisitNumber() to check if revisit
var visitNumber = memorySystem.GetCurrentVisitNumber(state, roomId);
var isRevisit = visitNumber >= 1;
```

## Dependencies

- .NET 9.0
- Blazor WebAssembly
- Blazored.LocalStorage (for save/load persistence)
- No external game frameworks - custom ECS implementation
