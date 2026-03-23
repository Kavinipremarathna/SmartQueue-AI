using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartQueueAPI.DTOs.Queue;
using SmartQueueAPI.DTOs.Tickets;
using SmartQueueAPI.Entities;
using SmartQueueAPI.Services.Interfaces;

namespace SmartQueueAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly IQueueService _queueService;

    public TicketsController(IQueueService queueService)
    {
        _queueService = queueService;
    }

    [Authorize(Roles = RoleNames.AllRoles)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTicketRequestDto request)
    {
        var created = await _queueService.CreateTicketAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    [Authorize(Roles = RoleNames.AdminOrStaff)]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTicketStatusRequestDto request)
    {
        var updated = await _queueService.UpdateStatusAsync(id, request.Status);
        if (updated is null)
        {
            return NotFound("Ticket not found or invalid status.");
        }

        return Ok(updated);
    }

    [Authorize(Roles = RoleNames.AdminOrStaff)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _queueService.DeleteTicketAsync(id);
        if (!deleted)
        {
            return NotFound("Ticket not found.");
        }

        return NoContent();
    }

    [Authorize(Roles = RoleNames.AdminOrStaff)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tickets = await _queueService.GetAllTicketsAsync();
        return Ok(tickets);
    }
}
