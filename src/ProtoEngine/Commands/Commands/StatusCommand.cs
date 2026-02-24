using ProtoEngine.Components;

namespace ProtoEngine.Commands.Commands;

public class StatusCommand : ICommand
{
    public string Verb => "status";
    public string[] Aliases => ["stats", "hp"];
    public string Description => "View your character status and stats";

    public CommandResult Execute(CommandContext context, string[] args)
    {
        var health = context.Player.Get<HealthComponent>();
        var stats = context.Player.Get<StatsComponent>();
        var desc = context.Player.Get<DescriptionComponent>();
        var equipment = context.Player.Get<EquipmentComponent>();
        var effects = context.Player.Get<StatusEffectsComponent>();

        var lines = new List<string>();
        lines.Add($"== {desc?.Name ?? "Adventurer"} ==");
        lines.Add($"  Level: {stats?.Level ?? 1}");
        lines.Add($"  HP:    {health?.Current ?? 0}/{health?.Max ?? 0}");
        lines.Add($"  XP:    {stats?.Experience ?? 0}/{stats?.ExperienceToNextLevel ?? 100}");
        lines.Add($"  STR:   {stats?.Strength ?? 0}");
        lines.Add($"  DEX:   {stats?.Dexterity ?? 0}");
        lines.Add($"  FOR:   {stats?.Fortitude ?? 0}");
        lines.Add($"  AGI:   {stats?.Agility ?? 0}");
        lines.Add($"  WIL:   {stats?.Willpower ?? 0}");
        lines.Add($"  INT:   {stats?.Intelligence ?? 0}");
        lines.Add($"  PER:   {stats?.Perception ?? 0}");
        lines.Add($"  CHA:   {stats?.Charisma ?? 0}");
        lines.Add($"  Gold:  {stats?.Gold ?? 0}");

        if (equipment is not null)
        {
            lines.Add("");
            lines.Add("Wielding:");
            var leftHand = equipment.GetSlotItem(EquipmentSlot.WieldLeft);
            var rightHand = equipment.GetSlotItem(EquipmentSlot.WieldRight);
            lines.Add($"  Left:  {leftHand?.ItemName ?? "(unarmed)"}");
            lines.Add($"  Right: {rightHand?.ItemName ?? "(unarmed)"}");

            lines.Add("");
            lines.Add("Wearing:");
            var head = equipment.GetSlotItem(EquipmentSlot.Head);
            var body = equipment.GetSlotItem(EquipmentSlot.Body);
            var arms = equipment.GetSlotItem(EquipmentSlot.Arms);
            var belt = equipment.GetSlotItem(EquipmentSlot.Belt);
            var legs = equipment.GetSlotItem(EquipmentSlot.Legs);
            var feet = equipment.GetSlotItem(EquipmentSlot.Feet);
            lines.Add($"  Head: {head?.ItemName ?? "(none)"}");
            lines.Add($"  Body: {body?.ItemName ?? "(none)"}");
            lines.Add($"  Arms: {arms?.ItemName ?? "(none)"}");
            lines.Add($"  Belt: {belt?.ItemName ?? "(none)"}");
            lines.Add($"  Legs: {legs?.ItemName ?? "(none)"}");
            lines.Add($"  Feet: {feet?.ItemName ?? "(none)"}");

            lines.Add("");
            lines.Add("Accessories:");
            var acc1 = equipment.GetSlotItem(EquipmentSlot.Accessory1);
            var acc2 = equipment.GetSlotItem(EquipmentSlot.Accessory2);
            var acc3 = equipment.GetSlotItem(EquipmentSlot.Accessory3);
            lines.Add($"  Acc 1: {acc1?.ItemName ?? "(none)"}");
            lines.Add($"  Acc 2: {acc2?.ItemName ?? "(none)"}");
            lines.Add($"  Acc 3: {acc3?.ItemName ?? "(none)"}");
        }

        if (effects?.Effects.Count > 0)
        {
            lines.Add("");
            lines.Add("Status Effects:");
            foreach (var effect in effects.Effects)
                lines.Add($"  - {effect.Name} ({effect.RemainingTicks} ticks remaining)");
        }

        return CommandResult.Ok(lines.ToArray());
    }
}
