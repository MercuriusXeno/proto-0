namespace ProtoEngine.Commands;

public class CommandResult
{
    public bool Success { get; init; }
    public List<string> Output { get; init; } = new();
    public List<OutputLine> RichOutput { get; init; } = new();
    public bool RefreshRoomDescription { get; init; } = false; // Set to true when room state changes

    public static CommandResult Ok(params string[] lines)
        => new() { Success = true, Output = lines.ToList() };

    public static CommandResult Fail(params string[] lines)
        => new() { Success = false, Output = lines.ToList() };

    public static CommandResult OkRich(params OutputLine[] lines)
        => new() {
            Success = true,
            RichOutput = lines.ToList(),
            Output = lines.Select(l => l.Text).ToList() // Fallback
        };

    public static CommandResult OkRichWithRefresh(params OutputLine[] lines)
        => new() {
            Success = true,
            RichOutput = lines.ToList(),
            Output = lines.Select(l => l.Text).ToList(),
            RefreshRoomDescription = true
        };

    public static CommandResult FailRich(params OutputLine[] lines)
        => new() {
            Success = false,
            RichOutput = lines.ToList(),
            Output = lines.Select(l => l.Text).ToList()
        };
}
