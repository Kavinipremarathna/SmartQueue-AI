using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartQueueAPI.Entities;
using SmartQueueAPI.Services.Interfaces;

namespace SmartQueueAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QueueController : ControllerBase
{
    private readonly IQueueService _queueService;

    public QueueController(IQueueService queueService)
    {
        _queueService = queueService;
    }

    [Authorize(Roles = RoleNames.AllRoles)]
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent()
    {
        var response = await _queueService.GetCurrentQueueAsync();
        return Ok(response);
    }

    [Authorize(Roles = RoleNames.AdminOrStaff)]
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var response = await _queueService.GetAllTicketsAsync();
        return Ok(response);
    }

    [Authorize(Roles = RoleNames.AdminOrStaff)]
    [HttpPost("serve-next")]
    public async Task<IActionResult> ServeNext()
    {
        var served = await _queueService.ServeNextAsync();
        if (served is null)
        {
            return NotFound("No waiting ticket found.");
        }

        return Ok(served);
    }

    [Authorize(Roles = RoleNames.AdminOrStaff)]
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await _queueService.GetSummaryAsync();
        return Ok(summary);
    }
}