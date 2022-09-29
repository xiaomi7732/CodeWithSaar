using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CodeWithSaar.FishCard;
using CodeWithSaar.FishCard.Auth;
using Blazored.LocalStorage;
using System.Text.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient(HttpClientName.Local, (_, c) =>
{
    c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

builder.Services.AddHttpClient(HttpClientName.Backend, (_, c) =>
{
    c.BaseAddress = new Uri("https://localhost:7280");
});

// this is for backwards compatibility
builder.Services.AddScoped<HttpClient>(provider =>
{
    IHttpClientFactory factory = provider.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("local");
});

builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
builder.Services.AddBlazoredLocalStorageAsSingleton();

builder.Services.AddScoped<BackendClient>();
builder.Services.AddScoped<QueryFishService>();
builder.Services.AddScoped<RecommendService>();

builder.Services.AddSingleton<JsonSerializerOptions>(_ =>
{
    JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    return jsonSerializerOptions;
});

await builder.Build().RunAsync();
