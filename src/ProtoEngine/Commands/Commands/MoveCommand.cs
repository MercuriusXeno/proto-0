using ProtoEngine.Components;
using ProtoEngine.Systems;

namespace ProtoEngine.Commands.Commands;

public class MoveCommand : ICommand
{
    private static readonly HashSet<string> DirectionWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "north", "south", "east", "west", "up", "down", "n", "s", "e", "w", "u", "d"
    };

    private static readonly Dictionary<string, string> Abbreviations = new(StringComparer.OrdinalIgnoreCase)
    {
        ["n"] = "north", ["s"] = "south", ["e"] = "east",
        ["w"] = "west", ["u"] = "up", ["d"] = "down"
    };

    private readonly WorldSystem _world;
    private readonly ActionLogSystem _actionLog;

    public string Verb => "go";
    public string[] Aliases => ["move", "walk", "north", "south", "east", "west", "up", "down", "n", "s", "e", "w", "u", "d"];
    public string Description => "Move in a direction (go north, or just 'north')";

    public MoveCommand(WorldSystem world, ActionLogSystem actionLog)
    {
        _world = world;
        _actionLog = actionLog;
    }

    public CommandResult Execute(CommandContext context, string[] args)
    {
        return ExecuteWithVerb(context, args, null);
    }

    public CommandResult ExecuteWithVerb(CommandContext context, string[] args, string? originalVerb)
    {
        var direction = ResolveDirection(args, originalVerb);
        if (direction is null)
            return CommandResult.Fail("Go where? Specify a direction (north, south, east, west, up, down).");

        var currentRoom = _world.GetPlayerRoom(context.State);
        if (currentRoom is null)
            return CommandResult.Fail("You are nowhere.");

        if (!_world.TryMove(context.State, direction, out var newRoomId))
            return CommandResult.Fail($"You can't go {direction} from here.");

        TrackExploration(context, newRoomId!);
        _actionLog.LogMovement(context.State, currentRoom.Name, direction, newRoomId!);

        return CommandResult.Ok($"You head {direction}.");
    }

    /// <summary>
    /// Parses direction from command arguments or the original verb, expanding abbreviations.
    /// Returns null if no valid direction could be determined.
    /// </summary>
    internal static string? ResolveDirection(string[] args, string? originalVerb)
    {
        string raw;
        if (args.Length > 0)
            raw = args[0];
        else if (originalVerb is not null && DirectionWords.Contains(originalVerb))
            raw = originalVerb;
        else
            return null;

        var lower = raw.ToLowerInvariant();
        return Abbreviations.TryGetValue(lower, out var expanded) ? expanded : lower;
    }

    private static void TrackExploration(CommandContext context, string newRoomId)
    {
        var explored = context.State.Player.Get<ExploredRoomsComponent>();
        if (explored == null)
        {
            explored = new ExploredRoomsComponent();
            context.State.Player.Add(explored);
        }
        explored.VisitedRoomIds.Add(newRoomId);
    }
}
