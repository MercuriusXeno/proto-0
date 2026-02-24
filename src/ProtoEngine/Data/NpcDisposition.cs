namespace ProtoEngine.Data;

/// <summary>
/// Defines NPC personality and behavioral traits
/// </summary>
public class NpcDisposition
{
    public string Type { get; set; } = "friendly"; // friendly, standoffish, hostile, etc.

    // Feature flags driven by NPC attitudes
    public bool OnlyGivesNameOnIntroduction { get; set; } = false; // If true, doesn't share title

    // Future attitude-driven features can be added here:
    // public bool RefusesToTrade { get; set; } = false;
    // public bool DistrustfulOfStrangers { get; set; } = false;
    // public int InitialAttitude { get; set; } = 0; // -100 to 100
    // public bool RemembersInsults { get; set; } = true;
}

/// <summary>
/// Predefined disposition types
/// </summary>
public static class NpcDispositions
{
    public static NpcDisposition Friendly => new()
    {
        Type = "friendly",
        OnlyGivesNameOnIntroduction = false
    };

    public static NpcDisposition Standoffish => new()
    {
        Type = "standoffish",
        OnlyGivesNameOnIntroduction = true
    };

    public static NpcDisposition Hostile => new()
    {
        Type = "hostile",
        OnlyGivesNameOnIntroduction = true
    };
}
