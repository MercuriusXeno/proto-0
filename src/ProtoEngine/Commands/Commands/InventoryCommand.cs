using ProtoEngine.Components;
using ProtoEngine.Systems;

namespace ProtoEngine.Commands.Commands;

public class InventoryCommand : ICommand
{
    private readonly InventorySystem _inventory;

    public string Verb => "inventory";
    public string[] Aliases => ["inv", "i"];
    public string Description => "View your inventory";

    public InventoryCommand(InventorySystem inventory)
    {
        _inventory = inventory;
    }

    public CommandResult Execute(CommandContext context, string[] args)
    {
        var items = _inventory.GetInventoryItems(context.State);
        var inv = context.Player.Get<InventoryComponent>();

        var lines = new List<string>();
        lines.Add("== Inventory ==");

        if (items.Count == 0)
        {
            lines.Add("Your inventory is empty.");
        }
        else
        {
            foreach (var item in items)
                lines.Add($"  - {item.Name}: {item.Description}");
        }

        if (inv is not null)
            lines.Add($"  ({items.Count}/{inv.Capacity} slots)");

        return CommandResult.Ok(lines.ToArray());
    }
}
