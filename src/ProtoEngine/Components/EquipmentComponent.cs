using ProtoEngine.Core;

namespace ProtoEngine.Components;

public class EquipmentComponent : IComponent
{
    public string? WeaponId { get; set; }
    public string? ArmorId { get; set; }
    public string? ShieldId { get; set; }
    public string? AccessoryId { get; set; }
}
