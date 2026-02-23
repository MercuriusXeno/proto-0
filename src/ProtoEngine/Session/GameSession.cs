using ProtoEngine.Commands;
using ProtoEngine.Commands.Commands;
using ProtoEngine.Core;
using ProtoEngine.Events;
using ProtoEngine.Systems;

namespace ProtoEngine.Session;

public class GameSession
{
    private readonly GameState _state;
    private readonly ICommandParser _parser;
    private readonly CommandRegistry _registry;
    private readonly CommandContext _context;
    private readonly List<ITickable> _tickables = new();
    private readonly List<IGameSystem> _systems = new();

    public GameState State => _state;
    public CommandRegistry Registry => _registry;

    public GameSession(GameState state, ICommandParser parser, CommandRegistry registry, IEventBus eventBus)
    {
        _state = state;
        _parser = parser;
        _registry = registry;
        _context = new CommandContext { State = state, EventBus = eventBus };
    }

    public void RegisterSystem(IGameSystem system)
    {
        _systems.Add(system);
        if (system is ITickable tickable)
            _tickables.Add(tickable);
    }

    public void Initialize()
    {
        foreach (var system in _systems)
            system.Initialize(_state);
        _state.IsInitialized = true;
    }

    public CommandResult ProcessCommand(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return CommandResult.Fail("Enter a command. Type 'help' for a list of commands.");

        var (verb, args) = _parser.Parse(input);
        var command = _registry.Resolve(verb);

        if (command is null)
            return CommandResult.Fail($"Unknown command: '{verb}'. Type 'help' for a list of commands.");

        // Pass original verb to MoveCommand for directional aliases
        var result = command is MoveCommand moveCmd
            ? moveCmd.ExecuteWithVerb(_context, args, verb)
            : command.Execute(_context, args);

        _state.Clock.Advance();
        foreach (var tickable in _tickables)
            tickable.Tick(_state, _state.Clock);

        return result;
    }
}
