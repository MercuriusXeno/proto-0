namespace ProtoEngine.Commands.Commands;

public class SaveCommand : ICommand
{
    public string Verb => "save";
    public string[] Aliases => [];
    public string Description => "Save your game progress";

    // Actual saving is handled by the Blazor service layer.
    // This command signals intent; the host intercepts it.
    public CommandResult Execute(CommandContext context, string[] args)
        => CommandResult.Ok("Game saved.");
}
