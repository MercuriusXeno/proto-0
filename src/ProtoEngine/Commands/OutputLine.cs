namespace ProtoEngine.Commands;

public record OutputLine
{
    public string Text { get; init; } = string.Empty;
    public List<EntityReference> Entities { get; init; } = new();

    public static OutputLine Plain(string text)
        => new() { Text = text };

    public static OutputLine WithEntities(string text, params EntityReference[] entities)
        => new() { Text = text, Entities = entities.ToList() };
}
