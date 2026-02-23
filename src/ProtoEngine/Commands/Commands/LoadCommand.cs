namespace ProtoEngine.Commands.Commands;

public class LoadCommand : ICommand
{
    public string Verb => "load";
    public string[] Aliases => [];
    public string Description => "Load a saved game";

    public CommandResult Execute(CommandContext context, string[] args)
        => CommandResult.Ok("Game loaded.");
}
