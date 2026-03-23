using Microsoft.AspNetCore.Mvc;
using SmartQueueAPI.Services;

namespace SmartQueueAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueueController : ControllerBase
    {
        private readonly QueueService _service;

        public QueueController(QueueService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetQueue()
        {
            var queue = await _service.GetQueue();
            return Ok(queue);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var tickets = await _service.GetAllTickets();
            return Ok(tickets);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var summary = await _service.GetSummary();
            return Ok(summary);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromQuery] string name, [FromQuery] int priority)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Name is required.");
            }

            var ticket = await _service.AddTicket(name, priority);
            return Ok(ticket);
        }

        [HttpPost("serve-next")]
        public async Task<IActionResult> ServeNext()
        {
            var served = await _service.ServeNextTicket();
            if (served is null)
            {
                return NotFound("No waiting ticket found.");
            }

            return Ok(served);
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return BadRequest("Status is required.");
            }

            var updated = await _service.UpdateTicketStatus(id, status);
            if (updated is null)
            {
                return NotFound("Ticket not found or invalid status.");
            }

            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var removed = await _service.DeleteTicket(id);
            if (!removed)
            {
                return NotFound("Ticket not found.");
            }

            return NoContent();
        }
    }
}