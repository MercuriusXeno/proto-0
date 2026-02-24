using ProtoEngine.Commands.Commands;
using ProtoEngine.Components;

namespace ProtoEngine.Tests.Commands;

public class StatusCommandTests
{
    [Theory]
    [InlineData(StatType.Strength, "STR")]
    [InlineData(StatType.Dexterity, "DEX")]
    [InlineData(StatType.Fortitude, "FOR")]
    [InlineData(StatType.Agility, "AGI")]
    [InlineData(StatType.Willpower, "WIL")]
    [InlineData(StatType.Intelligence, "INT")]
    [InlineData(StatType.Perception, "PER")]
    [InlineData(StatType.Charisma, "CHA")]
    public void ToAbbreviation_AllStatTypes_ReturnsCorrect3LetterCode(StatType stat, string expected)
    {
        var result = StatusCommand.ToAbbreviation(stat);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToAbbreviation_AllStatTypes_ReturnExactly3Characters()
    {
        foreach (var stat in Enum.GetValues<StatType>())
        {
            var result = StatusCommand.ToAbbreviation(stat);

            Assert.Equal(3, result.Length);
        }
    }

    [Fact]
    public void ToAbbreviation_AllStatTypes_ReturnUpperCase()
    {
        foreach (var stat in Enum.GetValues<StatType>())
        {
            var result = StatusCommand.ToAbbreviation(stat);

            Assert.Equal(result.ToUpperInvariant(), result);
        }
    }
}
