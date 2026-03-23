namespace SmartQueueAPI.DTOs.Analytics;

public record AnalyticsResponseDto(
    int TotalTickets,
    int WaitingTickets,
    int ServedTickets,
    int CancelledTickets,
    int AverageWaitMinutes,
    int PeakHour,
    double ServiceEfficiencyPercent,
    int StaffCount);
