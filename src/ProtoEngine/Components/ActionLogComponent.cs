using ProtoEngine.Core;

namespace ProtoEngine.Components;

/// <summary>
/// Tracks recent player actions for display in the action log
/// </summary>
public class ActionLogComponent : IComponent
{
    public List<ActionLogEntry> Entries { get; set; } = new();
    public int MaxEntries { get; set; } = 100;

    public void AddEntry(string message, ActionLogType type, int gameTick)
    {
        Entries.Add(new ActionLogEntry
        {
            Message = message,
            Type = type,
            GameTick = gameTick
        });

        // Keep only recent entries
        if (Entries.Count > MaxEntries)
            Entries.RemoveAt(0);
    }
}

public class ActionLogEntry
{
    public string Message { get; set; } = string.Empty;
    public ActionLogType Type { get; set; }
    public int GameTick { get; set; }
}

public enum ActionLogType
{
    Movement,
    Combat,
    Item,
    Interaction,
    System
}
