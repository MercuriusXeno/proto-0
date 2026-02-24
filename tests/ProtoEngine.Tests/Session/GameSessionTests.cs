using ProtoEngine.Commands;
using ProtoEngine.Core;
using ProtoEngine.Events;
using ProtoEngine.Session;

namespace ProtoEngine.Tests.Session;

public class GameSessionTests
{
    private class EchoCommand : ICommand
    {
        public string Verb => "echo";
        public string[] Aliases => new[] { "e" };
        public string Description => "Echoes input back";
        public CommandResult Execute(CommandContext context, string[] args)
            => CommandResult.Ok(string.Join(" ", args));
    }

    private static GameSession CreateSession(params ICommand[] commands)
    {
        var state = new GameState();
        var parser = new CommandParser();
        var registry = new CommandRegistry();
        var eventBus = new EventBus();

        foreach (var cmd in commands)
            registry.Register(cmd);

        var session = new GameSession(state, parser, registry, eventBus);
        session.Initialize();
        return session;
    }

    [Fact]
    public void ProcessCommand_ValidCommand_ReturnsSuccess()
    {
        var session = CreateSession(new EchoCommand());

        var result = session.ProcessCommand("echo hello");

        Assert.True(result.Success);
        Assert.Contains("hello", result.Output);
    }

    [Fact]
    public void ProcessCommand_UnknownCommand_ReturnsFailure()
    {
        var session = CreateSession();

        var result = session.ProcessCommand("nonexistent");

        Assert.False(result.Success);
        Assert.Contains(result.Output, o => o.Contains("Unknown command"));
    }

    [Fact]
    public void ProcessCommand_EmptyInput_ReturnsFailure()
    {
        var session = CreateSession();

        var result = session.ProcessCommand("");

        Assert.False(result.Success);
    }

    [Fact]
    public void ProcessCommand_WhitespaceInput_ReturnsFailure()
    {
        var session = CreateSession();

        var result = session.ProcessCommand("   ");

        Assert.False(result.Success);
    }

    [Fact]
    public void ProcessCommand_AdvancesClock()
    {
        var session = CreateSession(new EchoCommand());

        session.ProcessCommand("echo test");

        Assert.Equal(1, session.State.Clock.Tick);
    }

    [Fact]
    public void ProcessCommand_ByAlias_Works()
    {
        var session = CreateSession(new EchoCommand());

        var result = session.ProcessCommand("e hello");

        Assert.True(result.Success);
    }
}
