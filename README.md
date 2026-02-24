# proto-0

A text-based dungeon RPG built with C# .NET 9.0 and Blazor WebAssembly.

## Overview

Explore procedurally connected dungeons, battle NPCs, collect items, and complete quests in this classic text-based adventure. The game features a modern web-based interface with real-time action logging, contextual memory system, and an intuitive 3-column layout.

## Features

- **Entity-Component-System Architecture** — Clean separation of game logic and data
- **Memory System** — Tracks visited rooms, discovered items, and NPC encounters with timestamps
- **Action Log** — Rolling history of player actions (movement, combat, items, interactions)
- **Smart Terminal** — Clears for room-viewing commands, shows contextual room descriptions
- **Command-Driven Interface** — Natural language commands (look, move, take, attack, etc.)
- **Inventory & Crafting** — Collect items, manage equipment, craft new items from recipes
- **Quest System** — Track objectives and earn rewards
- **NPC Interactions** — Dialogue trees and combat encounters

## Project Structure

```
proto-0/
├── src/
│   ├── ProtoEngine/      # Core game logic (platform-agnostic)
│   │   ├── Commands/     # Player commands (look, move, take, etc.)
│   │   ├── Components/   # Data components (Health, Inventory, Position, etc.)
│   │   ├── Systems/      # Game systems (World, Combat, Memory, etc.)
│   │   ├── Data/         # Content data structures
│   │   └── Core/         # ECS framework, events, state management
│   └── ProtoMud/         # Blazor WebAssembly UI
│       ├── Components/   # UI components (Terminal, ActionLog, Stats, Map)
│       ├── Pages/        # Blazor pages
│       ├── Services/     # Game session hosting
│       └── wwwroot/      # Static assets and game content (JSON)
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
- `equip [item]` — Equip an item
- `attack [npc]` — Attack an NPC
- `talk [npc]` — Speak to an NPC
- `help` — See all available commands

## Architecture

**Entity-Component-System (ECS)**
- **Entity**: Generic container (Player, NPCs, Items)
- **Component**: Pure data structures (Health, Position, Stats)
- **System**: Logic processors (Combat, Inventory, World, Memory)

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
- `rooms.json` — Room definitions, exits, items, NPCs
- `items.json` — Item properties, stats, effects
- `npcs.json` — NPC stats, dialogue, loot tables
- `quests.json` — Quest objectives and rewards
- `recipes.json` — Crafting recipes
- `dialogue.json` — NPC conversation trees

## Development Roadmap

- [x] **Goal #1: Output Window Management** — Memory system, action logging, smart terminal clearing
- [ ] **Goal #2: Interactive Buttons** — Clickable elements in output with context menus
- [ ] **Goal #3: Navigation & Mapping** — Visual map showing visited rooms and connections
- [ ] **Goal #4: Expanded Attributes** — 13 diverse stats with action-based growth and rebirth
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
