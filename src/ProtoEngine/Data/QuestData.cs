namespace ProtoEngine.Data;

public class QuestData
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<QuestObjective> Objectives { get; set; } = new();
    public QuestReward Reward { get; set; } = new();
}

public class QuestObjective
{
    public string Id { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // kill, collect, visit, talk
    public string TargetId { get; set; } = string.Empty;
    public int RequiredCount { get; set; } = 1;
}

public class QuestReward
{
    public int Experience { get; set; }
    public int Gold { get; set; }
    public List<string> ItemIds { get; set; } = new();
}
