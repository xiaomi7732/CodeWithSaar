using System.Threading.Tasks;
using JWTAuth.AspNetCore.WebAPI;

namespace AuthExample.ConsumeNuGet
{
    internal class UserService : UserServiceBase<DefaultUserLogin>, IUserValidationService
    {
        protected override Task<UserInfo> IsValidUserAsync(DefaultUserLogin login)
        {
            if (login?.Username == "saar")
            {
                return Task.FromResult(new UserInfo()
                {
                    Name = login.Username,
                });
            }
            return Task.FromResult((UserInfo)null);
        }

        protected override Task SetRolesAsync(DefaultUserLogin login, UserInfo verifiedUserInfo)
        {
            return Task.CompletedTask;
        }
    }
}