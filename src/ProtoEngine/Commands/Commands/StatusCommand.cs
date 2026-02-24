using ProtoEngine.Components;

namespace ProtoEngine.Commands.Commands;

public class StatusCommand : ICommand
{
    private static readonly (EquipmentSlot Slot, string Label)[] WieldSlots =
    [
        (EquipmentSlot.WieldLeft, "Left"),
        (EquipmentSlot.WieldRight, "Right")
    ];

    private static readonly (EquipmentSlot Slot, string Label)[] ArmorSlots =
    [
        (EquipmentSlot.Head, "Head"),
        (EquipmentSlot.Body, "Body"),
        (EquipmentSlot.Arms, "Arms"),
        (EquipmentSlot.Belt, "Belt"),
        (EquipmentSlot.Legs, "Legs"),
        (EquipmentSlot.Feet, "Feet")
    ];

    private static readonly (EquipmentSlot Slot, string Label)[] AccessorySlots =
    [
        (EquipmentSlot.Accessory1, "Acc 1"),
        (EquipmentSlot.Accessory2, "Acc 2"),
        (EquipmentSlot.Accessory3, "Acc 3")
    ];

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

        foreach (var stat in Enum.GetValues<StatType>())
            lines.Add($"  {ToAbbreviation(stat)}:   {stats?.GetStat(stat) ?? 0}");

        lines.Add($"  Gold:  {stats?.Gold ?? 0}");

        if (equipment is not null)
        {
            AppendSlotGroup(lines, "Wielding:", WieldSlots, equipment, "(unarmed)");
            AppendSlotGroup(lines, "Wearing:", ArmorSlots, equipment, "(none)");
            AppendSlotGroup(lines, "Accessories:", AccessorySlots, equipment, "(none)");
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

    private static void AppendSlotGroup(
        List<string> lines,
        string header,
        (EquipmentSlot Slot, string Label)[] slots,
        EquipmentComponent equipment,
        string emptyLabel)
    {
        lines.Add("");
        lines.Add(header);
        foreach (var (slot, label) in slots)
        {
            var item = equipment.GetSlotItem(slot);
            lines.Add($"  {label}: {item?.ItemName ?? emptyLabel}");
        }
    }

    /// <summary>
    /// Converts a StatType to its 3-letter abbreviation for display.
    /// </summary>
    internal static string ToAbbreviation(StatType stat) => stat switch
    {
        StatType.Strength => "STR",
        StatType.Dexterity => "DEX",
        StatType.Fortitude => "FOR",
        StatType.Agility => "AGI",
        StatType.Willpower => "WIL",
        StatType.Intelligence => "INT",
        StatType.Perception => "PER",
        StatType.Charisma => "CHA",
        _ => stat.ToString()[..3].ToUpperInvariant()
    };
}
