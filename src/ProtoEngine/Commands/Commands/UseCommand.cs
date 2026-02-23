using ProtoEngine.Components;
using ProtoEngine.Events;
using ProtoEngine.Systems;

namespace ProtoEngine.Commands.Commands;

public class UseCommand : ICommand
{
    private readonly InventorySystem _inventory;

    public string Verb => "use";
    public string[] Aliases => ["drink", "eat", "consume"];
    public string Description => "Use an item from your inventory (use <item name>)";

    public UseCommand(InventorySystem inventory)
    {
        _inventory = inventory;
    }

    public CommandResult Execute(CommandContext context, string[] args)
    {
        if (args.Length == 0)
            return CommandResult.Fail("Use what? Specify an item name.");

        var itemName = string.Join(" ", args);
        var items = _inventory.GetInventoryItems(context.State);
        var item = items.FirstOrDefault(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));

        if (item is null)
            return CommandResult.Fail($"You don't have a '{itemName}'.");

        if (item.Type == "consumable" && item.HealAmount > 0)
        {
            var health = context.Player.Get<HealthComponent>();
            if (health is not null)
            {
                var healed = Math.Min(item.HealAmount, health.Max - health.Current);
                health.Current = Math.Min(health.Max, health.Current + item.HealAmount);
                var inv = context.Player.Get<InventoryComponent>();
                inv?.ItemIds.Remove(item.Id);
                context.EventBus.Publish(new ItemUsedEvent(context.Player.Id, item.Id));
                return CommandResult.Ok($"You use the {item.Name} and recover {healed} HP. ({health.Current}/{health.Max})");
            }
        }

        if (item.Type == "weapon")
        {
            var equip = context.Player.Get<EquipmentComponent>();
            var combat = context.Player.Get<CombatComponent>();
            if (equip is not null && combat is not null)
            {
                equip.WeaponId = item.Id;
                combat.AttackPower = 5 + item.AttackBonus;
                return CommandResult.Ok($"You equip the {item.Name}. Attack power: {combat.AttackPower}");
            }
        }

        if (item.Type == "armor")
        {
            var equip = context.Player.Get<EquipmentComponent>();
            var combat = context.Player.Get<CombatComponent>();
            if (equip is not null && combat is not null)
            {
                equip.ArmorId = item.Id;
                combat.Defense = 2 + item.DefenseBonus;
                return CommandResult.Ok($"You equip the {item.Name}. Defense: {combat.Defense}");
            }
        }

        return CommandResult.Fail($"You can't figure out how to use the {item.Name}.");
    }
}
