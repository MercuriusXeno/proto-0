using ProtoEngine.Core;
using ProtoEngine.Events;

namespace ProtoEngine.Systems;

public class EventSystem : IGameSystem
{
    private readonly IEventBus _eventBus;

    public EventSystem(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public void Initialize(GameState state) { }
}
