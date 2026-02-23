using ProtoEngine.Core;

namespace ProtoEngine.Components;

public class HealthComponent : IComponent
{
    public int Current { get; set; } = 100;
    public int Max { get; set; } = 100;
    public bool IsAlive => Current > 0;
}
