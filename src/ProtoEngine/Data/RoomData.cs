namespace ProtoEngine.Data;

public class RoomData
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, string> Exits { get; set; } = new(); // direction → roomId
    public Dictionary<string, string> ExitPreviews { get; set; } = new(); // direction → preview description
    public List<string> ItemIds { get; set; } = new();
    public List<string> NpcIds { get; set; } = new();
}
