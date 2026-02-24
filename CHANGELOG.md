# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/).

## [Unreleased]

### Added
- Unit tests for P2 extracted internal static methods: CommandClassifierTests, LookCommandTests (BuildItemLines/BuildNpcLines/BuildExitLines/GetArticle), MoveCommandTests (ResolveDirection), StatusCommandTests (ToAbbreviation) — 81 new tests (103 → 184 engine tests)
- `CommandClassifier` static class for centralized command type classification (look vs navigation vs other)
- `SaveDataMapper` class extracting save/load property mapping out of GameSessionHost
- Exercise-based stat growth system with geometric compounding thresholds (`StatGrowthSystem`)
- `StatType` enum for type-safe stat references across systems
- `ExerciseComponent` tracking accumulated exercise progress per stat
- `StatsComponent.GetStat()`/`SetStat()` generic accessors via `StatType`
- `StatExercisedEvent` and `StatIncreasedEvent` game events
- Per-stat growth curve overrides via `StatGrowthConfig`
- Text-based dungeon RPG built with C# .NET 9.0 and Blazor WebAssembly
- Entity-Component-System (ECS) architecture for game logic
- Command-driven gameplay with natural language commands (look, move, take, attack, talk, etc.)
- Interactive buttons with context menus for items and NPCs; single-click navigation for exits
- Rich output system with entity references and metadata
- Action log system recording player actions in rolling history (max 100 entries)
- Simplified equipment system with 11 slots: Head, Body, Arms, Belt, Legs, Feet, WieldLeft, WieldRight, Accessory1-3
- 8 streamlined character stats with 3-letter abbreviations: STR, DEX, FOR, AGI, WIL, INT, PER, CHA
- NPC disposition system (friendly, standoffish, hostile) affecting interactions
- Inventory, crafting, combat, and quest systems
- JSON-based game content (rooms, items, NPCs, quests, recipes, dialogue)
- Local storage save/load functionality
- WearCommand for armor and clothing
- WieldCommand for weapons and shields with left/right hand specification
- UnwieldCommand for removing wielded items
- EntityButton component for clickable game elements with context menus and tooltips
- PlayerInfoPanel with tabbed Equipment/Stats interface
- EquipmentPanel displaying all 11 equipped items in simple list
- StatsPanel showing all 8 character attributes with abbreviated labels
- TimeWidget displaying current time of day (Dawn/Morning/Midday/Afternoon/Evening/Night) and day number
- Session notes and development documentation (CLAUDE.md, README.md)
- Comprehensive development guide for contributors
- ExploredRoomsComponent for permanent room visit tracking
- Exit preview system with hover tooltips showing contextual directional descriptions
- RoomData.ExitPreviews for defining context-aware exit descriptions per room
- Compass rose map (MapPanel) showing adjacent rooms in all directions with SVG rendering
- Color-coded map nodes: current room (light blue), explored (grey), unexplored (dark grey)
- Direction labels above nodes, room names below (if explored)

### Changed
- **P2 Refactoring**: SOLID cleanup across 6 phases — broke up god methods, eliminated duplication, extracted helpers
- `LookCommand.Execute()` refactored from monolith into orchestrator + `BuildItemLines()`, `BuildNpcLines()`, `BuildExitLines()` internal static methods
- `StatusCommand.Execute()` refactored to use loops over `StatType` enum and equipment slot tuples instead of repetitive code
- `MoveCommand.ExecuteWithVerb()` refactored with extracted `ResolveDirection()` and `TrackExploration()` helpers
- `AttackCommand.Execute()` refactored with extracted `HandleTargetKilled()` and `AppendPlayerHealth()` helpers
- `GameSessionHost` refactored: save/load logic moved to `SaveDataMapper`, system/command registration extracted to dedicated methods
- Terminal and Game.razor command routing deduplicated via `CommandClassifier` and `Terminal.ExecuteCommand()` public API
- Terminal redesigned with split panels: persistent room description (top) + action/conversation output (bottom)
- StatusCommand now differentiates between worn items (armor/clothing) and wielded items (weapons/shields)
- Left sidebar split into PlayerInfoPanel (top) and MapPanel (bottom)
- Game layout redesigned with CSS grid for responsive 3-column layout
- Exit buttons now navigate directly on click instead of showing context menu
- Exit buttons show preview tooltips on hover with contextual descriptions
- Equipment system simplified from 24 slots with layering to 11 simple slots (one item per slot)
- Character stats simplified from 13 attributes to 8 core attributes
- Stat labels displayed as 3-letter abbreviations (STR, DEX, FOR, AGI, WIL, INT, PER, CHA)
- NPCs now always show their names (removed name discovery mechanic)
- Equipment panel slots reordered: Right and Left wield slots now at top of list
- MapPanel completely redesigned from exit list to visual compass rose

### Removed
- MemorySystem and RoomMemoryComponent (complex memory tracking)
- "You've been here before" messages on room revisits
- Item recollection feature ("This is where you found...")
- NPC name discovery system (showing "someone" until talked to)
- Timestamp tracking for NPC kills and item pickups
- Layering support in equipment system
- 5 stats removed: Vitality, Luck, Memory, Fate, Eldritch, Racial (replaced with Fortitude)
- NarrativeSystem and generic time-of-day descriptions from room output
- Redundant "Equipment" header from equipment panel tab

### Fixed
- Save/load now persists all 8 stats (previously only STR, DEX, INT were saved; FOR, AGI, WIL, PER, CHA were lost on reload)
- Exercise progress dictionary included in save/load round-trips
- Real-time room description updates when game state changes
- Proper separation of Use, Wear, and Wield command behaviors
- Equipment slot references throughout codebase after simplification
