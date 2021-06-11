using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace JWTAuth.AspNetCore.WebAPI
{
    [Obsolete("Do NOT use this anymore. Use delegates on JWTAuthOptions.", error: true)]
    public class DefaultUserService : UserServiceBase<string>
    {
        private readonly JWTAuthOptions _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DefaultUserService(
            IServiceScopeFactory serviceScopeFactory,
            IOptions<JWTAuthOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        protected override Task<UserInfo> IsValidUserAsync(string login)
        {
            Func<string, IServiceProvider, Task<UserInfo>> validator =
                _options.OnValidateUserInfo ??
                ((login, p) => Task.FromResult<UserInfo>(null));
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            return _options.OnValidateUserInfo(login, scope.ServiceProvider);
        }

        // Let the user OnValidateUserInfo to handle the deserialization.
        protected override Task<string> DeserializeUserLoginAsync(string jsonText)
        {
            return Task.FromResult(jsonText);
        }

        public override Task<IEnumerable<string>> ValidateRolesAsync(UserInfo validUser)
        {
            Func<UserInfo, IServiceProvider, Task<IEnumerable<string>>> validator =
                _options.OnValidateRoleInfo ??
                ((userInfo,p) => Task.FromResult<IEnumerable<string>>(Enumerable.Empty<string>()));
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            return validator(validUser, scope.ServiceProvider);
        }
    }
}