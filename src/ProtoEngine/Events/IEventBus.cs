namespace ProtoEngine.Events;

public interface IGameEvent { }

public interface IEventBus
{
    void Publish<T>(T gameEvent) where T : IGameEvent;
    void Subscribe<T>(Action<T> handler) where T : IGameEvent;
}
