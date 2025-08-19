using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using WarehouseManagement.Blazor;
using WarehouseManagement.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("https://localhost:7058/")  
});

builder.Services.AddMudServices();

builder.Services.AddScoped<IApiClient, ApiClient>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();

await builder.Build().RunAsync();