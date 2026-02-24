using ProtoEngine.Components;
using ProtoEngine.Systems;

namespace ProtoEngine.Commands.Commands;

public class TakeCommand : ICommand
{
    private readonly InventorySystem _inventory;
    private readonly WorldSystem _world;
    private readonly MemorySystem _memory;
    private readonly ActionLogSystem _actionLog;

    public string Verb => "take";
    public string[] Aliases => ["get", "grab", "pick"];
    public string Description => "Pick up an item (take <item name>)";

    public TakeCommand(InventorySystem inventory, WorldSystem world, MemorySystem memory, ActionLogSystem actionLog)
    {
        _inventory = inventory;
        _world = world;
        _memory = memory;
        _actionLog = actionLog;
    }

    public CommandResult Execute(CommandContext context, string[] args)
    {
        if (args.Length == 0)
            return CommandResult.Fail("Take what? Specify an item name.");

        var itemName = string.Join(" ", args);
        if (_inventory.TryPickUp(context.State, itemName, out var message))
        {
            var room = _world.GetPlayerRoom(context.State);
            if (room is not null)
            {
                var item = _inventory.FindItemByName(itemName);
                if (item is not null)
                {
                    _memory.RecordItemTaken(context.State, room.Id, item.Id, item.Name);
                    _actionLog.LogItem(context.State, $"Picked up {item.Name}");
                }
            }
            // Refresh room description to remove item from view
            return new CommandResult
            {
                Success = true,
                Output = new List<string> { message },
                RefreshRoomDescription = true
            };
        }
        return CommandResult.Fail(message);
    }
}
