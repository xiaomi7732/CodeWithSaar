using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CodeWithSaar.FishCard.Auth;

public static class AuthServiceRegister
{
    public static IServiceCollection TryAddFishcardAuthServices(
        this IServiceCollection services, 
        string configurationSectionName="Auth")
    {
        services.AddOptions<AuthOptions>().Configure<IConfiguration>((opt, configure) =>{
            configure.GetSection(configurationSectionName).Bind(opt);
        });

        services.TryAddScoped<IUserService, UserService>();
        services.TryAddScoped<ITokenService, TokenService>();

        return services;
    }
}