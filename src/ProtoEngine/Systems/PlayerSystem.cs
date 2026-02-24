using ProtoEngine.Components;
using ProtoEngine.Core;
using ProtoEngine.Events;

namespace ProtoEngine.Systems;

public class PlayerSystem : IGameSystem
{
    private readonly IEventBus _eventBus;

    public PlayerSystem(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public void Initialize(GameState state)
    {
        var player = state.Player;
        if (!player.Has<HealthComponent>()) player.Add(new HealthComponent());
        if (!player.Has<StatsComponent>()) player.Add(new StatsComponent());
        if (!player.Has<InventoryComponent>()) player.Add(new InventoryComponent());
        if (!player.Has<EquipmentComponent>()) player.Add(new EquipmentComponent());
        if (!player.Has<CombatComponent>()) player.Add(new CombatComponent());
        if (!player.Has<PositionComponent>()) player.Add(new PositionComponent());
        if (!player.Has<ExerciseComponent>()) player.Add(new ExerciseComponent());
        if (!player.Has<StatusEffectsComponent>()) player.Add(new StatusEffectsComponent());
        if (!player.Has<DescriptionComponent>())
            player.Add(new DescriptionComponent { Name = "Adventurer", ShortDescription = "A brave adventurer" });
    }

    public bool TryAddExperience(GameState state, int amount, out bool leveledUp)
    {
        leveledUp = false;
        var stats = state.Player.Get<StatsComponent>();
        if (stats is null) return false;

        stats.Experience += amount;
        while (stats.Experience >= stats.ExperienceToNextLevel)
        {
            stats.Experience -= stats.ExperienceToNextLevel;
            stats.Level++;
            stats.Strength += 2;
            stats.Dexterity += 2;
            stats.Intelligence += 2;
            var health = state.Player.Get<HealthComponent>();
            if (health is not null)
            {
                health.Max += 10;
                health.Current = health.Max;
            }
            leveledUp = true;
            _eventBus.Publish(new LevelUpEvent(state.Player.Id, stats.Level));
        }
        return true;
    }
}
