using CodeWithSaar.FishCard.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeWithSaar.FishCard.Controllers;

[ApiController]
public class TokenController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;

    public TokenController(
        IUserService userService,
        ITokenService tokenService)
    {

        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    [Route("token")]
    [HttpPost]
    public async Task<IActionResult> GetToken([FromBody] User login, CancellationToken cancellationToken)
    {
        if (await _userService.IsValidUserAsync(login, cancellationToken))
        {
            string accessToken = await _tokenService.GetAccessTokenAsync(login, cancellationToken);
            return Ok(new
            {
                token = accessToken,
            });
        }
        return Forbid();
    }

#if DEBUG
    [Route("test")]
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult TestAuth()
    {
        return Ok("Succeeded!");
    }
#endif
}