using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Frontend;
using CodeWithSaar.FishCard;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<QueryFishService>();
builder.Services.AddScoped<RecommendService>();
builder.Services.AddScoped<BackendClient>(p =>
{
    HttpClient httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri("https://fishcard.azurewebsites.net/");

    return new BackendClient(httpClient);
});

await builder.Build().RunAsync();