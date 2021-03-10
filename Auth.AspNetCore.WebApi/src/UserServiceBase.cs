using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace JWTAuth.AspNetCore.WebAPI
{
    /// <summary>
    /// A user service extension point.
    /// </summary>
    /// <typeparam name="T">Type for user login that deserialized from the login request.</typeparam>
    public abstract class UserServiceBase<T> : IUserValidationService, IRoleValidationService
    {
        public async Task<UserInfo> ValidateUserAsync(string requestStringContent)
        {
            try
            {
                T login = await DeserializeUserLoginAsync(requestStringContent).ConfigureAwait(false);

                UserInfo verifiedUser = await IsValidUserAsync(login).ConfigureAwait(false);
                return verifiedUser;
            }
            catch (System.Text.Json.JsonException)
            {
                throw new InvalidCastException();
            }
        }

        /// <summary>
        /// Updates the user info with role information.
        /// </summary>
        /// <param name="validUser">A valid UserInfo instance.</param>
        public abstract Task ValidateRolesAsync(UserInfo validUser);

        /// <summary>
        /// Verify if the user is valid.
        /// </summary>
        /// <returns>Retruns a new UserInfo when it is. Otherwise, return null.</returns>
        protected abstract Task<UserInfo> IsValidUserAsync(T login);

        protected virtual Task<T> DeserializeUserLoginAsync(string jsonText)
            => Task.FromResult(JsonSerializer.Deserialize<T>(jsonText, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
    }
}