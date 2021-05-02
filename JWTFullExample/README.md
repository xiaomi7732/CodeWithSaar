# JWT Web API End to End Example

## Description

Build a ASP.NET Core WebAPI with JWT Authentication, from database design up to API implementations and run it end to end with Postman.

This is the code example for YouTube video: [JWT Authentication End to End example](https://youtu.be/tLgdbTqQp7Q) (available on 5/3/2021).

## To Create the database

```.NET CLI
dotnet tool install --global dotnet-ef
dotnet ef database update
```

## To generate another migration when changed

```.NET CLI
dotnet ef migrations add InitialCreate
```

Refer to [Getting Started with EF Core](https://docs.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli).
