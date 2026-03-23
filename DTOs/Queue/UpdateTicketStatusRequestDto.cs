using System.ComponentModel.DataAnnotations;

namespace SmartQueueAPI.DTOs.Queue;

public class UpdateTicketStatusRequestDto
{
    [Required]
    public string Status { get; set; } = string.Empty;
}
