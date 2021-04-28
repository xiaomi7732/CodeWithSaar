using System.Collections.Generic;
using System.Threading.Tasks;
using JWTAuth.AspNetCore.WebAPI;

namespace AuthExample.ConsumeNuGet
{
    internal class UserService : UserServiceBase<DefaultUserLogin>
    {
        protected override Task<UserInfo> IsValidUserAsync(DefaultUserLogin login)
        {
            if (login?.Username == "saar")
            {
                return Task.FromResult(new UserInfo(login.Username, login));
            }
            return Task.FromResult<UserInfo>(null);
        }

        public override Task<IEnumerable<string>> ValidateRolesAsync(UserInfo validUser)
        {
            validUser.Roles = new[]{
                "User"
            };
            return Task.FromResult(validUser.Roles);
        }
    }
}