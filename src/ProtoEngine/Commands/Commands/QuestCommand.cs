using ProtoEngine.Systems;

namespace ProtoEngine.Commands.Commands;

public class QuestCommand : ICommand
{
    private readonly QuestSystem _quests;

    public string Verb => "quest";
    public string[] Aliases => ["quests", "journal"];
    public string Description => "View your quest journal";

    public QuestCommand(QuestSystem quests)
    {
        _quests = quests;
    }

    public CommandResult Execute(CommandContext context, string[] args)
    {
        var active = _quests.GetActiveQuestDetails();
        var completed = _quests.CompletedQuestIds;

        var lines = new List<string> { "== Quest Journal ==" };

        if (active.Count == 0 && completed.Count == 0)
        {
            lines.Add("No quests yet. Talk to NPCs to find quests.");
            return CommandResult.Ok(lines.ToArray());
        }

        if (active.Count > 0)
        {
            lines.Add("");
            lines.Add("Active Quests:");
            foreach (var (quest, aq) in active)
            {
                lines.Add($"  {quest.Name}");
                lines.Add($"    {quest.Description}");
                foreach (var obj in quest.Objectives)
                {
                    var progress = aq.Progress.GetValueOrDefault(obj.Id);
                    var status = progress >= obj.RequiredCount ? "[x]" : "[ ]";
                    lines.Add($"    {status} {obj.Description} ({progress}/{obj.RequiredCount})");
                }
            }
        }

        if (completed.Count > 0)
        {
            lines.Add("");
            lines.Add($"Completed: {completed.Count} quest(s)");
        }

        return CommandResult.Ok(lines.ToArray());
    }
}
