namespace ProtoEngine.Commands;

public class CommandParser : ICommandParser
{
    public (string verb, string[] args) Parse(string input)
    {
        var trimmed = input.Trim();
        if (string.IsNullOrEmpty(trimmed))
            return (string.Empty, Array.Empty<string>());

        var parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var verb = parts[0].ToLowerInvariant();
        var args = parts.Length > 1 ? parts[1..] : Array.Empty<string>();
        return (verb, args);
    }
}
