using ProtoEngine.Commands;

namespace ProtoEngine.Tests.Commands;

public class CommandClassifierTests
{
    [Theory]
    [InlineData("look")]
    [InlineData("l")]
    [InlineData("Look")]
    [InlineData("LOOK")]
    public void IsLookCommand_LookVerbs_ReturnsTrue(string command)
    {
        Assert.True(CommandClassifier.IsLookCommand(command));
    }

    [Theory]
    [InlineData("look around")]
    [InlineData("l north")]
    public void IsLookCommand_LookVerbWithArgs_ReturnsTrue(string command)
    {
        Assert.True(CommandClassifier.IsLookCommand(command));
    }

    [Theory]
    [InlineData("take sword")]
    [InlineData("attack goblin")]
    [InlineData("north")]
    [InlineData("go north")]
    [InlineData("")]
    public void IsLookCommand_NonLookVerbs_ReturnsFalse(string command)
    {
        Assert.False(CommandClassifier.IsLookCommand(command));
    }

    [Theory]
    [InlineData("go")]
    [InlineData("move")]
    [InlineData("walk")]
    [InlineData("north")]
    [InlineData("south")]
    [InlineData("east")]
    [InlineData("west")]
    [InlineData("up")]
    [InlineData("down")]
    [InlineData("n")]
    [InlineData("s")]
    [InlineData("e")]
    [InlineData("w")]
    [InlineData("u")]
    [InlineData("d")]
    public void IsNavigationCommand_NavigationVerbs_ReturnsTrue(string command)
    {
        Assert.True(CommandClassifier.IsNavigationCommand(command));
    }

    [Theory]
    [InlineData("GO")]
    [InlineData("North")]
    [InlineData("MOVE")]
    public void IsNavigationCommand_CaseInsensitive_ReturnsTrue(string command)
    {
        Assert.True(CommandClassifier.IsNavigationCommand(command));
    }

    [Theory]
    [InlineData("go north")]
    [InlineData("move east")]
    public void IsNavigationCommand_VerbWithArgs_ReturnsTrue(string command)
    {
        Assert.True(CommandClassifier.IsNavigationCommand(command));
    }

    [Theory]
    [InlineData("look")]
    [InlineData("take sword")]
    [InlineData("attack goblin")]
    [InlineData("")]
    public void IsNavigationCommand_NonNavigationVerbs_ReturnsFalse(string command)
    {
        Assert.False(CommandClassifier.IsNavigationCommand(command));
    }
}
