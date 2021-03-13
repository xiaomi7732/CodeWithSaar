# Deep Clone with Serailizer

## Description

Deep clone could be tedious. Refer to the example in [MemberwiseClone](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone?view=net-5.0). Serialize the object and deserialize it back is an easy way to do it.

```csharp
using System.Text.Json;
...
public Product DeepClone()
    => JsonSerializer.Deserialize<Product>(JsonSerializer.Serialize(this, this.GetType()));
```
