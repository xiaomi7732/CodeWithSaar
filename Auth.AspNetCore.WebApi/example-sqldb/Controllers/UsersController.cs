using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JWT.Example.WithSQLDB
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            UserService userService,
            ILogger<UsersController> logger)
        {
            _userService = userService ?? throw new System.ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [Route("")]
        public ActionResult<IAsyncEnumerable<User>> GetUsersAsync()
        {
            return Ok(_userService.ListUsersAsync());
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<User> GetUser(Guid id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult> CreateUserAsync([FromBody] NewUserInfo newUserInfo)
        {
            if (newUserInfo is null)
            {
                return BadRequest("User info is not provided.");
            }

            if (string.IsNullOrEmpty(newUserInfo.UserName))
            {
                return BadRequest("User name is not provided.");
            }

            if (string.IsNullOrEmpty(newUserInfo.Password))
            {
                return BadRequest("Password is not provided.");
            }

            try
            {
                User newUser = await _userService.AddUserAsync(newUserInfo.UserName, newUserInfo.Password).ConfigureAwait(false);
                return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUserInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create user login. {newUserInfo}", newUserInfo);
                // Log it here.
                return StatusCode(500, "Internal Server Error.");
            }
        }
    }
}