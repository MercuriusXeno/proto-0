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

        // Mark room as visited
        _memory.AddRoomVisit(context.State, room.Id);

        var lines = new List<string>();
        lines.Add($"== {room.Name} ==");
        lines.Add(_narrative.GetTimeDescription(context.State.Clock));
        lines.Add("");
        lines.Add(room.Description);

        // Get room memories
        var memories = _memory.GetRoomMemories(context.State, room.Id);

        // Items in room
        var items = _inventory.GetItemsInRoom(context.State, room.Id);
        if (items.Count > 0)
        {
            lines.Add("");
            foreach (var item in items)
            {
                // Check if item was found before
                var itemMemory = memories.FirstOrDefault(m =>
                    m.Type == RoomMemoryType.ItemFound && m.EntityId == item.Id);

                if (itemMemory != null)
                {
                    var timeStr = FormatGameTime(context.State.Clock, itemMemory.GameTick);
                    lines.Add($"You found {item.Name} here {timeStr}");
                }
                else
                {
                    lines.Add($"You see {item.Name} here");
                    // Record seeing it for the first time
                    _memory.RecordItemFound(context.State, room.Id, item.Id, item.Name);
                }
            }
        }

        // NPCs in room (alive and dead)
        var entities = _world.GetEntitiesInRoom(context.State, room.Id);
        var aliveNpcs = entities.Where(e => e.Tag == "npc" && e.Get<HealthComponent>()?.IsAlive == true).ToList();
        var deadNpcs = entities.Where(e => e.Tag == "npc" && e.Get<HealthComponent>()?.IsAlive == false).ToList();

        if (aliveNpcs.Count > 0 || deadNpcs.Count > 0)
        {
            lines.Add("");
        }

        foreach (var npc in aliveNpcs)
        {
            var desc = npc.Get<DescriptionComponent>();
            if (desc is not null)
            {
                var npcMemory = memories.FirstOrDefault(m =>
                    m.Type == RoomMemoryType.NpcMet && m.EntityId == npc.Id);

                if (npcMemory != null)
                {
                    lines.Add($"You see {desc.Name} here");
                }
                else
                {
                    lines.Add($"You see {desc.Name} here");
                    _memory.RecordNpcMet(context.State, room.Id, npc.Id, desc.Name);
                }
            }
        }

        foreach (var npc in deadNpcs)
        {
            var desc = npc.Get<DescriptionComponent>();
            if (desc is not null)
            {
                var killMemory = memories.FirstOrDefault(m =>
                    m.Type == RoomMemoryType.NpcKilled && m.EntityId == npc.Id);

                if (killMemory != null)
                {
                    var timeStr = FormatGameTime(context.State.Clock, killMemory.GameTick);
                    lines.Add($"You see the corpse of {desc.Name} you slew here {timeStr}");
                }
                else
                {
                    lines.Add($"You see the corpse of {desc.Name} here");
                }
            }
        }

        // Exits (with explored/unexplored distinction)
        if (room.Exits.Count > 0)
        {
            lines.Add("");
            var explored = new List<string>();
            var unexplored = new List<string>();

            foreach (var exit in room.Exits)
            {
                if (_memory.HasExploredExit(context.State, room.Id, exit.Key))
                    explored.Add(exit.Key);
                else
                    unexplored.Add(exit.Key);
            }

            if (explored.Count > 0)
                lines.Add($"Explored exits: {string.Join(", ", explored)}");
            if (unexplored.Count > 0)
                lines.Add($"Unexplored exits: {string.Join(", ", unexplored)}");
        }

        return CommandResult.Ok(lines.ToArray());
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
}
