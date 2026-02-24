using ProtoEngine.Components;

namespace ProtoEngine.Commands.Commands;

public class UnwieldCommand : ICommand
{
    public string Verb => "unwield";
    public string[] Aliases => ["sheathe", "lower"];
    public string Description => "Stop wielding an item (unwield [left|right])";

    public CommandResult Execute(CommandContext context, string[] args)
    {
        var hand = args.Length > 0 ? args[0].ToLowerInvariant() : "right";

        if (hand is not "left" and not "right")
            return CommandResult.Fail("Specify which hand: 'left' or 'right'.");

        var equip = context.Player.Get<EquipmentComponent>();
        if (equip is null)
            return CommandResult.Fail("You don't have an equipment component.");

        var slot = hand == "left" ? EquipmentSlot.WieldLeft : EquipmentSlot.WieldRight;
        var handName = hand == "left" ? "left hand" : "right hand";

        var wielded = equip.GetSlotItems(slot).FirstOrDefault();
        if (wielded is null)
            return CommandResult.Fail($"You're not wielding anything in your {handName}.");

        equip.UnequipItem(slot, wielded.ItemId);
        return CommandResult.Ok($"You lower the {wielded.ItemName}.");
    }
}
