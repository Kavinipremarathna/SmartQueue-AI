using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartQueueAPI.DTOs.Admin;
using SmartQueueAPI.Entities;
using SmartQueueAPI.Services.Interfaces;

namespace SmartQueueAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = RoleNames.Admin)]
public class AdminController : ControllerBase
{
    private readonly IQueueConfigurationService _configurationService;
    private readonly IQueueService _queueService;

    public AdminController(IQueueConfigurationService configurationService, IQueueService queueService)
    {
        _configurationService = configurationService;
        _queueService = queueService;
    }

    [HttpPost("staff-allocation")]
    public async Task<IActionResult> UpdateStaffAllocation([FromBody] UpdateStaffAllocationRequestDto request)
    {
        var updated = await _configurationService.UpdateStaffCountAsync(request.StaffCount);
        return Ok(updated);
    }

    [HttpGet("staff-allocation")]
    public async Task<IActionResult> GetStaffAllocation()
    {
        var config = await _configurationService.GetAsync();
        return Ok(config);
    }

    [HttpGet("live-queue")]
    public async Task<IActionResult> LiveQueue()
    {
        var queue = await _queueService.GetCurrentQueueAsync();
        return Ok(queue);
    }
}
