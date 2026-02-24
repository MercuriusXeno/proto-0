using ProtoEngine.Core;

namespace ProtoEngine.Components;

/// <summary>
/// Tracks player's memories of rooms, items, NPCs, and events
/// </summary>
public class RoomMemoryComponent : IComponent
{
    public HashSet<string> VisitedRoomIds { get; set; } = new();
    public Dictionary<string, List<RoomMemory>> RoomMemories { get; set; } = new();
    public Dictionary<string, int> RoomVisitCounts { get; set; } = new(); // How many times entered each room
    public string CurrentRoomId { get; set; } = string.Empty; // Track current room for visit counting
}

public class RoomMemory
{
    public RoomMemoryType Type { get; set; }
    public string EntityId { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public int GameTick { get; set; }
    public string? AdditionalInfo { get; set; } // e.g., "murdered", "taken"
    public int RoomVisitNumber { get; set; } // Which visit to the room this memory was created
}

public enum RoomMemoryType
{
    ItemFound,
    ItemTaken,
    NpcMet,
    NpcKilled,
    ExitExplored
}
