using SmartQueueAPI.DTOs.Tickets;

namespace SmartQueueAPI.DTOs.Queue;

public record QueueCurrentResponseDto(
    IReadOnlyList<TicketResponseDto> WaitingTickets,
    int TotalWaiting,
    int StaffCount,
    int AverageServiceMinutes,
    int PredictedWaitMinutes);
