
using System.Threading.Tasks;

namespace JWTAuth.AspNetCore.WebAPI
{
    public interface IRoleValidationService
    {
        Task ValidateRolesAsync(UserInfo validUser);
    }
}
