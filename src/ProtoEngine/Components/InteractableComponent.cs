using ProtoEngine.Core;

namespace ProtoEngine.Components;

public class InteractableComponent : IComponent
{
    public string InteractionType { get; set; } = string.Empty; // door, lever, chest, etc.
    public string? TargetId { get; set; }
    public bool IsActive { get; set; } = true;
    public string? UseMessage { get; set; }
}
