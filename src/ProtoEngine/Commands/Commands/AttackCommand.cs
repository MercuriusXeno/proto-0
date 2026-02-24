using ProtoEngine.Components;
using ProtoEngine.Systems;

namespace ProtoEngine.Commands.Commands;

public class AttackCommand : ICommand
{
    private readonly CombatSystem _combat;
    private readonly PlayerSystem _player;
    private readonly ActionLogSystem _actionLog;

    public string Verb => "attack";
    public string[] Aliases => ["fight", "hit", "kill"];
    public string Description => "Attack an NPC (attack <target name>)";

    public AttackCommand(CombatSystem combat, PlayerSystem player, ActionLogSystem actionLog)
    {
        _combat = combat;
        _player = player;
        _actionLog = actionLog;
    }

    public CommandResult Execute(CommandContext context, string[] args)
    {
        if (args.Length == 0)
            return CommandResult.Fail("Attack whom? Specify a target.");

        var targetName = string.Join(" ", args);
        var target = _combat.FindTargetInRoom(context.State, targetName);

        if (target is null)
            return CommandResult.Fail($"There is no '{targetName}' here to attack.");

        var desc = target.Get<DescriptionComponent>();
        _actionLog.LogCombat(context.State, $"Attacked {desc?.Name ?? "enemy"}!");

        var (damage, targetDied, messages) = _combat.Attack(context.State, target);

        if (targetDied)
            HandleTargetKilled(context, desc, messages);
        else
            AppendPlayerHealth(context, messages);

        return CommandResult.Ok(messages.ToArray());
    }

    private void HandleTargetKilled(CommandContext context, DescriptionComponent? desc, List<string> messages)
    {
        if (desc is not null)
            _actionLog.LogCombat(context.State, $"Slew {desc.Name} in cold blood!");

        _player.TryAddExperience(context.State, 25, out var leveledUp);
        messages.Add("You gain 25 experience.");

        if (leveledUp)
        {
            var stats = context.Player.Get<StatsComponent>();
            messages.Add($"** LEVEL UP! You are now level {stats?.Level}! **");
        }
    }

    private static void AppendPlayerHealth(CommandContext context, List<string> messages)
    {
        var playerHealth = context.Player.Get<HealthComponent>();
        if (playerHealth is not null)
            messages.Add($"Your HP: {playerHealth.Current}/{playerHealth.Max}");
    }
}
