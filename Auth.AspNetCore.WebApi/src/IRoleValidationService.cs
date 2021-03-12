
using System.Threading.Tasks;

namespace JWTAuth.AspNetCore.WebAPI
{
    public interface IRoleValidationService
    {
        Task<bool> ValidateRolesAsync(UserInfo validUser);
    }
}
