# JWTAuth.AspNetCore.WebAPI

Authentication implementation without identity server should still be simple in ASP.NET Core WebAPI. We support simple ways to do a basic authentication.

## Get Started

1. Create a WebAPI using cli

    ```dotnetcli
    dotnet new webapi
    ```

1. Build a class to take login info, for example, if you want the login request body look like this:

    ```json
    {
        "userName": "user",
        "password": "123"
    }
    ```

    Create a class like [this](example/Login.cs):

    ```csharp
    public class Login
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    ```

1. Create a UserService to verify user info like it in [UserService.cs](example/UserService.cs)

    ```csharp
    // User service to handle the Login created before 
    internal class UserService : UserServiceBase<Login>
    {
        protected override Task<UserInfo> IsValidUserAsync(Login login)
        {
            // This is just an example with hard-coded values.
            // Check with database or other service to making sure the user info is valid.
            if (string.Equals(login.UserName, "user", StringComparison.OrdinalIgnoreCase)
                && string.Equals(login.Password, "123", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(new UserInfo()
                {
                    Name = "saars",
                    // Also query the database or other service to get the proper role info.
                    // This is optional if you don't want to support role based access control.
                    Roles = new[] { "User", "Admin" },
                });
            }
            return Task.FromResult((UserInfo)default);
        }
    }
    ```

1. Register the services in [Startup.cs](./example/Startup.cs):

    ```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        // Register the user service created
        services.AddSingleton<IUserService, UserService>();
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

That is it. When run it in Postman:

![Run JWTAuth in Postman](./img/AuthInPostman.png)
