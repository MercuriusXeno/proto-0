using ProtoEngine.Components;
using ProtoEngine.Systems;

namespace ProtoEngine.Commands.Commands;

public class WieldCommand : ICommand
{
    private readonly InventorySystem _inventory;

    public string Verb => "wield";
    public string[] Aliases => ["draw", "ready"];
    public string Description => "Wield a weapon or shield (wield <item> [left|right])";

    public WieldCommand(InventorySystem inventory)
    {
        _inventory = inventory;
    }

    public CommandResult Execute(CommandContext context, string[] args)
    {
        if (args.Length == 0)
            return CommandResult.Fail("Wield what? Specify an item name.");

        // Parse item name and optional hand specifier
        var lastArg = args[^1].ToLowerInvariant();
        var handSpecified = lastArg is "left" or "right";

        var itemName = handSpecified
            ? string.Join(" ", args[..^1])
            : string.Join(" ", args);

        var hand = handSpecified ? lastArg : "right"; // Default to right hand

        var items = _inventory.GetInventoryItems(context.State);
        var item = items.FirstOrDefault(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));

        if (item is null)
            return CommandResult.Fail($"You don't have a '{itemName}'.");

        if (item.Type != "weapon")
            return CommandResult.Fail($"You can't wield the {item.Name}.");

        var equip = context.Player.Get<EquipmentComponent>();
        var combat = context.Player.Get<CombatComponent>();

        if (equip is null || combat is null)
            return CommandResult.Fail("You can't wield items right now.");

        var slot = hand == "left" ? EquipmentSlot.WieldLeft : EquipmentSlot.WieldRight;
        var handName = hand == "left" ? "left hand" : "right hand";

        // Check if something is already wielded
        var currentlyWielded = equip.GetSlotItem(slot);
        if (currentlyWielded != null)
            return CommandResult.Fail($"You're already wielding {currentlyWielded.ItemName} in your {handName}. Unwield it first.");

        equip.EquipItem(slot, item.Id, item.Name);
        combat.AttackPower = 5 + item.AttackBonus;

        return CommandResult.Ok($"You wield the {item.Name} in your {handName}. Attack power: {combat.AttackPower}");
    }
}
