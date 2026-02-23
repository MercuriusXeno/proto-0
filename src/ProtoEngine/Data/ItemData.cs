namespace ProtoEngine.Data;

public class ItemData
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = "misc"; // weapon, armor, consumable, key, misc
    public int AttackBonus { get; set; }
    public int DefenseBonus { get; set; }
    public int HealAmount { get; set; }
    public bool IsPickable { get; set; } = true;
}
