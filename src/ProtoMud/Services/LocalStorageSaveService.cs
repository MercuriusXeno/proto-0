using Blazored.LocalStorage;
using ProtoEngine.Persistence;

namespace ProtoMud.Services;

public class LocalStorageSaveService : ISaveLoadService
{
    private const string SaveKey = "proto0_save";
    private readonly ILocalStorageService _localStorage;

    public LocalStorageSaveService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task SaveAsync(SaveData data)
    {
        await _localStorage.SetItemAsync(SaveKey, data);
    }

    public async Task<SaveData?> LoadAsync()
    {
        try
        {
            return await _localStorage.GetItemAsync<SaveData>(SaveKey);
        }
        catch
        {
            return null;
        }
    }

    public Task ExportAsync(SaveData data)
    {
        // File export handled via JS interop in the Blazor component
        return Task.CompletedTask;
    }

    public Task<SaveData?> ImportAsync()
    {
        // File import handled via JS interop in the Blazor component
        return Task.FromResult<SaveData?>(null);
    }
}
