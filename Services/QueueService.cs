using SmartQueueAPI.DTOs.Queue;
using SmartQueueAPI.DTOs.Tickets;
using SmartQueueAPI.Entities;
using SmartQueueAPI.Repositories;
using SmartQueueAPI.Services.Interfaces;

namespace SmartQueueAPI.Services;

public class QueueService : IQueueService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IQueueConfigurationRepository _configurationRepository;
    private readonly IQueueNotifier _queueNotifier;

    public QueueService(
        ITicketRepository ticketRepository,
        IQueueConfigurationRepository configurationRepository,
        IQueueNotifier queueNotifier)
    {
        _ticketRepository = ticketRepository;
        _configurationRepository = configurationRepository;
        _queueNotifier = queueNotifier;
    }

    public async Task<TicketResponseDto> CreateTicketAsync(CreateTicketRequestDto request)
    {
        var ticket = new Ticket
        {
            CustomerName = request.CustomerName.Trim(),
            Priority = Math.Clamp(request.Priority, 0, 10),
            Status = TicketStatus.Waiting
        };

        var waitingCount = (await _ticketRepository.GetWaitingAsync()).Count;
        var config = await _configurationRepository.GetOrCreateAsync();
        ticket.EstimatedWaitMinutes = PredictWait(waitingCount + 1, config.AverageServiceMinutes, config.StaffCount);

        var saved = await _ticketRepository.AddAsync(ticket);
        await _queueNotifier.BroadcastQueueUpdatedAsync();

        return Map(saved);
    }

    public async Task<QueueCurrentResponseDto> GetCurrentQueueAsync()
    {
        var waiting = await _ticketRepository.GetWaitingAsync();
        var config = await _configurationRepository.GetOrCreateAsync();
        var predicted = PredictWait(waiting.Count, config.AverageServiceMinutes, config.StaffCount);

        return new QueueCurrentResponseDto(
            waiting.Select(Map).ToList(),
            waiting.Count,
            config.StaffCount,
            config.AverageServiceMinutes,
            predicted);
    }

    public async Task<IReadOnlyList<TicketResponseDto>> GetAllTicketsAsync()
    {
        var tickets = await _ticketRepository.GetAllAsync();
        return tickets.Select(Map).ToList();
    }

    public async Task<TicketResponseDto?> ServeNextAsync()
    {
        var waiting = await _ticketRepository.GetWaitingAsync();
        var next = waiting.FirstOrDefault();
        if (next is null)
        {
            return null;
        }

        next.Status = TicketStatus.Served;
        next.ServedAtUtc = DateTime.UtcNow;
        await _ticketRepository.SaveChangesAsync();
        await _queueNotifier.BroadcastQueueUpdatedAsync();

        return Map(next);
    }

    public async Task<TicketResponseDto?> UpdateStatusAsync(int ticketId, string status)
    {
        var normalized = NormalizeStatus(status);
        if (normalized is null)
        {
            return null;
        }

        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket is null)
        {
            return null;
        }

        ticket.Status = normalized;
        ticket.ServedAtUtc = normalized == TicketStatus.Served ? DateTime.UtcNow : null;
        await _ticketRepository.SaveChangesAsync();
        await _queueNotifier.BroadcastQueueUpdatedAsync();

        return Map(ticket);
    }

    public async Task<bool> DeleteTicketAsync(int ticketId)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket is null)
        {
            return false;
        }

        await _ticketRepository.DeleteAsync(ticket);
        await _ticketRepository.SaveChangesAsync();
        await _queueNotifier.BroadcastQueueUpdatedAsync();
        return true;
    }

    public async Task<QueueSummaryResponseDto> GetSummaryAsync()
    {
        var tickets = await _ticketRepository.GetAllAsync();
        var waiting = tickets.Where(t => t.Status == TicketStatus.Waiting).ToList();

        var avgPriority = waiting.Count == 0 ? 0 : (int)Math.Round(waiting.Average(t => t.Priority));
        var oldestMinutes = waiting.Count == 0
            ? 0
            : (int)Math.Round((DateTime.UtcNow - waiting.Min(t => t.CreatedAtUtc)).TotalMinutes);

        return new QueueSummaryResponseDto(
            tickets.Count,
            waiting.Count,
            tickets.Count(t => t.Status == TicketStatus.Served),
            tickets.Count(t => t.Status == TicketStatus.Cancelled),
            avgPriority,
            oldestMinutes);
    }

    private static TicketResponseDto Map(Ticket ticket)
    {
        return new TicketResponseDto(
            ticket.Id,
            ticket.CustomerName,
            ticket.Status,
            ticket.Priority,
            ticket.EstimatedWaitMinutes,
            ticket.CreatedAtUtc,
            ticket.ServedAtUtc,
            ticket.AppointmentId);
    }

    private static int PredictWait(int queueLength, int averageServiceMinutes, int staffCount)
    {
        var safeStaffCount = Math.Max(1, staffCount);
        return (int)Math.Ceiling((double)(queueLength * averageServiceMinutes) / safeStaffCount);
    }

    private static string? NormalizeStatus(string status)
    {
        var normalized = status.Trim().ToLowerInvariant();
        return normalized switch
        {
            "waiting" => TicketStatus.Waiting,
            "served" => TicketStatus.Served,
            "cancelled" => TicketStatus.Cancelled,
            _ => null
        };
    }
}