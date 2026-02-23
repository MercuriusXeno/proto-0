namespace ProtoEngine.Commands;

public class CommandResult
{
    public bool Success { get; init; }
    public List<string> Output { get; init; } = new();

    public static CommandResult Ok(params string[] lines)
        => new() { Success = true, Output = lines.ToList() };

    public static CommandResult Fail(params string[] lines)
        => new() { Success = false, Output = lines.ToList() };
}
