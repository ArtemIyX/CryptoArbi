using ArbiBlazor;
using ArbiBlazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
string serverUrl = builder.Configuration["ServerUrl"] ?? "";
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(serverUrl) });
builder.Services.AddScoped<IAppSettingsService, AppSettingsService>();
builder.Services.AddScoped<IExchangeService, ExchangeService>();


await builder.Build().RunAsync();
