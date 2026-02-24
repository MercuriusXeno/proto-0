using ProtoEngine.Components;
using ProtoEngine.Core;
using ProtoEngine.Events;

namespace ProtoEngine.Systems;

public class StatGrowthConfig
{
    public double BaseCost { get; set; } = 100.0;
    public double Ratio { get; set; } = 1.15;
    public int BaseStat { get; set; } = 10;
    public Dictionary<StatType, StatGrowthOverride>? Overrides { get; set; }
}

public class StatGrowthOverride
{
    public double? BaseCost { get; set; }
    public double? Ratio { get; set; }
}

public class StatGrowthSystem : IGameSystem
{
    private readonly IEventBus _eventBus;
    private readonly StatGrowthConfig _config;

    public StatGrowthSystem(IEventBus eventBus, StatGrowthConfig? config = null)
    {
        _eventBus = eventBus;
        _config = config ?? new StatGrowthConfig();
    }

    public void Initialize(GameState state) { }

    public void Exercise(GameState state, StatType stat, double amount)
    {
        var stats = state.Player.Get<StatsComponent>();
        var exercise = state.Player.Get<ExerciseComponent>();
        if (stats is null || exercise is null || amount <= 0) return;

        exercise.AddProgress(stat, amount);
        _eventBus.Publish(new StatExercisedEvent(state.Player.Id, stat, amount));

        // Check for stat increases (loop handles excess exercise rolling over)
        while (true)
        {
            var currentValue = stats.GetStat(stat);
            var timesRaised = currentValue - _config.BaseStat;
            if (timesRaised < 0) timesRaised = 0;

            var threshold = GetThreshold(stat, timesRaised);
            var progress = exercise.GetProgress(stat);

            if (progress < threshold) break;

            // Subtract threshold and increment stat
            exercise.Progress[stat] = progress - threshold;
            var newValue = currentValue + 1;
            stats.SetStat(stat, newValue);
            _eventBus.Publish(new StatIncreasedEvent(state.Player.Id, stat, currentValue, newValue));
        }
    }

    public double GetThreshold(StatType stat, int timesRaised)
    {
        var baseCost = _config.BaseCost;
        var ratio = _config.Ratio;

        if (_config.Overrides?.TryGetValue(stat, out var over) == true)
        {
            if (over.BaseCost.HasValue) baseCost = over.BaseCost.Value;
            if (over.Ratio.HasValue) ratio = over.Ratio.Value;
        }

        return baseCost * Math.Pow(ratio, timesRaised);
    }
}
