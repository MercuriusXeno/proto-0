using ProtoEngine.Components;
using ProtoEngine.Systems;

namespace ProtoEngine.Commands.Commands;

public class WearCommand : ICommand
{
    private readonly InventorySystem _inventory;

    public string Verb => "wear";
    public string[] Aliases => ["equip"];
    public string Description => "Wear armor or clothing (wear <item name>)";

    public WearCommand(InventorySystem inventory)
    {
        _inventory = inventory;
    }

    public CommandResult Execute(CommandContext context, string[] args)
    {
        if (args.Length == 0)
            return CommandResult.Fail("Wear what? Specify an item name.");

        var itemName = string.Join(" ", args);
        var items = _inventory.GetInventoryItems(context.State);
        var item = items.FirstOrDefault(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));

        if (item is null)
            return CommandResult.Fail($"You don't have a '{itemName}'.");

        if (item.Type != "armor")
            return CommandResult.Fail($"You can't wear the {item.Name}.");

        var equip = context.Player.Get<EquipmentComponent>();
        var combat = context.Player.Get<CombatComponent>();

        if (equip is null || combat is null)
            return CommandResult.Fail("You can't equip items right now.");

        // For now, default armor to UpperTorso
        // TODO: Add slot information to ItemData for more precise equipment
        equip.EquipItem(EquipmentSlot.UpperTorso, item.Id, item.Name);
        combat.Defense = 2 + item.DefenseBonus;

        return CommandResult.Ok($"You wear the {item.Name}. Defense: {combat.Defense}");
    }
}
