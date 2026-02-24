# proto-0

A text-based dungeon RPG built with C# .NET 9.0 and Blazor WebAssembly.

## Overview

Explore procedurally connected dungeons, battle NPCs, collect items, and complete quests in this classic text-based adventure. The game features a modern web-based interface with real-time action logging, room exploration tracking, and an intuitive 3-column layout.

## Features

- **Entity-Component-System Architecture** — Clean separation of game logic and data
- **Interactive Buttons** — Clickable items and NPCs in terminal output with context menus; exits navigate with single click
- **Rich Output System** — Structured terminal output with entity references and metadata
- **Exit Previews** — Hover over exits to see contextual descriptions of what lies in that direction
- **Room Exploration Tracking** — Permanent tracking of visited rooms; exits marked as explored/unexplored
- **Compass Rose Map** — Visual map showing immediately adjacent rooms in all directions with color-coded exploration status
- **Action Log** — Rolling history of player actions (movement, combat, items, interactions)
- **Smart Terminal** — Persistent room description panel + action/conversation panel
- **Time-of-Day System** — Game clock with visual time widget showing dawn/morning/midday/afternoon/evening/night cycles
- **Command-Driven Interface** — Natural language commands (look, move, take, attack, wear, wield, etc.)
- **Simplified Equipment System** — 11 slots: Head, Body, Arms, Belt, Legs, Feet, Left/Right wielding, 3 accessory slots
- **8 Core Stats** — STR, DEX, FOR, AGI, WIL, INT, PER, CHA (displayed with 3-letter abbreviations)
- **Exercise-Based Stat Growth** — Stats grow through accumulated actions with geometric compounding thresholds (each point costs 15% more)
- **NPC Personality System** — NPCs have dispositions (friendly, standoffish, hostile) affecting interactions
- **Inventory & Crafting** — Collect items, manage equipment, craft new items from recipes
- **Quest System** — Track objectives and earn rewards

## Project Structure

```
proto-0/
├── src/
│   ├── ProtoEngine/      # Core game logic (platform-agnostic)
│   │   ├── Commands/     # Player commands (look, move, take, etc.) + CommandClassifier
│   │   ├── Components/   # Data components (Health, Inventory, Position, Equipment, Stats, Exercise, etc.)
│   │   ├── Systems/      # Game systems (World, Combat, Inventory, ActionLog, StatGrowth, etc.)
│   │   ├── Data/         # Content data structures
│   │   ├── Persistence/  # Save/load mapping (SaveData, SaveDataMapper)
│   │   └── Core/         # ECS framework, events, state management
│   └── ProtoMud/         # Blazor WebAssembly UI
│       ├── Components/   # UI components (Terminal, EntityButton, PlayerInfoPanel, EquipmentPanel, ActionLogPanel, MapPanel, TimeWidget, StatsPanel)
│       ├── Pages/        # Blazor pages (Home, Game)
│       ├── Services/     # Game session hosting
│       └── wwwroot/      # Static assets and game content (JSON)
├── tests/
│   ├── ProtoEngine.Tests/  # xUnit tests for core game logic (184 tests)
│   └── ProtoMud.Tests/     # bUnit + xUnit tests for Blazor UI (8 tests)
└── CLAUDE.md             # Development guide for contributors
```

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- A modern web browser (Chrome, Firefox, Edge, Safari)

### Running the Game

```bash
# Clone the repository
git clone https://github.com/MercuriusXeno/proto-0.git
cd proto-0

# Run the game
cd src/ProtoMud
dotnet run

# Open browser to displayed localhost URL (typically https://localhost:5001)
```

### Building

```bash
# Build entire solution
dotnet build

# Build specific project
cd src/ProtoEngine
dotnet build
```

## How to Play

The game uses natural language commands in the terminal. Try these to get started:

- `look` — Examine your current surroundings
- `north`, `south`, `east`, `west` — Move in a direction
- `take [item]` — Pick up an item
- `inventory` — View your items
- `use [item]` — Use a consumable item (potions, food)
- `wear [item]` — Wear armor or clothing
- `wield [item] [left|right]` — Wield a weapon or shield (defaults to right hand)
- `unwield [left|right]` — Stop wielding an item
- `attack [npc]` — Attack an NPC
- `talk [npc]` — Speak to an NPC
- `status` — View character stats and equipment
- `help` — See all available commands

**Tips:**
- Click on highlighted items and NPCs to see context menu actions
- Click on exits to instantly navigate in that direction
- Hover over exits to preview what you can see in that direction

## Architecture

**Entity-Component-System (ECS)**
- **Entity**: Generic container (Player, NPCs, Items)
- **Component**: Pure data structures (Health, Position, Stats)
- **System**: Logic processors (Combat, Inventory, World, ActionLog)

**Command System**
- Player inputs parsed into commands
- Commands execute against game state
- Results displayed in terminal
- Game clock advances, systems tick

**Event Bus**
- Pub/sub pattern for game events
- Systems react to events (item picked up, NPC killed, room entered)

See [CLAUDE.md](CLAUDE.md) for detailed development documentation.

## Game Content

All game content is defined in JSON files located in `src/ProtoMud/wwwroot/data/`:
- `rooms.json` — Room definitions, exits, exit previews, items, NPCs
- `items.json` — Item properties, stats, effects
- `npcs.json` — NPC stats, dialogue, loot tables
- `quests.json` — Quest objectives and rewards
- `recipes.json` — Crafting recipes
- `dialogue.json` — NPC conversation trees

## Development Roadmap

- [x] **Goal #1: Output Window Management** — Action logging, smart terminal clearing, room exploration tracking
- [x] **Goal #2: Interactive Buttons** — Clickable elements with context menus (items/NPCs) and direct navigation (exits)
- [x] **Goal #3: Navigation & Mapping** — Compass rose map showing adjacent rooms with directional layout
- [~] **Goal #4: Expanded Attributes** — 8 streamlined stats ✓; exercise-based growth seam ✓; exercise consumers and rebirth system pending
- [ ] **Goal #5: Comprehensive Skills** — Magic types, skill progression, command unlocks
- [ ] **Goal #6: Intelligent Aliasing** — Smart partial matching, disambiguation, item tagging
- [ ] **Goal #7: Content Editor** — Visual toolchain for creating game assets

## Contributing

This project uses Claude Code for development assistance. See [CLAUDE.md](CLAUDE.md) for:
- Architecture patterns
- How to add new components, systems, and commands
- Development workflow and conventions

## License

This project is a personal experiment and learning project.

## Acknowledgments

Built with assistance from [Claude Code](https://claude.ai/code) by Anthropic.
