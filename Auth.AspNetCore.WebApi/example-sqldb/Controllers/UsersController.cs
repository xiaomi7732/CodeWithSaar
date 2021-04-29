using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
                // Log it here.
                return StatusCode(500, "Internal Server Error.");
            }
        }
    }
}