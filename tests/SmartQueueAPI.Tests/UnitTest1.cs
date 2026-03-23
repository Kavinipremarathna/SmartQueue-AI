using Microsoft.EntityFrameworkCore;
using SmartQueueAPI.Data;
using SmartQueueAPI.Models;
using SmartQueueAPI.Services;

namespace SmartQueueAPI.Tests;

public class QueueServiceTests
{
    [Fact]
    public async Task AddTicket_ClampsPriority_AndTrimsName()
    {
        await using var context = CreateContext();
        var service = new QueueService(context);

        var ticket = await service.AddTicket("  Alice  ", 99);

        Assert.Equal("Alice", ticket.CustomerName);
        Assert.Equal(10, ticket.Priority);
        Assert.Equal("Waiting", ticket.Status);
    }

    [Fact]
    public async Task GetQueue_ReturnsWaitingTickets_InPriorityThenAgeOrder()
    {
        await using var context = CreateContext();
        context.Tickets.AddRange(
            new Ticket { CustomerName = "Low", Priority = 1, CreatedAt = DateTime.Now.AddMinutes(-1) },
            new Ticket { CustomerName = "HighOld", Priority = 5, CreatedAt = DateTime.Now.AddMinutes(-10) },
            new Ticket { CustomerName = "HighNew", Priority = 5, CreatedAt = DateTime.Now.AddMinutes(-2) },
            new Ticket { CustomerName = "Done", Priority = 9, Status = "Served" });
        await context.SaveChangesAsync();

        var service = new QueueService(context);
        var queue = await service.GetQueue();

        Assert.Collection(
            queue,
            t => Assert.Equal("HighOld", t.CustomerName),
            t => Assert.Equal("HighNew", t.CustomerName),
            t => Assert.Equal("Low", t.CustomerName));
    }

    [Fact]
    public async Task ServeNextTicket_MarksHighestPriorityWaitingAsServed()
    {
        await using var context = CreateContext();
        context.Tickets.AddRange(
            new Ticket { CustomerName = "A", Priority = 2, CreatedAt = DateTime.Now.AddMinutes(-5) },
            new Ticket { CustomerName = "B", Priority = 6, CreatedAt = DateTime.Now.AddMinutes(-1) });
        await context.SaveChangesAsync();

        var service = new QueueService(context);
        var served = await service.ServeNextTicket();

        Assert.NotNull(served);
        Assert.Equal("B", served!.CustomerName);
        Assert.Equal("Served", served.Status);
    }

    [Fact]
    public async Task UpdateTicketStatus_RejectsInvalidStatus()
    {
        await using var context = CreateContext();
        context.Tickets.Add(new Ticket { CustomerName = "X", Priority = 1 });
        await context.SaveChangesAsync();

        var service = new QueueService(context);
        var result = await service.UpdateTicketStatus(1, "unknown");

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteTicket_RemovesTicket()
    {
        await using var context = CreateContext();
        context.Tickets.Add(new Ticket { CustomerName = "DeleteMe", Priority = 1 });
        await context.SaveChangesAsync();

        var service = new QueueService(context);
        var deleted = await service.DeleteTicket(1);

        Assert.True(deleted);
        Assert.Empty(context.Tickets);
    }

    [Fact]
    public async Task GetSummary_ReturnsExpectedCounts()
    {
        await using var context = CreateContext();
        context.Tickets.AddRange(
            new Ticket { CustomerName = "W1", Priority = 3, Status = "Waiting", CreatedAt = DateTime.Now.AddMinutes(-15) },
            new Ticket { CustomerName = "W2", Priority = 7, Status = "Waiting", CreatedAt = DateTime.Now.AddMinutes(-2) },
            new Ticket { CustomerName = "S1", Priority = 1, Status = "Served" },
            new Ticket { CustomerName = "C1", Priority = 1, Status = "Cancelled" });
        await context.SaveChangesAsync();

        var service = new QueueService(context);
        var summary = await service.GetSummary();

        Assert.Equal(4, summary.Total);
        Assert.Equal(2, summary.Waiting);
        Assert.Equal(1, summary.Served);
        Assert.Equal(1, summary.Cancelled);
        Assert.Equal(5, summary.AverageWaitingPriority);
        Assert.True(summary.OldestWaitingMinutes >= 14);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
