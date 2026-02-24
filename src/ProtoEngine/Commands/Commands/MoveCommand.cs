using ProtoEngine.Components;
using ProtoEngine.Systems;

namespace ProtoEngine.Commands.Commands;

public class MoveCommand : ICommand
{
    private static readonly HashSet<string> DirectionWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "north", "south", "east", "west", "up", "down", "n", "s", "e", "w", "u", "d"
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
        string direction;

        if (args.Length > 0)
        {
            direction = args[0].ToLowerInvariant();
        }
        else if (originalVerb is not null && DirectionWords.Contains(originalVerb))
        {
            // User typed a direction directly (e.g., "north")
            direction = originalVerb.ToLowerInvariant();
        }
        else
        {
            return CommandResult.Fail("Go where? Specify a direction (north, south, east, west, up, down).");
        }

        // Expand abbreviations
        direction = direction switch
        {
            "n" => "north",
            "s" => "south",
            "e" => "east",
            "w" => "west",
            "u" => "up",
            "d" => "down",
            _ => direction
        };

        var currentRoom = _world.GetPlayerRoom(context.State);
        if (currentRoom is null)
            return CommandResult.Fail("You are nowhere.");

        if (_world.TryMove(context.State, direction, out var newRoomId))
        {
            // Track visited room
            var explored = context.State.Player.Get<ExploredRoomsComponent>();
            if (explored == null)
            {
                explored = new ExploredRoomsComponent();
                context.State.Player.Add(explored);
            }
            explored.VisitedRoomIds.Add(newRoomId!);

            // Log the movement
            _actionLog.LogMovement(context.State, currentRoom.Name, direction, newRoomId!);

            return CommandResult.Ok($"You head {direction}.");
        }

        return CommandResult.Fail($"You can't go {direction} from here.");
    }
}
