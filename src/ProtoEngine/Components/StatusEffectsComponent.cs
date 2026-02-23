using ProtoEngine.Core;

namespace ProtoEngine.Components;

public class StatusEffect
{
    public string Name { get; set; } = string.Empty;
    public int RemainingTicks { get; set; }
    public int StatModifier { get; set; }
    public string AffectedStat { get; set; } = string.Empty;
}

public class StatusEffectsComponent : IComponent
{
    public List<StatusEffect> Effects { get; set; } = new();
}
