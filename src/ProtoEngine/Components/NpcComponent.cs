using ProtoEngine.Core;

namespace ProtoEngine.Components;

public class NpcComponent : IComponent
{
    public string Title { get; set; } = string.Empty; // Job/occupation (e.g., "merchant", "guard")
    public string DialogueId { get; set; } = string.Empty;
    public string Behavior { get; set; } = "idle"; // idle, patrol, merchant, hostile
    public bool IsHostile { get; set; }
    public Data.NpcDisposition Disposition { get; set; } = new(); // Personality and attitude
}
