using ProtoEngine.Components;
using ProtoEngine.Core;
using ProtoEngine.Events;
using ProtoEngine.Systems;

namespace ProtoEngine.Tests.Systems;

public class StatGrowthSystemTests
{
    private static (StatGrowthSystem system, GameState state, EventBus eventBus) CreateTestHarness(
        StatGrowthConfig? config = null)
    {
        var eventBus = new EventBus();
        var system = new StatGrowthSystem(eventBus, config);
        var state = new GameState();
        state.Player.Add(new StatsComponent());
        state.Player.Add(new ExerciseComponent());
        return (system, state, eventBus);
    }

    [Fact]
    public void GetThreshold_FirstLevel_ReturnsBaseCost()
    {
        var (system, _, _) = CreateTestHarness();

        var threshold = system.GetThreshold(StatType.Strength, 0);

        Assert.Equal(100.0, threshold);
    }

    [Fact]
    public void GetThreshold_SecondLevel_AppliesCompounding()
    {
        var (system, _, _) = CreateTestHarness();

        var threshold = system.GetThreshold(StatType.Strength, 1);

        Assert.Equal(100.0 * 1.15, threshold, precision: 10);
    }

    [Fact]
    public void GetThreshold_MultipleRaises_CompoundsGeometrically()
    {
        var (system, _, _) = CreateTestHarness();

        var threshold = system.GetThreshold(StatType.Strength, 5);

        var expected = 100.0 * Math.Pow(1.15, 5);
        Assert.Equal(expected, threshold, precision: 10);
    }

    [Fact]
    public void GetThreshold_WithOverride_UsesOverrideValues()
    {
        var config = new StatGrowthConfig
        {
            Overrides = new Dictionary<StatType, StatGrowthOverride>
            {
                [StatType.Intelligence] = new() { BaseCost = 50.0, Ratio = 1.10 }
            }
        };
        var (system, _, _) = CreateTestHarness(config);

        var threshold = system.GetThreshold(StatType.Intelligence, 2);

        var expected = 50.0 * Math.Pow(1.10, 2);
        Assert.Equal(expected, threshold, precision: 10);
    }

    [Fact]
    public void GetThreshold_OverrideOnDifferentStat_UsesDefaults()
    {
        var config = new StatGrowthConfig
        {
            Overrides = new Dictionary<StatType, StatGrowthOverride>
            {
                [StatType.Intelligence] = new() { BaseCost = 50.0 }
            }
        };
        var (system, _, _) = CreateTestHarness(config);

        var threshold = system.GetThreshold(StatType.Strength, 0);

        Assert.Equal(100.0, threshold);
    }

    [Fact]
    public void Exercise_BelowThreshold_DoesNotIncreaseStat()
    {
        var (system, state, _) = CreateTestHarness();

        system.Exercise(state, StatType.Strength, 50.0);

        Assert.Equal(10, state.Player.Get<StatsComponent>()!.GetStat(StatType.Strength));
    }

    [Fact]
    public void Exercise_BelowThreshold_AccumulatesProgress()
    {
        var (system, state, _) = CreateTestHarness();

        system.Exercise(state, StatType.Strength, 50.0);

        Assert.Equal(50.0, state.Player.Get<ExerciseComponent>()!.GetProgress(StatType.Strength));
    }

    [Fact]
    public void Exercise_ExactlyAtThreshold_IncrementsStat()
    {
        var (system, state, _) = CreateTestHarness();

        system.Exercise(state, StatType.Strength, 100.0);

        Assert.Equal(11, state.Player.Get<StatsComponent>()!.GetStat(StatType.Strength));
    }

    [Fact]
    public void Exercise_ExactlyAtThreshold_ZerosProgress()
    {
        var (system, state, _) = CreateTestHarness();

        system.Exercise(state, StatType.Strength, 100.0);

        Assert.Equal(0.0, state.Player.Get<ExerciseComponent>()!.GetProgress(StatType.Strength));
    }

    [Fact]
    public void Exercise_ExceedsThreshold_RollsOverExcess()
    {
        var (system, state, _) = CreateTestHarness();

        system.Exercise(state, StatType.Strength, 130.0);

        Assert.Equal(11, state.Player.Get<StatsComponent>()!.GetStat(StatType.Strength));
        Assert.Equal(30.0, state.Player.Get<ExerciseComponent>()!.GetProgress(StatType.Strength));
    }

    [Fact]
    public void Exercise_EnoughForTwoLevels_IncrementsTwice()
    {
        var (system, state, _) = CreateTestHarness();
        var firstThreshold = 100.0;
        var secondThreshold = 100.0 * 1.15;

        system.Exercise(state, StatType.Strength, firstThreshold + secondThreshold);

        Assert.Equal(12, state.Player.Get<StatsComponent>()!.GetStat(StatType.Strength));
        Assert.Equal(0.0, state.Player.Get<ExerciseComponent>()!.GetProgress(StatType.Strength), precision: 10);
    }

    [Fact]
    public void Exercise_ZeroAmount_NoEffect()
    {
        var (system, state, _) = CreateTestHarness();

        system.Exercise(state, StatType.Strength, 0);

        Assert.Equal(0.0, state.Player.Get<ExerciseComponent>()!.GetProgress(StatType.Strength));
    }

    [Fact]
    public void Exercise_NegativeAmount_NoEffect()
    {
        var (system, state, _) = CreateTestHarness();

        system.Exercise(state, StatType.Strength, -10.0);

        Assert.Equal(0.0, state.Player.Get<ExerciseComponent>()!.GetProgress(StatType.Strength));
    }

    [Fact]
    public void Exercise_PublishesStatExercisedEvent()
    {
        var (system, state, eventBus) = CreateTestHarness();
        StatExercisedEvent? captured = null;
        eventBus.Subscribe<StatExercisedEvent>(e => captured = e);

        system.Exercise(state, StatType.Dexterity, 25.0);

        Assert.NotNull(captured);
        Assert.Equal(StatType.Dexterity, captured.Stat);
        Assert.Equal(25.0, captured.Amount);
    }

    [Fact]
    public void Exercise_AtThreshold_PublishesStatIncreasedEvent()
    {
        var (system, state, eventBus) = CreateTestHarness();
        StatIncreasedEvent? captured = null;
        eventBus.Subscribe<StatIncreasedEvent>(e => captured = e);

        system.Exercise(state, StatType.Strength, 100.0);

        Assert.NotNull(captured);
        Assert.Equal(StatType.Strength, captured.Stat);
        Assert.Equal(10, captured.OldValue);
        Assert.Equal(11, captured.NewValue);
    }

    [Fact]
    public void Exercise_BelowThreshold_DoesNotPublishStatIncreasedEvent()
    {
        var (system, state, eventBus) = CreateTestHarness();
        StatIncreasedEvent? captured = null;
        eventBus.Subscribe<StatIncreasedEvent>(e => captured = e);

        system.Exercise(state, StatType.Strength, 50.0);

        Assert.Null(captured);
    }

    [Fact]
    public void Exercise_IndependentStatTracking()
    {
        var (system, state, _) = CreateTestHarness();

        system.Exercise(state, StatType.Strength, 100.0);
        system.Exercise(state, StatType.Dexterity, 50.0);

        var stats = state.Player.Get<StatsComponent>()!;
        Assert.Equal(11, stats.GetStat(StatType.Strength));
        Assert.Equal(10, stats.GetStat(StatType.Dexterity));
    }

    [Fact]
    public void Exercise_IncrementalProgress_AccumulatesAcrossCalls()
    {
        var (system, state, _) = CreateTestHarness();

        system.Exercise(state, StatType.Strength, 30.0);
        system.Exercise(state, StatType.Strength, 30.0);
        system.Exercise(state, StatType.Strength, 40.0);

        Assert.Equal(11, state.Player.Get<StatsComponent>()!.GetStat(StatType.Strength));
    }

    [Fact]
    public void Exercise_MissingStatsComponent_NoEffect()
    {
        var eventBus = new EventBus();
        var system = new StatGrowthSystem(eventBus);
        var state = new GameState();
        state.Player.Add(new ExerciseComponent());

        system.Exercise(state, StatType.Strength, 100.0);

        Assert.Equal(0.0, state.Player.Get<ExerciseComponent>()!.GetProgress(StatType.Strength));
    }

    [Fact]
    public void Exercise_MissingExerciseComponent_NoEffect()
    {
        var eventBus = new EventBus();
        var system = new StatGrowthSystem(eventBus);
        var state = new GameState();
        state.Player.Add(new StatsComponent());

        system.Exercise(state, StatType.Strength, 100.0);

        Assert.Equal(10, state.Player.Get<StatsComponent>()!.GetStat(StatType.Strength));
    }
}
