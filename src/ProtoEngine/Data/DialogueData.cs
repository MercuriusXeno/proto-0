namespace ProtoEngine.Data;

public class DialogueData
{
    public string Id { get; set; } = string.Empty;
    public List<DialogueNode> Nodes { get; set; } = new();
}

public class DialogueNode
{
    public string Id { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public List<DialogueChoice> Choices { get; set; } = new();
    public string? QuestToGive { get; set; }
}

public class DialogueChoice
{
    public string Text { get; set; } = string.Empty;
    public string NextNodeId { get; set; } = string.Empty;
}
