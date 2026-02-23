namespace ProtoEngine.Data;

public class ContentManifest
{
    public List<RoomData> Rooms { get; set; } = new();
    public List<ItemData> Items { get; set; } = new();
    public List<NpcData> Npcs { get; set; } = new();
    public List<QuestData> Quests { get; set; } = new();
    public List<RecipeData> Recipes { get; set; } = new();
    public List<DialogueData> Dialogues { get; set; } = new();
    public string StartingRoomId { get; set; } = string.Empty;
}
