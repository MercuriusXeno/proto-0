using ProtoEngine.Core;

namespace ProtoEngine.Components;

public class CombatComponent : IComponent
{
    public int AttackPower { get; set; } = 5;
    public int Defense { get; set; } = 2;
    public bool InCombat { get; set; }
    public string? TargetId { get; set; }
}
