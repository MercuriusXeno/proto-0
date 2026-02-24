using ProtoEngine.Core;

namespace ProtoEngine.Components;

/// <summary>
/// Tracks which rooms the player has visited (for exit exploration tracking)
/// </summary>
public class ExploredRoomsComponent : IComponent
{
    public HashSet<string> VisitedRoomIds { get; set; } = new();
}
