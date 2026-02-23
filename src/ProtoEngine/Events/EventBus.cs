namespace ProtoEngine.Events;

public class EventBus : IEventBus
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();

    public void Subscribe<T>(Action<T> handler) where T : IGameEvent
    {
        var type = typeof(T);
        if (!_handlers.ContainsKey(type))
            _handlers[type] = new List<Delegate>();
        _handlers[type].Add(handler);
    }

    public void Publish<T>(T gameEvent) where T : IGameEvent
    {
        var type = typeof(T);
        if (_handlers.TryGetValue(type, out var handlers))
        {
            foreach (var handler in handlers)
                ((Action<T>)handler)(gameEvent);
        }
    }
}
