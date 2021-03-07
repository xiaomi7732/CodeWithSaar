# JWTAuth.AspNetCore.WebAPI

Authentication implementation without identity server should still be simple in ASP.NET Core WebAPI. With this library, it is simple way to implement a JWT authentication.

## Get Started

1. Create a WebAPI using cli

    ```dotnetcli
    dotnet new webapi
    ```

1. Create a UserService, inherits `UserServiceBase<DefaultUserLogin>`, to verify user info like it in [UserService.cs](example/UserService.cs)

    ```csharp
    // User service to handle the Login created before 
    internal class UserService : UserServiceBase<DefaultUserLogin>
    {
        protected override Task<UserInfo> IsValidUserAsync(DefaultUserLogin login)
        {
            // This is just an example with hard-coded values.
            // Check with database or other service to making sure the user info is valid.
            if (!_inMemoryUserDB.TryGetValue(login.Username, out string passwordInDatabase))
            {
                // No username
                return null;
            }

            if (!string.Equals(login.Password, passwordInDatabase, StringComparison.Ordinal))
            {
                // Password doesn't match
                return null;
            }

            // Create UserInfo
            return Task.FromResult(new UserInfo()
            {
                Name = login.Username,
            });
        }

        protected override Task SetRolesAsync(DefaultUserLogin _, UserInfo verifiedUserInfo)
        {
            // This is optional if you don't want to support role based access control, return Task.CompletedTask in that case.
            // Query the database or other service to get the proper role info.
            verifiedUserInfo.Roles = _userRoleMapping
                .Where(mapping => string.Equals(mapping.userName, verifiedUserInfo.Name, StringComparison.OrdinalIgnoreCase))
                .Select(mapping => mapping.role);
            return Task.CompletedTask;
        }
    }
    ```

1. Register the services in [Startup.cs](./example/Startup.cs):

    ```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        // Register the user service created
        services.AddSingleton<IUserValidationService, UserService>();
        // Add service to support JWT authentication
        services.AddJWTAuth();
    }
    ...
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        ...
        // Add this step in front of UserAuthorization().
        app.UseJWTAuth();
        app.UseAuthorization();
        ...
    }
    ```

1. Add `Authorize` attribute to route that needs protection like [WeatherForecastController.cs](./example/Controllers/WeatherForecastController.cs):

```csharp
using Microsoft.AspNetCore.Authorization;
...
[Authorize(Roles = "User")]    // Requires access token with a role of 'User' on it.
public class WeatherForecastController : ControllerBase
...
```

1. To get an access token:

When run it in Postman by issue a `POST` to the `token/` route:

Use **HTTP Method: POST** with a body:

```json
{
    "username": "saar",
    "password": "123"
}
```

![Run JWTAuth in Postman](./img/AuthInPostman.png)

1. To access the protected resource, for example:

* Copy the token above.

* Use **HTTP Method: GET** on route: `/weatherforecast`, put the token copied to the `Bearer` authentication header:

