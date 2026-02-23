using ProtoEngine.Core;

namespace ProtoEngine.Components;

public class InventoryComponent : IComponent
{
    public List<string> ItemIds { get; set; } = new();
    public int Capacity { get; set; } = 20;
    public bool IsFull => ItemIds.Count >= Capacity;
}
