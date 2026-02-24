# Session Notes
<!-- Written by /wrapup. Read by /catchup at the start of the next session. -->
<!-- Overwritten each session — history preserved in git log of this file. -->

- **Date:** 2026-02-23
- **Branch:** master

## What Was Done
- Removed MemorySystem entirely: deleted `MemorySystem.cs`, `RoomMemoryComponent.cs`
- Removed memory features: "You've been here before" messages, item recollection, NPC name discovery
- Added simple room exploration: created `ExploredRoomsComponent.cs` for permanent visited room tracking
- Updated `MoveCommand` to track visited rooms, `LookCommand` to show explored/unexplored exits
- Simplified equipment from 24 slots (with layering) to 11 slots: Head, Body, Arms, Belt, Legs, Feet, WieldLeft, WieldRight, Accessory1-3
- Updated `EquipmentComponent.cs` (removed layers, one item per slot), `EquipmentPanel.razor`, `StatusCommand.cs`, `WieldCommand.cs`, `UnwieldCommand.cs`
- Simplified stats from 13 to 8: STR, DEX, FOR, AGI, WIL, INT, PER, CHA (3-letter abbreviations)
- Updated `StatsComponent.cs`, `StatsPanel.razor`, `StatusCommand.cs` with abbreviated labels
- Added exit preview system: `RoomData.cs` with `ExitPreviews`, `EntityReference.cs` with `TooltipText`
- Updated `EntityButton.razor` for hover tooltips and single-click exit navigation
- Added sample exit previews to `rooms.json` (town_square, dark_alley, forest_path)
- Updated all commands that referenced MemorySystem: `LookCommand`, `TakeCommand`, `MoveCommand`, `TalkCommand`, `AttackCommand`
- Updated `GameSessionHost.cs` to remove MemorySystem registration
- Updated README.md, CLAUDE.md, CHANGELOG.md to reflect all changes and fix stale references

## Decisions Made
- Memory system too complex: Simplified to just permanent room visit tracking (no degradation, no timestamps)
- Equipment slots simplified: Removed regional grouping, layering - just 11 clear slots
- Stats reduced to core 8: Removed Vitality, Luck, Memory, Fate, Eldritch, Racial; added Fortitude
- 3-letter stat abbreviations: More compact display (STR vs "Strength")
- Exit navigation UX: Single-click to move (faster), hover for preview (contextual)
- Exit previews context-aware: Each room defines what you see looking in each direction

## Open Items
- [ ] Remove generic time-of-day descriptions from NarrativeSystem (still present, not yet addressed)
- [ ] Visual map for Goal #3 (exploration tracking done, map visualization pending)
- [ ] Action-based stat growth system (Goal #4 - stats exist, progression mechanics not implemented)
- [ ] Rebirth system (Goal #4 - not implemented)

## Next Steps
1. Remove generic NarrativeSystem time descriptions in favor of room-specific time-variant content
2. Implement visual map panel showing visited rooms and connections (Goal #3)
3. Design and implement action-based stat progression system (Goal #4)
4. Continue with Goal #5 (Comprehensive Skills) or Goal #6 (Intelligent Aliasing)

## Context for Next Session
Major simplification session - removed complex MemorySystem in favor of lightweight room tracking, simplified equipment from 24 to 11 slots, reduced stats from 13 to 8. Added exit preview system with hover tooltips and direct navigation. All documentation updated to match. Core gameplay loop is cleaner and more focused. Ready to tackle visual mapping or stat progression next.
