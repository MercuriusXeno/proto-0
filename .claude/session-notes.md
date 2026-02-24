# Session Notes
<!-- Written by /wrapup. Read by /catchup at the start of the next session. -->
<!-- Overwritten each session — history preserved in git log of this file. -->

- **Date:** 2026-02-24
- **Branch:** master

## What Was Done
- Added unit tests for all P2 extracted internal static methods (81 new tests, 103 → 184 engine tests)
  - `CommandClassifierTests` (11 tests): IsLookCommand/IsNavigationCommand — verbs, aliases, case insensitivity, args
  - `LookCommandTests` (18 tests): BuildItemLines, BuildNpcLines, BuildExitLines, GetArticle — empty/single/multiple items, alive/dead NPCs, explored/unexplored exits, previews
  - `MoveCommandTests` (10 tests): ResolveDirection — full directions, abbreviations, verb fallback, null cases, override priority, case insensitivity
  - `StatusCommandTests` (3 tests): ToAbbreviation — all 8 stat types, 3-char length, uppercase
- Updated docs (README, CLAUDE.md, CHANGELOG, MEMORY.md) to reflect new test counts and test file listings

## Decisions Made
- Tests exercise the extracted `internal static` methods directly (not through full command execution) — tests the seam, not the integration
- Used `[Theory]`/`[InlineData]` for parameterized tests where multiple inputs share the same assertion pattern
- NPC test helper (`CreateNpc`) creates minimal entities with only the components needed by `BuildNpcLines`

## Open Items
- [ ] Double-refresh bug: `Game.razor` `HandleAction` calls `ExecuteCommand` then `RefreshPanels` — RefreshPanels also fires via `OnCommandExecuted` callback, so panels refresh twice per action bar/map click
- [ ] No tests for `AttackCommand.HandleTargetKilled`/`AppendPlayerHealth` (private, not internal — would need refactoring to test)

## Next Steps
1. P3: Map Polish — fix colors, borders, click-to-navigate on compass rose
2. P4: Equipment Slot UI — interactive context buttons for equipped items
3. P5: Time System Foundation

## Context for Next Session
All P2 extracted internal methods now have dedicated tests. The open item about missing P2 tests from last session is resolved. The only untested P2 extractions are `AttackCommand.HandleTargetKilled()` and `AppendPlayerHealth()` which are `private` (not `internal`) — testing them would require changing visibility. Total: 192 tests (184 engine + 8 UI), all passing.
