namespace SmartQueueAPI.Entities;

public class Appointment
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime SlotStartUtc { get; set; }
    public DateTime SlotEndUtc { get; set; }
    public string Status { get; set; } = AppointmentStatus.Booked;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
