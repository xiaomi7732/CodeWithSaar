
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JWTAuth.AspNetCore.WebAPI
{
    public interface IRoleValidationService
    {
        /// <summary>
        /// Get a list of roles the user is in and return the roles.
        /// </summary>
        Task<IEnumerable<string>> ValidateRolesAsync(UserInfo validUser);
    }
}
