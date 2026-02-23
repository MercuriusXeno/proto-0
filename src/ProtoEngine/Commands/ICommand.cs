namespace ProtoEngine.Commands;

public interface ICommand
{
    string Verb { get; }
    string[] Aliases { get; }
    string Description { get; }
    CommandResult Execute(CommandContext context, string[] args);
}
