using ProtoEngine.Core;

namespace ProtoEngine.Components;

public class NpcComponent : IComponent
{
    public string DialogueId { get; set; } = string.Empty;
    public string Behavior { get; set; } = "idle"; // idle, patrol, merchant, hostile
    public bool IsHostile { get; set; }
}
