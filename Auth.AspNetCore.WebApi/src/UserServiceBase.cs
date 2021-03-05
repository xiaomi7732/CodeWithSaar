using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace JWTAuth.AspNetCore.WebAPI
{
    public abstract class UserServiceBase<T> : IUserService
    {
        public Task<UserInfo> IsValidUserAsync(string requestStringContent)
        {
            try
            {
                T login = JsonSerializer.Deserialize<T>(requestStringContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return IsValidUserAsync(login);
            }
            catch (System.Text.Json.JsonException)
            {
                throw new InvalidCastException();
            }
        }

        protected abstract Task<UserInfo> IsValidUserAsync(T login);
    }
}