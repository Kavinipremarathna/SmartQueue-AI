using System.ComponentModel.DataAnnotations;

namespace SmartQueueAPI.DTOs.Tickets;

public class CreateTicketRequestDto
{
    [Required]
    [MaxLength(80)]
    public string CustomerName { get; set; } = string.Empty;

    [Range(0, 10)]
    public int Priority { get; set; } = 1;
}
