# Configuration Providers Cheat Sheet

## Official Docs

* [Provider list](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0#configuration-providers)

## Code to inspect configurations in IConfiguration object:

```csharp
foreach ((string key, string value) 
    in configuration.AsEnumerable().Where(t => t.Value is not null))
{
    Console.WriteLine($"{key}={value}");
}
```

* Common Providers info

Name | Package Name | Register Method | Remarks
--|--|--|--
Json Configuration Provider | Microsoft.Extensions.Configuration.Json | config.AddJsonFile() | -
Environment Variables Provider | Microsoft.Extensions.Configuration.EnvironmentVariables | config.AddEnvironmentVariables() | Section__Key = Value
Command-line Arguments Provider | Microsoft.Extensions.Configuration.CommandLine | config.AddCommandLine() |  Allows switch to key mapping
User Secrets Providers |  Microsoft.Extensions.Configuration.UserSecrets() | config.AddUserSecrets() | dotnet user-secrets set "Section:Key" "12345"
