namespace ProtoEngine.Commands;

/// <summary>
/// Classifies commands by type for UI routing decisions.
/// </summary>
public static class CommandClassifier
{
    private static readonly HashSet<string> LookVerbs = new(StringComparer.OrdinalIgnoreCase)
    {
        "look", "l"
    };

    private static readonly HashSet<string> NavigationVerbs = new(StringComparer.OrdinalIgnoreCase)
    {
        "go", "move", "walk",
        "north", "south", "east", "west", "up", "down",
        "n", "s", "e", "w", "u", "d"
    };

    /// <summary>
    /// Returns true if the input is a look/examine command that should update the room description panel.
    /// </summary>
    public static bool IsLookCommand(string command)
    {
        var verb = ExtractVerb(command);
        return LookVerbs.Contains(verb);
    }

    /// <summary>
    /// Returns true if the input is a navigation command that moves the player between rooms.
    /// </summary>
    public static bool IsNavigationCommand(string command)
    {
        var verb = ExtractVerb(command);
        return NavigationVerbs.Contains(verb);
    }

    private static string ExtractVerb(string command)
    {
        var trimmed = command.Trim();
        var spaceIndex = trimmed.IndexOf(' ');
        return spaceIndex >= 0 ? trimmed[..spaceIndex] : trimmed;
    }
}
