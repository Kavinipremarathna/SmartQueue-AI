namespace SmartQueueAPI.DTOs.Queue;

public record QueueSummaryResponseDto(
    int Total,
    int Waiting,
    int Served,
    int Cancelled,
    int AverageWaitingPriority,
    int OldestWaitingMinutes);
