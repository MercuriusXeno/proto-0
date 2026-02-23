using ProtoEngine.Components;
using ProtoEngine.Core;

namespace ProtoEngine.Systems;

public class ActionLogSystem : IGameSystem
{
    public void Initialize(GameState state)
    {
        // Ensure player has action log component
        if (state.Player.Get<ActionLogComponent>() is null)
            state.Player.Add(new ActionLogComponent());
    }

    public void LogMovement(GameState state, string fromRoom, string direction, string toRoom)
    {
        var log = state.Player.Get<ActionLogComponent>();
        if (log is null) return;

        var message = $"Departed {direction} from {fromRoom}";
        log.AddEntry(message, ActionLogType.Movement, state.Clock.Tick);
    }

    public void LogCombat(GameState state, string message)
    {
        var log = state.Player.Get<ActionLogComponent>();
        if (log is null) return;

        log.AddEntry(message, ActionLogType.Combat, state.Clock.Tick);
    }

    public void LogItem(GameState state, string message)
    {
        var log = state.Player.Get<ActionLogComponent>();
        if (log is null) return;

        log.AddEntry(message, ActionLogType.Item, state.Clock.Tick);
    }

    public void LogInteraction(GameState state, string message)
    {
        var log = state.Player.Get<ActionLogComponent>();
        if (log is null) return;

        log.AddEntry(message, ActionLogType.Interaction, state.Clock.Tick);
    }

    public void LogSystem(GameState state, string message)
    {
        var log = state.Player.Get<ActionLogComponent>();
        if (log is null) return;

        log.AddEntry(message, ActionLogType.System, state.Clock.Tick);
    }

    public List<ActionLogEntry> GetRecentEntries(GameState state, int count = 50)
    {
        var log = state.Player.Get<ActionLogComponent>();
        if (log is null) return new List<ActionLogEntry>();

        return log.Entries.TakeLast(count).ToList();
    }
}
