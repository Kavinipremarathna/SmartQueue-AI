using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartQueueAPI.DTOs.Appointments;
using SmartQueueAPI.Entities;
using SmartQueueAPI.Services.Interfaces;

namespace SmartQueueAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [Authorize(Roles = RoleNames.AllRoles)]
    [HttpPost]
    public async Task<IActionResult> Book([FromBody] CreateAppointmentRequestDto request)
    {
        var appointment = await _appointmentService.BookAsync(request);
        return Ok(appointment);
    }

    [Authorize(Roles = RoleNames.AdminOrStaff)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var appointments = await _appointmentService.GetAllAsync();
        return Ok(appointments);
    }
}
