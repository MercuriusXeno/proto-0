namespace ProtoEngine.Commands;

public class CommandRegistry
{
    private readonly Dictionary<string, ICommand> _commands = new(StringComparer.OrdinalIgnoreCase);

    public void Register(ICommand command)
    {
        _commands[command.Verb] = command;
        foreach (var alias in command.Aliases)
            _commands[alias] = command;
    }

    public ICommand? Resolve(string verb)
        => _commands.TryGetValue(verb, out var cmd) ? cmd : null;

    public IEnumerable<ICommand> AllCommands
        => _commands.Values.DistinctBy(c => c.Verb);
}
