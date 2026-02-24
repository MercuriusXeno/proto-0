using ProtoEngine.Components;
using ProtoEngine.Systems;

namespace ProtoEngine.Commands.Commands;

public class LookCommand : ICommand
{
    private readonly WorldSystem _world;
    private readonly InventorySystem _inventory;
    private readonly NarrativeSystem _narrative;

    public string Verb => "look";
    public string[] Aliases => ["l", "examine"];
    public string Description => "Look around the current room or examine something";

    public LookCommand(WorldSystem world, InventorySystem inventory, NarrativeSystem narrative)
    {
        _world = world;
        _inventory = inventory;
        _narrative = narrative;
    }

    public CommandResult Execute(CommandContext context, string[] args)
    {
        var room = _world.GetPlayerRoom(context.State);
        if (room is null)
            return CommandResult.Fail("You are nowhere.");

        var lines = new List<OutputLine>();
        lines.Add(OutputLine.Plain($"== {room.Name} =="));
        lines.Add(OutputLine.Plain(_narrative.GetTimeDescription(context.State.Clock)));
        lines.Add(OutputLine.Plain(""));
        lines.Add(OutputLine.Plain(room.Description));

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

                var article = GetArticle(item.Name);
                lines.Add(OutputLine.WithEntities($"You see {article}{item.Name} here", itemRef));
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
                var npcComp = npc.Get<NpcComponent>();
                var displayText = desc.Name;

                // Add title if they have one
                if (!string.IsNullOrEmpty(npcComp?.Title))
                {
                    displayText = $"{desc.Name}, the {npcComp.Title},";
                }
                else
                {
                    displayText = $"{desc.Name},";
                }

                var npcRef = new EntityReference(
                    npc.Id,
                    desc.Name,
                    EntityType.Npc,
                    new() { EntityAction.Talk, EntityAction.Attack, EntityAction.Examine }
                );

                lines.Add(OutputLine.WithEntities($"You see {displayText} here", npcRef));
            }
        }

        foreach (var npc in deadNpcs)
        {
            var desc = npc.Get<DescriptionComponent>();
            if (desc is not null)
            {
                var npcComp = npc.Get<NpcComponent>();
                var displayText = desc.Name;

                // Add title if they have one
                if (!string.IsNullOrEmpty(npcComp?.Title))
                {
                    displayText = $"{desc.Name}, the {npcComp.Title},";
                }
                else
                {
                    displayText = $"{desc.Name},";
                }

                var npcRef = new EntityReference(
                    npc.Id,
                    desc.Name,
                    EntityType.Npc,
                    new() { EntityAction.Examine }
                );

                lines.Add(OutputLine.WithEntities($"You see the corpse of {displayText} here", npcRef));
            }
        }

        // Exits (with explored/unexplored)
        if (room.Exits.Count > 0)
        {
            lines.Add(OutputLine.Plain(""));
            var explored = context.State.Player.Get<ExploredRoomsComponent>();
            var exploredExits = new List<EntityReference>();
            var unexploredExits = new List<EntityReference>();

            foreach (var exit in room.Exits)
            {
                // Get preview text for this exit direction if available
                var previewText = room.ExitPreviews?.ContainsKey(exit.Key) == true
                    ? room.ExitPreviews[exit.Key]
                    : null;

                var exitRef = new EntityReference(
                    exit.Key,  // Direction as ID
                    exit.Key,
                    EntityType.Exit,
                    new() { EntityAction.Move },
                    previewText
                );

                // Check if we've been to the room this exit leads to
                if (explored?.VisitedRoomIds.Contains(exit.Value) == true)
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

    private string GetArticle(string word)
    {
        if (string.IsNullOrEmpty(word)) return "";
        var first = char.ToLower(word[0]);
        var vowels = new[] { 'a', 'e', 'i', 'o', 'u' };
        return vowels.Contains(first) ? "an " : "a ";
    }
}
