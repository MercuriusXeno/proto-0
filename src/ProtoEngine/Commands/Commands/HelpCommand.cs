namespace ProtoEngine.Commands.Commands;

public class HelpCommand : ICommand
{
    private readonly CommandRegistry _registry;

    public string Verb => "help";
    public string[] Aliases => ["?", "commands"];
    public string Description => "Show available commands";

    public HelpCommand(CommandRegistry registry)
    {
        _registry = registry;
    }

    public CommandResult Execute(CommandContext context, string[] args)
    {
        var lines = new List<string>
        {
            "== Available Commands ==",
            ""
        };

        foreach (var cmd in _registry.AllCommands.OrderBy(c => c.Verb))
        {
            var aliases = cmd.Aliases.Length > 0
                ? $" (aliases: {string.Join(", ", cmd.Aliases)})"
                : "";
            lines.Add($"  {cmd.Verb,-12} {cmd.Description}{aliases}");
        }

        lines.Add("");
        lines.Add("Tip: You can also type direction names directly (north, south, etc.)");
        return CommandResult.Ok(lines.ToArray());
    }
}
