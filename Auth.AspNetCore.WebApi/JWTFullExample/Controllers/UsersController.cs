using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JWT.Example.WithSQLDB
{
    [Authorize(Roles="Admin")]
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
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            try
            {
                User target = await _userService.GetUserByIdAsync(id, user => user.Include(u => u.Roles)).ConfigureAwait(false);
                return Ok(target);
            }
            catch (InvalidOperationException ex) when (ex.Message.StartsWith("Sequence contains no elements", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogError(ex, "User by id doesn't exist.");
                return NotFound(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed fetch the user.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult> CreateUserAsync([FromBody] UserRegisterModel newUserInfo)
        {
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