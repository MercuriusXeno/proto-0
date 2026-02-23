namespace ProtoEngine.Persistence;

public interface ISaveLoadService
{
    Task SaveAsync(SaveData data);
    Task<SaveData?> LoadAsync();
    Task ExportAsync(SaveData data);
    Task<SaveData?> ImportAsync();
}
