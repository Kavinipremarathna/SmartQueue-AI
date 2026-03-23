using Microsoft.EntityFrameworkCore;
using SmartQueueAPI.Data;
using SmartQueueAPI.Entities;

namespace SmartQueueAPI.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly AppDbContext _context;

    public TicketRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Ticket> AddAsync(Ticket ticket)
    {
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }

    public Task<Ticket?> GetByIdAsync(int id)
    {
        return _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
    }

    public Task<List<Ticket>> GetWaitingAsync()
    {
        return _context.Tickets
            .Where(t => t.Status == TicketStatus.Waiting)
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.CreatedAtUtc)
            .ToListAsync();
    }

    public Task<List<Ticket>> GetAllAsync()
    {
        return _context.Tickets
            .OrderBy(t => t.Status == TicketStatus.Waiting ? 0 : t.Status == TicketStatus.Served ? 1 : 2)
            .ThenByDescending(t => t.Priority)
            .ThenBy(t => t.CreatedAtUtc)
            .ToListAsync();
    }

    public Task DeleteAsync(Ticket ticket)
    {
        _context.Tickets.Remove(ticket);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
