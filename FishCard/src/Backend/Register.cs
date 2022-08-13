using System.Reflection;
using System.Text.Json;
using CodeWithSaar.FishCard.Biz;
using CodeWithSaar.FishCard.DataAccess;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CodeWithSaar.FishCard;

internal static class Register
{
    public static void AddFishServices(this IServiceCollection services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.TryAddSingleton<JsonSerializerOptions>(_ => CreateJsonSerializerOptions());

        services.AddTransient<IFishData>(p => new FishData(Path.Combine(GetExecutingAssemblyPath(), "fishdata.json"), p.GetRequiredService<JsonSerializerOptions>()));
        services.AddTransient<IBizFish, BizFish>();
    }

    private static JsonSerializerOptions CreateJsonSerializerOptions()
    {
        JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.PropertyNameCaseInsensitive = true;
        return options;
    }

    private static string GetExecutingAssemblyPath()
    {
        return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    }
}