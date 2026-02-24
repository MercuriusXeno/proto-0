using ProtoEngine.Components;
using ProtoEngine.Systems;

namespace ProtoEngine.Commands.Commands;

public class LookCommand : ICommand
{
    private readonly WorldSystem _world;
    private readonly InventorySystem _inventory;
    private readonly NarrativeSystem _narrative;
    private readonly MemorySystem _memory;

    public string Verb => "look";
    public string[] Aliases => ["l", "examine"];
    public string Description => "Look around the current room or examine something";

    public LookCommand(WorldSystem world, InventorySystem inventory, NarrativeSystem narrative, MemorySystem memory)
    {
        _world = world;
        _inventory = inventory;
        _narrative = narrative;
        _memory = memory;
    }

    public CommandResult Execute(CommandContext context, string[] args)
    {
        var room = _world.GetPlayerRoom(context.State);
        if (room is null)
            return CommandResult.Fail("You are nowhere.");

        // Check if we've been here before (currentVisit is pre-increment, so >= 1 means we've been here)
        var currentVisit = _memory.GetCurrentVisitNumber(context.State, room.Id);
        var isRevisit = currentVisit >= 1;

        // Mark room as visited
        _memory.AddRoomVisit(context.State, room.Id);

        var lines = new List<OutputLine>();
        lines.Add(OutputLine.Plain($"== {room.Name} =="));
        lines.Add(OutputLine.Plain(_narrative.GetTimeDescription(context.State.Clock)));

        if (isRevisit)
        {
            lines.Add(OutputLine.Plain("You've been here before."));
        }

        lines.Add(OutputLine.Plain(""));
        lines.Add(OutputLine.Plain(room.Description));

        // Get room memories
        var memories = _memory.GetRoomMemories(context.State, room.Id);

        // Items in room
        var items = _inventory.GetItemsInRoom(context.State, room.Id);
        if (items.Count > 0)
        {
            lines.Add(OutputLine.Plain(""));

            foreach (var item in items)
            {
                var itemRef = new EntityReference(
                    item.Id,
                    item.Name,
                    EntityType.Item,
                    new() { EntityAction.Take, EntityAction.Examine }
                );

                // Check if we have a memory of TAKING this item (actually picking it up)
                var itemTakenMemory = memories.FirstOrDefault(m =>
                    m.Type == RoomMemoryType.ItemTaken && m.EntityId == item.Id);

                if (itemTakenMemory != null && currentVisit > itemTakenMemory.RoomVisitNumber)
                {
                    // We've taken this item before and are revisiting
                    var timeStr = FormatGameTime(context.State.Clock, itemTakenMemory.GameTick);
                    var article = GetArticle(item.Name);

                    // Check if player currently has this item in inventory
                    var playerInventory = _inventory.GetInventoryItems(context.State);
                    var hasItem = playerInventory.Any(i => i.Id == item.Id);

                    if (hasItem)
                    {
                        lines.Add(OutputLine.WithEntities($"You picked up {article}{item.Name} here {timeStr}, which you still have", itemRef));
                    }
                    else
                    {
                        lines.Add(OutputLine.WithEntities($"You picked up {article}{item.Name} here {timeStr}", itemRef));
                    }
                }
                else
                {
                    // Haven't taken this item yet, or still on same visit
                    var article = GetArticle(item.Name);
                    lines.Add(OutputLine.WithEntities($"You see {article}{item.Name} here", itemRef));
                }
            }
        }

        // NPCs in room (alive and dead)
        var entities = _world.GetEntitiesInRoom(context.State, room.Id);
        var aliveNpcs = entities.Where(e => e.Tag == "npc" && e.Get<HealthComponent>()?.IsAlive == true).ToList();
        var deadNpcs = entities.Where(e => e.Tag == "npc" && e.Get<HealthComponent>()?.IsAlive == false).ToList();

        if (aliveNpcs.Count > 0 || deadNpcs.Count > 0)
        {
            lines.Add(OutputLine.Plain(""));
        }

        foreach (var npc in aliveNpcs)
        {
            var desc = npc.Get<DescriptionComponent>();
            if (desc is not null)
            {
                // Check if we've met this NPC (talked to them)
                var npcMemory = memories.FirstOrDefault(m =>
                    m.Type == RoomMemoryType.NpcMet && m.EntityId == npc.Id);

                if (npcMemory != null)
                {
                    // We know their name - show "Name the Title"
                    var npcComp = npc.Get<NpcComponent>();
                    var displayText = desc.Name;

                    // Add title if they have one and aren't a creature
                    if (!string.IsNullOrEmpty(npcComp?.Title))
                    {
                        displayText = $"{desc.Name} the {npcComp.Title}";
                    }

                    var npcRef = new EntityReference(
                        npc.Id,
                        desc.Name,
                        EntityType.Npc,
                        new() { EntityAction.Talk, EntityAction.Attack, EntityAction.Examine }
                    );

                    lines.Add(OutputLine.WithEntities($"You see {displayText} here", npcRef));
                }
                else
                {
                    // Haven't talked to them yet - just show "someone"
                    var npcRef = new EntityReference(
                        npc.Id,
                        "someone",
                        EntityType.Npc,
                        new() { EntityAction.Talk, EntityAction.Attack, EntityAction.Examine }
                    );

                    lines.Add(OutputLine.WithEntities($"You see someone here", npcRef));
                }
            }
        }

        foreach (var npc in deadNpcs)
        {
            var desc = npc.Get<DescriptionComponent>();
            if (desc is not null)
            {
                // Check if we knew them when they were alive
                var npcMemory = memories.FirstOrDefault(m =>
                    m.Type == RoomMemoryType.NpcMet && m.EntityId == npc.Id);

                var npcComp = npc.Get<NpcComponent>();
                string displayText;
                string npcRefName;

                if (npcMemory != null)
                {
                    // We knew their name - show "Name the Title"
                    npcRefName = desc.Name;
                    displayText = desc.Name;

                    // Add title if they have one and aren't a creature
                    if (!string.IsNullOrEmpty(npcComp?.Title))
                    {
                        displayText = $"{desc.Name} the {npcComp.Title}";
                    }
                }
                else
                {
                    // We didn't know their name - just show "someone"
                    npcRefName = "someone";
                    displayText = "someone";
                }

                var npcRef = new EntityReference(
                    npc.Id,
                    npcRefName,
                    EntityType.Npc,
                    new() { EntityAction.Examine }
                );

                var killMemory = memories.FirstOrDefault(m =>
                    m.Type == RoomMemoryType.NpcKilled && m.EntityId == npc.Id);

                if (killMemory != null && currentVisit > killMemory.RoomVisitNumber)
                {
                    var timeStr = FormatGameTime(context.State.Clock, killMemory.GameTick);
                    lines.Add(OutputLine.WithEntities($"You see the corpse of {displayText} you slew here {timeStr}", npcRef));
                }
                else
                {
                    lines.Add(OutputLine.WithEntities($"You see the corpse of {displayText} here", npcRef));
                }
            }
        }

        // Exits (with explored/unexplored distinction)
        if (room.Exits.Count > 0)
        {
            lines.Add(OutputLine.Plain(""));
            var exploredExits = new List<EntityReference>();
            var unexploredExits = new List<EntityReference>();

            foreach (var exit in room.Exits)
            {
                var exitRef = new EntityReference(
                    exit.Key,  // Direction as ID
                    exit.Key,
                    EntityType.Exit,
                    new() { EntityAction.Move }
                );

                if (_memory.HasExploredExit(context.State, room.Id, exit.Key))
                    exploredExits.Add(exitRef);
                else
                    unexploredExits.Add(exitRef);
            }

            if (exploredExits.Count > 0)
            {
                var exploredText = $"Explored exits: {string.Join(", ", exploredExits.Select(e => e.DisplayName))}";
                lines.Add(OutputLine.WithEntities(exploredText, exploredExits.ToArray()));
            }
            if (unexploredExits.Count > 0)
            {
                var unexploredText = $"Unexplored exits: {string.Join(", ", unexploredExits.Select(e => e.DisplayName))}";
                lines.Add(OutputLine.WithEntities(unexploredText, unexploredExits.ToArray()));
            }
        }

        return CommandResult.OkRich(lines.ToArray());
    }

    private string FormatGameTime(Core.GameClock clock, int tick)
    {
        var day = (tick / 24) + 1;
        var hour = tick % 24;
        var timeOfDay = hour switch
        {
            >= 5 and < 8 => "dawn",
            >= 8 and < 12 => "morning",
            >= 12 and < 14 => "midday",
            >= 14 and < 18 => "afternoon",
            >= 18 and < 21 => "evening",
            _ => "night"
        };
        return $"on day {day} at {timeOfDay}";
    }

    private string GetArticle(string word)
    {
        if (string.IsNullOrEmpty(word)) return "";
        var first = char.ToLower(word[0]);
        var vowels = new[] { 'a', 'e', 'i', 'o', 'u' };
        return vowels.Contains(first) ? "an " : "a ";
    }
}
