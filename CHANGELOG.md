# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/).

## [Unreleased]

### Added
- Text-based dungeon RPG built with C# .NET 9.0 and Blazor WebAssembly
- Entity-Component-System (ECS) architecture for game logic
- Command-driven gameplay with natural language commands (look, move, take, attack, talk, etc.)
- Interactive buttons with context menus for items, NPCs, and exits
- Rich output system with entity references and metadata
- Memory system tracking visited rooms, items taken, and NPC encounters with visit-based recollection
- Action log system recording player actions in rolling history (max 100 entries)
- Equipment system with 24 slots: 16 body slots + 6 relic slots + 2 wielding slots (left/right hand)
- 13 diverse character stats: Strength, Agility, Dexterity, Perception, Intelligence, Willpower, Vitality, Charisma, Luck, Memory, Fate, Eldritch, Racial
- NPC disposition system (friendly, standoffish, hostile) affecting interactions
- Inventory, crafting, combat, and quest systems
- JSON-based game content (rooms, items, NPCs, quests, recipes, dialogue)
- Local storage save/load functionality
- WearCommand for armor and clothing
- WieldCommand for weapons and shields with left/right hand specification
- UnwieldCommand for removing wielded items
- EntityButton component for clickable game elements
- PlayerInfoPanel with tabbed Equipment/Stats interface
- EquipmentPanel displaying all equipped items by body region
- StatsPanel showing all 13 character attributes
- TimeWidget displaying current time of day (Dawn/Morning/Midday/Afternoon/Evening/Night) and day number
- Session notes and development documentation (CLAUDE.md, README.md)
- Comprehensive development guide for contributors

### Changed
- Terminal redesigned with split panels: persistent room description (top) + action/conversation output (bottom)
- NPC system now shows "someone" until player talks to them; disposition affects introduction
- StatusCommand now differentiates between worn items (armor/clothing) and wielded items (weapons/shields)
- Left sidebar split into PlayerInfoPanel (top) and MapPanel (bottom)
- Game layout redesigned with CSS grid for responsive 3-column layout
- Memory system now only remembers items when picked up (not just seen)
- Room descriptions show contextual memories with timestamps on revisits

### Fixed
- Real-time room description updates when game state changes (e.g., NPC introductions)
- Proper separation of Use, Wear, and Wield command behaviors
