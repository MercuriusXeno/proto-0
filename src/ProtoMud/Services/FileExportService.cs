using System.Text.Json;
using Microsoft.JSInterop;
using ProtoEngine.Persistence;

namespace ProtoMud.Services;

public class FileExportService
{
    private readonly IJSRuntime _js;

    public FileExportService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task ExportSaveAsync(SaveData data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        await _js.InvokeVoidAsync("downloadFile", "proto0_save.json", Convert.ToBase64String(bytes));
    }
}
