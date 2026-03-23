using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartQueueAPI.Entities;
using SmartQueueAPI.Services.Interfaces;

namespace SmartQueueAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = RoleNames.AdminOrStaff)]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var analytics = await _analyticsService.GetAnalyticsAsync();
        return Ok(analytics);
    }
}
