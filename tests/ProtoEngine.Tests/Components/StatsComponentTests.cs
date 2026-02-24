using ProtoEngine.Components;

namespace ProtoEngine.Tests.Components;

public class StatsComponentTests
{
    [Fact]
    public void AllStats_DefaultToTen()
    {
        var stats = new StatsComponent();

        foreach (StatType stat in Enum.GetValues<StatType>())
        {
            Assert.Equal(10, stats.GetStat(stat));
        }
    }

    [Theory]
    [InlineData(StatType.Strength)]
    [InlineData(StatType.Dexterity)]
    [InlineData(StatType.Fortitude)]
    [InlineData(StatType.Agility)]
    [InlineData(StatType.Willpower)]
    [InlineData(StatType.Intelligence)]
    [InlineData(StatType.Perception)]
    [InlineData(StatType.Charisma)]
    public void SetStat_GetStat_RoundTrips(StatType stat)
    {
        var stats = new StatsComponent();

        stats.SetStat(stat, 25);

        Assert.Equal(25, stats.GetStat(stat));
    }

    [Fact]
    public void SetStat_DoesNotAffectOtherStats()
    {
        var stats = new StatsComponent();

        stats.SetStat(StatType.Strength, 99);

        Assert.Equal(10, stats.GetStat(StatType.Dexterity));
        Assert.Equal(10, stats.GetStat(StatType.Intelligence));
    }

    [Fact]
    public void GetStat_InvalidStatType_Throws()
    {
        var stats = new StatsComponent();

        Assert.Throws<ArgumentOutOfRangeException>(() => stats.GetStat((StatType)999));
    }

    [Fact]
    public void SetStat_InvalidStatType_Throws()
    {
        var stats = new StatsComponent();

        Assert.Throws<ArgumentOutOfRangeException>(() => stats.SetStat((StatType)999, 10));
    }
}
