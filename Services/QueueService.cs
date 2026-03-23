using SmartQueueAPI.Data;
using SmartQueueAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace SmartQueueAPI.Services
{
    public record QueueSummary(
        int Total,
        int Waiting,
        int Served,
        int Cancelled,
        int AverageWaitingPriority,
        int OldestWaitingMinutes);

    public class QueueService
    {
        private readonly AppDbContext _context;

        public QueueService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Ticket> AddTicket(string name, int priority)
        {
            var ticket = new Ticket
            {
                CustomerName = name.Trim(),
                Priority = Math.Clamp(priority, 0, 10)
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<List<Ticket>> GetQueue()
        {
            return await _context.Tickets
                .Where(t => t.Status == "Waiting")
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Ticket>> GetAllTickets()
        {
            return await _context.Tickets
                .OrderBy(t => t.Status == "Waiting" ? 0 : t.Status == "Served" ? 1 : 2)
                .ThenByDescending(t => t.Priority)
                .ThenBy(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<Ticket?> ServeNextTicket()
        {
            var next = await _context.Tickets
                .Where(t => t.Status == "Waiting")
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.CreatedAt)
                .FirstOrDefaultAsync();

            if (next is null)
            {
                return null;
            }

            next.Status = "Served";
            await _context.SaveChangesAsync();
            return next;
        }

        public async Task<Ticket?> UpdateTicketStatus(int id, string status)
        {
            var normalized = NormalizeStatus(status);
            if (normalized is null)
            {
                return null;
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket is null)
            {
                return null;
            }

            ticket.Status = normalized;
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<bool> DeleteTicket(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket is null)
            {
                return false;
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<QueueSummary> GetSummary()
        {
            var tickets = await _context.Tickets.ToListAsync();
            var waiting = tickets.Where(t => t.Status == "Waiting").ToList();

            var avgPriority = waiting.Count == 0
                ? 0
                : (int)Math.Round(waiting.Average(t => t.Priority));

            var oldestMinutes = waiting.Count == 0
                ? 0
                : (int)Math.Round((DateTime.Now - waiting.Min(t => t.CreatedAt)).TotalMinutes);

            return new QueueSummary(
                tickets.Count,
                waiting.Count,
                tickets.Count(t => t.Status == "Served"),
                tickets.Count(t => t.Status == "Cancelled"),
                avgPriority,
                oldestMinutes);
        }

        private static string? NormalizeStatus(string status)
        {
            var value = status.Trim().ToLowerInvariant();
            return value switch
            {
                "waiting" => "Waiting",
                "served" => "Served",
                "cancelled" => "Cancelled",
                _ => null
            };
        }
    }
}