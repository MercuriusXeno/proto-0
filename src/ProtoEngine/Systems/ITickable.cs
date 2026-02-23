using ProtoEngine.Core;

namespace ProtoEngine.Systems;

public interface ITickable
{
    void Tick(GameState state, GameClock clock);
}
