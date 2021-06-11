using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using JWTAuth.AspNetCore.WebAPI;

namespace AuthExample.ConsumeNuGet
{
    internal class UserService
    {
        public Task<UserInfo> ValidUserAsync(string loginPayload)
        {
            DefaultUserLogin login = JsonSerializer.Deserialize<DefaultUserLogin>(loginPayload, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            if (login?.Username == "saar" || login?.Username == "user")
            {
                return Task.FromResult(new UserInfo(login.Username, login));
            }
            return Task.FromResult<UserInfo>(null);
        }

        public Task<IEnumerable<string>> ValidateRolesAsync(UserInfo validUser)
        {
            validUser.Roles = new[]{
                "User"
            };
            return Task.FromResult(validUser.Roles);
        }
    }
}