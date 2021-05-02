using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JWT.Example.WithSQLDB
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly RoleService _roleService;
        private readonly ILogger _logger;

        public RolesController(
            RoleService roleService,
            ILogger<RolesController> logger)
        {
            _roleService = roleService ?? throw new System.ArgumentNullException(nameof(roleService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [Route("")]
        public ActionResult<IAsyncEnumerable<Role>> GetRoles()
        {
            try
            {
                return Ok(_roleService.ListRoles());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error listing roles.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<Role> GetRole(Guid id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult> CreateRole([FromBody] string roleName)
        {
            try
            {
                Role newRole = await _roleService.AddRoleAsync(roleName).ConfigureAwait(false);
                return CreatedAtAction(nameof(GetRole), new { id = newRole.Id }, newRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role of {roleName}", roleName);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        [Route("{roleId}/users/{userId}")]
        public async Task<ActionResult> CreateRoleAssignment(Guid roleId, Guid userId)
        {
            try
            {
                await _roleService.AddRoleAssignmentAsync(userId, roleId).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed adding role assignment.");
                return Conflict();
            }
        }
    }
}