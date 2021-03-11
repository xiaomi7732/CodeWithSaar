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
                return Task.FromResult(new UserInfo()
                {
                    Name = login.Username,
                });
            }
            return Task.FromResult((UserInfo)null);
        }

        public override Task ValidateRolesAsync(UserInfo validUser)
        {
            validUser.Roles = new []{
                "User"
            };
            return Task.CompletedTask;
        }
    }
}