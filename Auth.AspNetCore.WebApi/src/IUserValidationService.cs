using System;
using System.Threading.Tasks;

namespace JWTAuth.AspNetCore.WebAPI
{
    [Obsolete("Do NOT use this anymore. Use delegates on JWTAuthOptions.", error: true)]
    public interface IUserValidationService
    {
        /// <summary>
        /// Verify the login info in the request body and return valid user info.
        /// Returns null if the user isn't valid.
        /// </summary>
        /// <param name="requestStringContent">Http request body.</param>
        /// <returns>The UserInfo when the user is valid.</returns>
        Task<UserInfo> ValidateUserAsync(string requestStringContent);
    }
}