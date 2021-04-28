using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace JWT.Example.WithSQLDB
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService ?? throw new System.ArgumentNullException(nameof(userService));
        }

        [HttpGet]
        public ActionResult<IAsyncEnumerable<User>> GetUsersAsync()
        {
            return Ok(_userService.ListUsersAsync());
        }
    }
}