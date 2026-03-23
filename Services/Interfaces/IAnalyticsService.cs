using SmartQueueAPI.DTOs.Analytics;

namespace SmartQueueAPI.Services.Interfaces;

public interface IAnalyticsService
{
    Task<AnalyticsResponseDto> GetAnalyticsAsync();
}
