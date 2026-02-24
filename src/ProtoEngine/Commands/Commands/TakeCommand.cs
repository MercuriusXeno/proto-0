using ProtoEngine.Systems;

namespace ProtoEngine.Commands.Commands;

public class TakeCommand : ICommand
{
    private readonly InventorySystem _inventory;
    private readonly ActionLogSystem _actionLog;

    public string Verb => "take";
    public string[] Aliases => ["get", "grab", "pick"];
    public string Description => "Pick up an item (take <item name>)";

    public TakeCommand(InventorySystem inventory, ActionLogSystem actionLog)
    {
        _inventory = inventory;
        _actionLog = actionLog;
    }

    public CommandResult Execute(CommandContext context, string[] args)
    {
        if (args.Length == 0)
            return CommandResult.Fail("Take what? Specify an item name.");

        var itemName = string.Join(" ", args);
        if (_inventory.TryPickUp(context.State, itemName, out var message))
        {
            var item = _inventory.FindItemByName(itemName);
            if (item is not null)
            {
                _actionLog.LogItem(context.State, $"Picked up {item.Name}");
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
