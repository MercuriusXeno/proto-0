using ProtoEngine.Components;
using ProtoEngine.Core;

namespace ProtoEngine.Systems;

public class StatusEffectSystem : IGameSystem, ITickable
{
    public void Initialize(GameState state) { }

    public void Tick(GameState state, GameClock clock)
    {
        foreach (var entity in state.Entities.Values)
        {
            var effects = entity.Get<StatusEffectsComponent>();
            if (effects is null) continue;

            for (int i = effects.Effects.Count - 1; i >= 0; i--)
            {
                effects.Effects[i].RemainingTicks--;
                if (effects.Effects[i].RemainingTicks <= 0)
                    effects.Effects.RemoveAt(i);
            }
        }

        // Also process player effects
        var playerEffects = state.Player.Get<StatusEffectsComponent>();
        if (playerEffects is not null)
        {
            for (int i = playerEffects.Effects.Count - 1; i >= 0; i--)
            {
                playerEffects.Effects[i].RemainingTicks--;
                if (playerEffects.Effects[i].RemainingTicks <= 0)
                    playerEffects.Effects.RemoveAt(i);
            }
        }
    }
}
