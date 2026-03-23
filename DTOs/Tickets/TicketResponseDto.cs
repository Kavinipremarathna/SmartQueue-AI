namespace SmartQueueAPI.DTOs.Tickets;

public record TicketResponseDto(
    int Id,
    string CustomerName,
    string Status,
    int Priority,
    int EstimatedWaitMinutes,
    DateTime CreatedAtUtc,
    DateTime? ServedAtUtc,
    int? AppointmentId);
