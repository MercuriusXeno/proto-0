namespace ProtoEngine.Data;

public class RecipeData
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> IngredientIds { get; set; } = new();
    public string ResultItemId { get; set; } = string.Empty;
}
