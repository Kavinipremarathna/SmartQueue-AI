using SmartQueueAPI.Entities;

namespace SmartQueueAPI.Repositories;

public interface ITicketRepository
{
    Task<Ticket> AddAsync(Ticket ticket);
    Task<Ticket?> GetByIdAsync(int id);
    Task<List<Ticket>> GetWaitingAsync();
    Task<List<Ticket>> GetAllAsync();
    Task DeleteAsync(Ticket ticket);
    Task<int> SaveChangesAsync();
}
