using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ProtoEngine.Persistence;
using ProtoMud;
using ProtoMud.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<ISaveLoadService, LocalStorageSaveService>();
builder.Services.AddScoped<GameSessionHost>();
builder.Services.AddScoped<FileExportService>();

await builder.Build().RunAsync();
