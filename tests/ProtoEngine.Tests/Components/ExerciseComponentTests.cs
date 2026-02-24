using ProtoEngine.Components;

namespace ProtoEngine.Tests.Components;

public class ExerciseComponentTests
{
    [Fact]
    public void GetProgress_NoProgress_ReturnsZero()
    {
        var exercise = new ExerciseComponent();

        Assert.Equal(0.0, exercise.GetProgress(StatType.Strength));
    }

    [Fact]
    public void AddProgress_AccumulatesCorrectly()
    {
        var exercise = new ExerciseComponent();

        exercise.AddProgress(StatType.Strength, 10.0);
        exercise.AddProgress(StatType.Strength, 15.0);

        Assert.Equal(25.0, exercise.GetProgress(StatType.Strength));
    }

    [Fact]
    public void AddProgress_IndependentPerStat()
    {
        var exercise = new ExerciseComponent();

        exercise.AddProgress(StatType.Strength, 50.0);
        exercise.AddProgress(StatType.Dexterity, 30.0);

        Assert.Equal(50.0, exercise.GetProgress(StatType.Strength));
        Assert.Equal(30.0, exercise.GetProgress(StatType.Dexterity));
    }

    [Fact]
    public void AddProgress_FractionalAmounts()
    {
        var exercise = new ExerciseComponent();

        exercise.AddProgress(StatType.Perception, 0.5);
        exercise.AddProgress(StatType.Perception, 0.3);

        Assert.Equal(0.8, exercise.GetProgress(StatType.Perception), precision: 10);
    }
}
