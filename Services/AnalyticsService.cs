using SmartQueueAPI.DTOs.Analytics;
using SmartQueueAPI.Entities;
using SmartQueueAPI.Repositories;
using SmartQueueAPI.Services.Interfaces;

namespace SmartQueueAPI.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IQueueConfigurationRepository _configurationRepository;

    public AnalyticsService(
        ITicketRepository ticketRepository,
        IQueueConfigurationRepository configurationRepository)
    {
        _ticketRepository = ticketRepository;
        _configurationRepository = configurationRepository;
    }

    public async Task<AnalyticsResponseDto> GetAnalyticsAsync()
    {
        var tickets = await _ticketRepository.GetAllAsync();
        var config = await _configurationRepository.GetOrCreateAsync();

        var servedTickets = tickets.Where(t => t.Status == TicketStatus.Served && t.ServedAtUtc.HasValue).ToList();
        var waitingTickets = tickets.Where(t => t.Status == TicketStatus.Waiting).ToList();

        var averageWait = servedTickets.Count == 0
            ? 0
            : (int)Math.Round(servedTickets.Average(t => (t.ServedAtUtc!.Value - t.CreatedAtUtc).TotalMinutes));

        var peakHour = tickets.Count == 0
            ? DateTime.UtcNow.Hour
            : tickets
                .GroupBy(t => t.CreatedAtUtc.Hour)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .First();

        var finished = tickets.Count(t => t.Status == TicketStatus.Served || t.Status == TicketStatus.Cancelled);
        var efficiency = tickets.Count == 0 ? 0d : (double)finished / tickets.Count * 100d;

        return new AnalyticsResponseDto(
            tickets.Count,
            waitingTickets.Count,
            tickets.Count(t => t.Status == TicketStatus.Served),
            tickets.Count(t => t.Status == TicketStatus.Cancelled),
            averageWait,
            peakHour,
            Math.Round(efficiency, 2),
            config.StaffCount);
    }
}
