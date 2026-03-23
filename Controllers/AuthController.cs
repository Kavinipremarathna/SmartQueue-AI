using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartQueueAPI.DTOs.Auth;
using SmartQueueAPI.Services.Interfaces;

namespace SmartQueueAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var token = await _authService.LoginAsync(request.Username, request.Password);
        if (token is null)
        {
            return Unauthorized("Invalid username or password.");
        }

        return Ok(token);
    }
}
