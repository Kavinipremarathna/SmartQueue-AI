using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartQueueAPI.DTOs.Auth;
using SmartQueueAPI.Services.Interfaces;

namespace SmartQueueAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private const string LegacyLoginPath = "/api/auth/login";

    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("token")]
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        SetLegacyLoginDeprecationHeaders();

        var token = await _authService.LoginAsync(request.Username, request.Password);
        if (token is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Authentication failed",
                Detail = "Invalid username or password."
            });
        }

        return Ok(token);
    }

    private void SetLegacyLoginDeprecationHeaders()
    {
        if (!Request.Path.Equals(LegacyLoginPath, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        Response.Headers.Append("Deprecation", "true");
        Response.Headers.Append("Sunset", "Tue, 30 Jun 2026 23:59:59 GMT");
        Response.Headers.Append("Link", "</api/auth/token>; rel=\"successor-version\"");
    }
}
