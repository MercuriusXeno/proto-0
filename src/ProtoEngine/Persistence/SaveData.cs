namespace ProtoEngine.Persistence;

public class SaveData
{
    public int ClockTick { get; set; }
    public string PlayerRoomId { get; set; } = string.Empty;
    public int PlayerHealth { get; set; }
    public int PlayerMaxHealth { get; set; }
    public int PlayerLevel { get; set; }
    public int PlayerExperience { get; set; }
    public int PlayerStrength { get; set; }
    public int PlayerDexterity { get; set; }
    public int PlayerIntelligence { get; set; }
    public int PlayerGold { get; set; }
    public List<string> InventoryItemIds { get; set; } = new();
    public string? WeaponId { get; set; }
    public string? ArmorId { get; set; }
    public DateTime SavedAt { get; set; } = DateTime.UtcNow;
}
