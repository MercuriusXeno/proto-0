# Session Notes
<!-- Written by /wrapup. Read by /catchup at the start of the next session. -->
<!-- Overwritten each session — history preserved in git log of this file. -->

- **Date:** 2026-02-23
- **Branch:** master

## What Was Done
- Implemented interactive buttons (Goal #2): EntityReference, OutputLine, EntityButton components with clickable context menus
- Redesigned equipment system: 24 slots (16 body + 6 relic + WieldLeft/WieldRight), multi-layer support via EquipmentComponent
- Separated commands: UseCommand (consumables only), WearCommand (armor/clothing), WieldCommand (weapons/shields), UnwieldCommand
- Implemented 13 diverse stats in StatsComponent: Strength, Agility, Dexterity, Perception, Intelligence, Willpower, Vitality, Charisma, Luck, Memory, Fate, Eldritch, Racial
- Added NPC disposition system (NpcDisposition class) with name/title separation; NPCs show "someone" until talked to
- Implemented visit-based memory: Items remembered only when taken (not just seen), room visit counts track revisits
- Redesigned UI: Split terminal (persistent room description + clearing action panel), tabbed PlayerInfoPanel (Equipment/Stats), split left sidebar (PlayerInfo top, Map bottom)
- Real-time room updates: Commands can set RefreshRoomDescription flag to trigger immediate room description refresh (e.g., TalkCommand on first NPC meeting)
- Updated all NPCs in npcs.json with actual names (Aldric, Greta, Marcus, Ezra) and disposition data
- Fixed save/load to use WieldRight slot for weapon backwards compatibility

## Decisions Made
- **Wear vs Wield**: Hand slot is for gloves (worn equipment), WieldLeft/WieldRight are separate wielding slots for weapons/shields. User clarified: "Hand slot is for hand equipment like gloves. Weapons are wielded."
- **Rich output backwards compatible**: CommandResult.RichOutput coexists with plain Output; Terminal renders both. Existing commands work unchanged.
- **Visit-based memory**: Items only show memory text on room revisits if previously taken (not just looked at). User feedback: "I want the player to remember that they found a sword in that room."
- **NPC discovery pattern**: NPCs display as "someone" in room descriptions until player uses TalkCommand. Disposition affects introduction (friendly gives name+title, standoffish gives name only).
- **Terminal split design**: Room description persists in top panel, action/conversation results clear in bottom panel on every command. Room description auto-updates when RefreshRoomDescription flag set.
- **Equipment defaults**: Armor defaults to UpperTorso, weapons to WieldRight in current implementation. ItemData doesn't yet specify target slots.

## Open Items
- [ ] ItemData lacks EquipmentSlot field — armor/weapons default to hardcoded slots (UpperTorso, WieldRight)
- [ ] Action-based stat growth not implemented — all 13 stats static at 10
- [ ] Rebirth system for stats not implemented (Goal #4 partially complete)
- [ ] Dynamic equipment slots (items adding slots, e.g., belt adding scabbard) deferred to future
- [ ] Visual map system (Goal #3) not started — MapPanel is placeholder

## Next Steps
1. Add EquipmentSlot/EquipmentSlots field to ItemData for items to specify which slot(s) they equip to
2. Test interactive buttons in browser — verify EntityButton click → context menu → command execution works end-to-end
3. Implement action-based stat growth system (stats increase through player actions, not leveling)

## Context for Next Session
Equipment system redesign is complete and compiles successfully. All commands (Use/Wear/Wield/Unwield) differentiate consumables from worn equipment from wielded weapons. Rich output system is in place but only LookCommand uses it (TalkCommand also uses RichOutput for NPC entity references). Next priority is testing the interactive buttons in the browser to ensure the full click-to-action workflow functions correctly, then extending rich output to more commands (InventoryCommand, StatusCommand) if desired.
