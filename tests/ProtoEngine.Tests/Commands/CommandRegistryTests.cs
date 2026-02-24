using ProtoEngine.Commands;

namespace ProtoEngine.Tests.Commands;

public class CommandRegistryTests
{
    private class StubCommand : ICommand
    {
        public string Verb { get; init; } = "test";
        public string[] Aliases { get; init; } = Array.Empty<string>();
        public string Description => "A test command";
        public CommandResult Execute(CommandContext context, string[] args) => CommandResult.Ok("done");
    }

    [Fact]
    public void Resolve_RegisteredVerb_ReturnsCommand()
    {
        var registry = new CommandRegistry();
        var command = new StubCommand { Verb = "look" };
        registry.Register(command);

        var resolved = registry.Resolve("look");

        Assert.Same(command, resolved);
    }

    [Fact]
    public void Resolve_UnregisteredVerb_ReturnsNull()
    {
        var registry = new CommandRegistry();

        var resolved = registry.Resolve("nonexistent");

        Assert.Null(resolved);
    }

    [Fact]
    public void Resolve_ByAlias_ReturnsCommand()
    {
        var registry = new CommandRegistry();
        var command = new StubCommand { Verb = "look", Aliases = new[] { "l", "examine" } };
        registry.Register(command);

        Assert.Same(command, registry.Resolve("l"));
        Assert.Same(command, registry.Resolve("examine"));
    }

    [Fact]
    public void Resolve_CaseInsensitive()
    {
        var registry = new CommandRegistry();
        var command = new StubCommand { Verb = "look" };
        registry.Register(command);

        Assert.Same(command, registry.Resolve("LOOK"));
        Assert.Same(command, registry.Resolve("Look"));
    }

    [Fact]
    public void AllCommands_ReturnsDistinctCommands()
    {
        var registry = new CommandRegistry();
        var cmd1 = new StubCommand { Verb = "look", Aliases = new[] { "l" } };
        var cmd2 = new StubCommand { Verb = "take", Aliases = new[] { "get" } };
        registry.Register(cmd1);
        registry.Register(cmd2);

        var all = registry.AllCommands.ToList();

        Assert.Equal(2, all.Count);
    }
}
