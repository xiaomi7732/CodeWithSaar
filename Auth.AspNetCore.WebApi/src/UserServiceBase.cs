using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace JWTAuth.AspNetCore.WebAPI
{
    /// <summary>
    /// A user service extension point.
    /// </summary>
    /// <typeparam name="T">Type for user login that deserialized from the login request.</typeparam>
    public abstract class UserServiceBase<T> : IUserValidationService
    {
        public Task<IEnumerable<string>> GetUserRoles(UserInfo user)
        {
            throw new NotImplementedException();
        }

        public async Task<UserInfo> IsValidUserAsync(string requestStringContent)
        {
            try
            {
                T login = await DeserializeUserLoginAsync(requestStringContent).ConfigureAwait(false);
                UserInfo verifiedUser = await IsValidUserAsync(login).ConfigureAwait(false);
                await SetRolesAsync(login, verifiedUser).ConfigureAwait(false);

                return verifiedUser;
            }
            catch (System.Text.Json.JsonException)
            {
                throw new InvalidCastException();
            }
        }


        protected virtual Task<T> DeserializeUserLoginAsync(string jsonText)
            => Task.FromResult(JsonSerializer.Deserialize<T>(jsonText, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));

        /// <summary>
        /// Verify if the user is valid.
        /// </summary>
        /// <returns>Retruns UserInfo when it is. Otherwise, return null.</returns>
        protected abstract Task<UserInfo> IsValidUserAsync(T login);
        
        /// <summary>
        /// Set roles info onto the verifiedUserInfo.
        /// </summary>
        protected abstract Task SetRolesAsync(T login, UserInfo verifiedUserInfo);
    }
}