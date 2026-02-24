using ProtoEngine.Core;

namespace ProtoEngine.Tests.Core;

public class GameClockTests
{
    [Fact]
    public void NewClock_StartsAtTickZero()
    {
        var clock = new GameClock();

        Assert.Equal(0, clock.Tick);
    }

    [Fact]
    public void Advance_IncrementsByOne()
    {
        var clock = new GameClock();

        clock.Advance();

        Assert.Equal(1, clock.Tick);
    }

    [Fact]
    public void Advance_CustomAmount_IncrementsByAmount()
    {
        var clock = new GameClock();

        clock.Advance(5);

        Assert.Equal(5, clock.Tick);
    }

    [Fact]
    public void Hour_WrapsAt24()
    {
        var clock = new GameClock();
        clock.Set(25);

        Assert.Equal(1, clock.Hour);
    }

    [Fact]
    public void Day_StartsAtOne()
    {
        var clock = new GameClock();

        Assert.Equal(1, clock.Day);
    }

    [Fact]
    public void Day_IncreasesAfter24Ticks()
    {
        var clock = new GameClock();
        clock.Set(24);

        Assert.Equal(2, clock.Day);
    }

    [Theory]
    [InlineData(5, "dawn")]
    [InlineData(7, "dawn")]
    [InlineData(8, "morning")]
    [InlineData(11, "morning")]
    [InlineData(12, "midday")]
    [InlineData(13, "midday")]
    [InlineData(14, "afternoon")]
    [InlineData(17, "afternoon")]
    [InlineData(18, "evening")]
    [InlineData(20, "evening")]
    [InlineData(21, "night")]
    [InlineData(0, "night")]
    [InlineData(4, "night")]
    public void TimeOfDay_ReturnsCorrectPeriod(int tick, string expected)
    {
        var clock = new GameClock();
        clock.Set(tick);

        Assert.Equal(expected, clock.TimeOfDay);
    }
}
