using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace JWTAuth.AspNetCore.WebAPI
{
    /// <summary>
    /// A user service extension point.
    /// </summary>
    /// <typeparam name="T">Type for user login that deserialized from the login request.</typeparam>
    [Obsolete("Do NOT use this anymore. Use delegates on JWTAuthOptions.", error: true)]
    public abstract class UserServiceBase<T> : IUserValidationService, IRoleValidationService
        where T : class
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
        /// Get the list of roles of the user and return the roles.
        /// </summary>
        /// <param name="validUser">A valid UserInfo instance.</param>
        /// <returns>The list of the roles.</returns>
        public virtual Task<IEnumerable<string>> ValidateRolesAsync(UserInfo validUser)
            => Task.FromResult(Enumerable.Empty<string>());

        /// <summary>
        /// Verify if the user is valid.
        /// </summary>
        /// <returns>Retruns a new UserInfo when it is. Otherwise, return null.</returns>
        protected abstract Task<UserInfo> IsValidUserAsync(T login);

        protected virtual Task<T> DeserializeUserLoginAsync(string jsonText)
            => Task.FromResult(JsonSerializer.Deserialize<T>(jsonText, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));

    }
}