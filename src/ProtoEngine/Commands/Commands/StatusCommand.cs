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
        lines.Add($"  Level:        {stats?.Level ?? 1}");
        lines.Add($"  HP:           {health?.Current ?? 0}/{health?.Max ?? 0}");
        lines.Add($"  Experience:   {stats?.Experience ?? 0}/{stats?.ExperienceToNextLevel ?? 100}");
        lines.Add($"  Strength:     {stats?.Strength ?? 0}");
        lines.Add($"  Dexterity:    {stats?.Dexterity ?? 0}");
        lines.Add($"  Intelligence: {stats?.Intelligence ?? 0}");
        lines.Add($"  Gold:         {stats?.Gold ?? 0}");

        if (equipment is not null)
        {
            lines.Add("");
            lines.Add("Equipment:");
            lines.Add($"  Weapon:    {equipment.WeaponId ?? "(none)"}");
            lines.Add($"  Armor:     {equipment.ArmorId ?? "(none)"}");
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
