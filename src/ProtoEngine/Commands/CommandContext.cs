using ProtoEngine.Core;
using ProtoEngine.Events;

namespace ProtoEngine.Commands;

public class CommandContext
{
    public required GameState State { get; init; }
    public required IEventBus EventBus { get; init; }
    public Entity Player => State.Player;
}
