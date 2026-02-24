namespace ProtoEngine.Data;

public class NpcData
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty; // Actual name (e.g., "Scrimblo")
    public string Title { get; set; } = string.Empty; // Job/occupation (e.g., "merchant", "barkeep")
    public string Description { get; set; } = string.Empty;
    public string Behavior { get; set; } = "idle";
    public bool IsHostile { get; set; }
    public NpcDisposition Disposition { get; set; } = new(); // Personality and attitude
    public string DialogueId { get; set; } = string.Empty;
    public int Health { get; set; } = 50;
    public int AttackPower { get; set; } = 5;
    public int Defense { get; set; } = 2;
    public int ExperienceReward { get; set; } = 25;
}
