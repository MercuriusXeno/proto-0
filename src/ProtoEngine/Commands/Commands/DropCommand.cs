using ProtoEngine.Systems;

namespace ProtoEngine.Commands.Commands;

public class DropCommand : ICommand
{
    private readonly InventorySystem _inventory;

    public string Verb => "drop";
    public string[] Aliases => ["discard"];
    public string Description => "Drop an item from your inventory (drop <item name>)";

    public DropCommand(InventorySystem inventory)
    {
        _inventory = inventory;
    }

    public CommandResult Execute(CommandContext context, string[] args)
    {
        if (args.Length == 0)
            return CommandResult.Fail("Drop what? Specify an item name.");

        var itemName = string.Join(" ", args);
        if (_inventory.TryDrop(context.State, itemName, out var message))
            return CommandResult.Ok(message);
        return CommandResult.Fail(message);
    }
}
