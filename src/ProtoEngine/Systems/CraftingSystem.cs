using ProtoEngine.Components;
using ProtoEngine.Core;
using ProtoEngine.Data;
using ProtoEngine.Events;

namespace ProtoEngine.Systems;

public class CraftingSystem : IGameSystem
{
    private readonly ContentManifest _content;
    private readonly IEventBus _eventBus;

    public CraftingSystem(ContentManifest content, IEventBus eventBus)
    {
        _content = content;
        _eventBus = eventBus;
    }

    public void Initialize(GameState state) { }

    public bool TryCraft(GameState state, string recipeName, out string message)
    {
        message = string.Empty;
        var recipe = _content.Recipes.FirstOrDefault(r =>
            r.Name.Equals(recipeName, StringComparison.OrdinalIgnoreCase));
        if (recipe is null)
        {
            message = $"Unknown recipe: '{recipeName}'. Type 'craft' to see available recipes.";
            return false;
        }

        var inventory = state.Player.Get<InventoryComponent>();
        if (inventory is null)
        {
            message = "You can't craft anything.";
            return false;
        }

        // Check ingredients
        var missingItems = new List<string>();
        foreach (var ingredientId in recipe.IngredientIds)
        {
            if (!inventory.ItemIds.Contains(ingredientId))
            {
                var itemData = _content.Items.FirstOrDefault(i => i.Id == ingredientId);
                missingItems.Add(itemData?.Name ?? ingredientId);
            }
        }

        if (missingItems.Count > 0)
        {
            message = $"Missing ingredients: {string.Join(", ", missingItems)}";
            return false;
        }

        if (inventory.IsFull)
        {
            message = "Your inventory is full.";
            return false;
        }

        // Remove ingredients and add result
        foreach (var ingredientId in recipe.IngredientIds)
            inventory.ItemIds.Remove(ingredientId);
        inventory.ItemIds.Add(recipe.ResultItemId);

        var resultItem = _content.Items.FirstOrDefault(i => i.Id == recipe.ResultItemId);
        _eventBus.Publish(new ItemCraftedEvent(recipe.Id));
        message = $"You crafted: {resultItem?.Name ?? recipe.ResultItemId}!";
        return true;
    }

    public List<RecipeData> GetAvailableRecipes() => _content.Recipes;
}
