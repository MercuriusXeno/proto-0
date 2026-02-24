using ProtoEngine.Commands.Commands;

namespace ProtoEngine.Tests.Commands;

public class MoveCommandTests
{
    [Theory]
    [InlineData("north", "north")]
    [InlineData("south", "south")]
    [InlineData("east", "east")]
    [InlineData("west", "west")]
    [InlineData("up", "up")]
    [InlineData("down", "down")]
    public void ResolveDirection_FullDirectionInArgs_ReturnsDirection(string arg, string expected)
    {
        var result = MoveCommand.ResolveDirection(new[] { arg }, null);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("n", "north")]
    [InlineData("s", "south")]
    [InlineData("e", "east")]
    [InlineData("w", "west")]
    [InlineData("u", "up")]
    [InlineData("d", "down")]
    public void ResolveDirection_AbbreviationInArgs_ExpandsToFull(string arg, string expected)
    {
        var result = MoveCommand.ResolveDirection(new[] { arg }, null);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("north")]
    [InlineData("s")]
    [InlineData("east")]
    [InlineData("w")]
    public void ResolveDirection_DirectionAsOriginalVerb_ReturnsDirection(string verb)
    {
        var result = MoveCommand.ResolveDirection(Array.Empty<string>(), verb);

        Assert.NotNull(result);
    }

    [Fact]
    public void ResolveDirection_AbbreviationAsVerb_ExpandsToFull()
    {
        var result = MoveCommand.ResolveDirection(Array.Empty<string>(), "n");

        Assert.Equal("north", result);
    }

    [Fact]
    public void ResolveDirection_NoArgsNoVerb_ReturnsNull()
    {
        var result = MoveCommand.ResolveDirection(Array.Empty<string>(), null);

        Assert.Null(result);
    }

    [Fact]
    public void ResolveDirection_NonDirectionVerb_ReturnsNull()
    {
        var result = MoveCommand.ResolveDirection(Array.Empty<string>(), "go");

        Assert.Null(result);
    }

    [Fact]
    public void ResolveDirection_ArgsOverrideVerb()
    {
        var result = MoveCommand.ResolveDirection(new[] { "south" }, "north");

        Assert.Equal("south", result);
    }

    [Fact]
    public void ResolveDirection_CaseInsensitive()
    {
        var result = MoveCommand.ResolveDirection(new[] { "NORTH" }, null);

        Assert.Equal("north", result);
    }
}
