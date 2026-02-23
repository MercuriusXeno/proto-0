using ProtoEngine.Systems;

namespace ProtoEngine.Commands.Commands;

public class CraftCommand : ICommand
{
    private readonly CraftingSystem _crafting;

    public string Verb => "craft";
    public string[] Aliases => ["make", "create"];
    public string Description => "Craft an item from a recipe (craft <recipe name>)";

    public CraftCommand(CraftingSystem crafting)
    {
        _crafting = crafting;
    }

    public CommandResult Execute(CommandContext context, string[] args)
    {
        if (args.Length == 0)
        {
            // List available recipes
            var recipes = _crafting.GetAvailableRecipes();
            if (recipes.Count == 0)
                return CommandResult.Ok("No recipes available.");

            var lines = new List<string> { "== Recipes ==" };
            foreach (var recipe in recipes)
                lines.Add($"  - {recipe.Name}: {recipe.Description}");
            lines.Add("");
            lines.Add("Usage: craft <recipe name>");
            return CommandResult.Ok(lines.ToArray());
        }

        var recipeName = string.Join(" ", args);
        if (_crafting.TryCraft(context.State, recipeName, out var message))
            return CommandResult.Ok(message);
        return CommandResult.Fail(message);
    }
}
