using System.ComponentModel.DataAnnotations;

namespace SmartQueueAPI.DTOs.Appointments;

public class CreateAppointmentRequestDto
{
    [Required]
    [MaxLength(80)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    public DateTime SlotStartUtc { get; set; }

    [Range(10, 120)]
    public int DurationMinutes { get; set; } = 20;
}
