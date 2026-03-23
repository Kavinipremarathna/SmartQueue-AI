namespace SmartQueueAPI.Entities;

public class Ticket
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = TicketStatus.Waiting;
    public int Priority { get; set; }
    public int EstimatedWaitMinutes { get; set; }
    public DateTime? ServedAtUtc { get; set; }
    public int? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }
}
