using ProtoEngine.Components;
using ProtoEngine.Core;
using ProtoEngine.Data;
using ProtoEngine.Systems;

namespace ProtoEngine.Commands.Commands;

public class LookCommand : ICommand
{
    private readonly WorldSystem _world;
    private readonly InventorySystem _inventory;

    public string Verb => "look";
    public string[] Aliases => ["l", "examine"];
    public string Description => "Look around the current room or examine something";

    public LookCommand(WorldSystem world, InventorySystem inventory)
    {
        _world = world;
        _inventory = inventory;
    }

    public CommandResult Execute(CommandContext context, string[] args)
    {
        var room = _world.GetPlayerRoom(context.State);
        if (room is null)
            return CommandResult.Fail("You are nowhere.");

        var lines = new List<OutputLine>();
        lines.Add(OutputLine.Plain($"== {room.Name} =="));
        lines.Add(OutputLine.Plain(""));
        lines.Add(OutputLine.Plain(room.Description));

        var items = _inventory.GetItemsInRoom(context.State, room.Id);
        lines.AddRange(BuildItemLines(items));

        var entities = _world.GetEntitiesInRoom(context.State, room.Id);
        var aliveNpcs = entities.Where(e => e.Tag == "npc" && e.Get<HealthComponent>()?.IsAlive == true).ToList();
        var deadNpcs = entities.Where(e => e.Tag == "npc" && e.Get<HealthComponent>()?.IsAlive == false).ToList();
        lines.AddRange(BuildNpcLines(aliveNpcs, deadNpcs));

        var explored = context.State.Player.Get<ExploredRoomsComponent>();
        lines.AddRange(BuildExitLines(room.Exits, room.ExitPreviews, explored));

        return CommandResult.OkRich(lines.ToArray());
    }

    /// <summary>
    /// Builds output lines for items visible in the current room.
    /// </summary>
    internal static List<OutputLine> BuildItemLines(List<ItemData> items)
    {
        var lines = new List<OutputLine>();
        if (items.Count == 0)
            return lines;

        lines.Add(OutputLine.Plain(""));

        foreach (var item in items)
        {
            var itemRef = new EntityReference(
                item.Id, item.Name, EntityType.Item,
                new() { EntityAction.Take, EntityAction.Examine }
            );
            var article = GetArticle(item.Name);
            lines.Add(OutputLine.WithEntities($"You see {article}{item.Name} here", itemRef));
        }

        return lines;
    }

    /// <summary>
    /// Builds output lines for alive and dead NPCs in the current room.
    /// </summary>
    internal static List<OutputLine> BuildNpcLines(List<Entity> aliveNpcs, List<Entity> deadNpcs)
    {
        var lines = new List<OutputLine>();
        if (aliveNpcs.Count == 0 && deadNpcs.Count == 0)
            return lines;

        lines.Add(OutputLine.Plain(""));

        foreach (var npc in aliveNpcs)
            AddNpcLine(lines, npc, isAlive: true);

        foreach (var npc in deadNpcs)
            AddNpcLine(lines, npc, isAlive: false);

        return lines;
    }

    /// <summary>
    /// Builds output lines for room exits, split into explored and unexplored groups.
    /// </summary>
    internal static List<OutputLine> BuildExitLines(
        Dictionary<string, string> exits,
        Dictionary<string, string>? exitPreviews,
        ExploredRoomsComponent? explored)
    {
        var lines = new List<OutputLine>();
        if (exits.Count == 0)
            return lines;

        lines.Add(OutputLine.Plain(""));
        var exploredExits = new List<EntityReference>();
        var unexploredExits = new List<EntityReference>();

        foreach (var exit in exits)
        {
            var previewText = exitPreviews?.ContainsKey(exit.Key) == true
                ? exitPreviews[exit.Key]
                : null;

            var exitRef = new EntityReference(
                exit.Key,
                exit.Key,
                EntityType.Exit,
                new() { EntityAction.Move },
                previewText
            );

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

        return lines;
    }

    private static void AddNpcLine(List<OutputLine> lines, Entity npc, bool isAlive)
    {
        var desc = npc.Get<DescriptionComponent>();
        if (desc is null)
            return;

        var npcComp = npc.Get<NpcComponent>();
        var displayText = !string.IsNullOrEmpty(npcComp?.Title)
            ? $"{desc.Name}, the {npcComp.Title},"
            : $"{desc.Name},";

        var actions = isAlive
            ? new List<EntityAction> { EntityAction.Talk, EntityAction.Attack, EntityAction.Examine }
            : new List<EntityAction> { EntityAction.Examine };

        var npcRef = new EntityReference(npc.Id, desc.Name, EntityType.Npc, actions);

        var prefix = isAlive ? "You see" : "You see the corpse of";
        lines.Add(OutputLine.WithEntities($"{prefix} {displayText} here", npcRef));
    }

    internal static string GetArticle(string word)
    {
        if (string.IsNullOrEmpty(word)) return "";
        var first = char.ToLower(word[0]);
        var vowels = new[] { 'a', 'e', 'i', 'o', 'u' };
        return vowels.Contains(first) ? "an " : "a ";
    }
}
