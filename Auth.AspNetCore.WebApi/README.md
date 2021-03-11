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
        protected override async Task<UserInfo> IsValidUserAsync(DefaultUserLogin login)
        {
            // This is just an example with hard-coded values.
            // Check with database or other service to making sure the user info is valid.
            byte[] passwordHash = await GetHashedPasswordFromDBOrSomewhereElseAsync(login.Username).ConfigureAwait(false);
            if (passwordHash == null)
            {
                // Username doesn't match
                return null;
            }

            byte[] loginPasswordHash = GetPasswordHash(login.Password);
            if (!passwordHash.SequenceEqual(loginPasswordHash))
            {
                // Password doesn't match
                return null;
            }

            // Create UserInfo
            return new UserInfo(login.Username);
        }

        public override async Task ValidateRolesAsync(UserInfo userInfo)
        {
            userInfo.Roles = await GetRolesFromDBOrSomewhereElseAsync(userInfo.Name).ConfigureAwait(false);
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

## Run the example

* To get an access token:

  When run it in Postman by issue a `POST` to the `token/` route:

  Use **HTTP Method: POST** with a body:

  ```json
  {
    "username": "saar",
    "password": "123"
  }
  ```

  ![Run JWTAuth in Postman](./img/AuthInPostman.png)

* To access the protected resource, for example:

  * Copy the token above.

  * Use **HTTP Method: GET** on route: `/weatherforecast`, put the token copied to the `Bearer` authentication header:

  ![Get protected resource in Postman](./img/GetWeatherForecast.png)

## Customization

* Use a different endpoint for token:

    By default, the token is exposed on `/token`. It can be customized in [appsettings.json](./example/appsettings.json):

    ```jsonc
    {
      ...
      "JWTAuth": {
        "TokenPath": "/api/token"
      }
    }
    ```

    You can also customize those when adding JWTAuth in service:

    ```csharp
    services.AddJWTAuth(opt => opt.TokenPath="/api/token");
    ```

    Actually, there are various options that could be configured in the same way. Refer to [JWTAuthOptions.cs](./src/JWTAuthOptions.cs) for all of them.

    _Tips: Always setup `IssuerSigningSecret` in Production system to have a stable signing key. It is critical when you scale out your service._

* Use a different user credential (other than username/password)

  `DefaultUserLogin` is provided in the convenience for login requests that uses a username and a password. It could be customized:

  * Create a model class:

  ```csharp
  public class MyUserLogin
  {
      public string SharedKey {get; set;}
  }
  ```

  * Update the `UserService` class above to take `MyUserLogin`:

  ```csharp
  internal class UserService : UserServiceBase<MyUserLogin>
  ```

  * Making sure the client is POST with a proper body for authentication:

  ```json
  {
    "sharedKey": "what-ever-your-secret-is-that-will-pass-user-validation...",
  }
  ```
