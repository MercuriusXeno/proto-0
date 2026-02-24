# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/).

## [Unreleased]

### Added
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

### Changed
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

### Removed
- MemorySystem and RoomMemoryComponent (complex memory tracking)
- "You've been here before" messages on room revisits
- Item recollection feature ("This is where you found...")
- NPC name discovery system (showing "someone" until talked to)
- Timestamp tracking for NPC kills and item pickups
- Layering support in equipment system
- 5 stats removed: Vitality, Luck, Memory, Fate, Eldritch, Racial (replaced with Fortitude)

### Fixed
- Real-time room description updates when game state changes
- Proper separation of Use, Wear, and Wield command behaviors
- Equipment slot references throughout codebase after simplification
