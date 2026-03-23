using SmartQueueAPI.DTOs.Queue;
using SmartQueueAPI.DTOs.Tickets;

namespace SmartQueueAPI.Services.Interfaces;

public interface IQueueService
{
    Task<TicketResponseDto> CreateTicketAsync(CreateTicketRequestDto request);
    Task<QueueCurrentResponseDto> GetCurrentQueueAsync();
    Task<IReadOnlyList<TicketResponseDto>> GetAllTicketsAsync();
    Task<TicketResponseDto?> ServeNextAsync();
    Task<TicketResponseDto?> UpdateStatusAsync(int ticketId, string status);
    Task<bool> DeleteTicketAsync(int ticketId);
    Task<QueueSummaryResponseDto> GetSummaryAsync();
}
