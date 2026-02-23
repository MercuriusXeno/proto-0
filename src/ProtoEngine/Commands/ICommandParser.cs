namespace ProtoEngine.Commands;

public interface ICommandParser
{
    (string verb, string[] args) Parse(string input);
}
