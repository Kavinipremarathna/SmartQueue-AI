using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SmartQueueAPI.Data;
using SmartQueueAPI.DTOs.Tickets;
using SmartQueueAPI.Entities;
using SmartQueueAPI.Infrastructure.Security;
using SmartQueueAPI.Repositories;
using SmartQueueAPI.Services;
using SmartQueueAPI.Services.Interfaces;

namespace SmartQueueAPI.Tests;

public class SmartQueueCoreTests
{
    [Fact]
    public async Task CreateTicket_ClampsPriority_AndSetsPrediction()
    {
        await using var context = CreateContext();
        var queueService = CreateQueueService(context);

        var created = await queueService.CreateTicketAsync(new CreateTicketRequestDto
        {
            CustomerName = "  Alice  ",
            Priority = 99
        });

        Assert.Equal("Alice", created.CustomerName);
        Assert.Equal(10, created.Priority);
        Assert.True(created.EstimatedWaitMinutes > 0);
    }

    [Fact]
    public async Task ServeNext_PicksHighestPriorityWaitingTicket()
    {
        await using var context = CreateContext();
        context.Tickets.AddRange(
            new Ticket { CustomerName = "Low", Priority = 1, Status = TicketStatus.Waiting },
            new Ticket { CustomerName = "High", Priority = 9, Status = TicketStatus.Waiting });
        context.QueueConfigurations.Add(new QueueConfiguration { StaffCount = 2, AverageServiceMinutes = 5 });
        await context.SaveChangesAsync();

        var queueService = CreateQueueService(context);
        var served = await queueService.ServeNextAsync();

        Assert.NotNull(served);
        Assert.Equal("High", served!.CustomerName);
        Assert.Equal(TicketStatus.Served, served.Status);
    }

    [Fact]
    public async Task Login_ReturnsJwtToken_ForValidCredentials()
    {
        await using var context = CreateContext();

        var userRepo = new UserRepository(context);
        var passwordHasher = new Sha256PasswordHasher();
        await userRepo.AddAsync(new User
        {
            Username = "admin",
            Role = RoleNames.Admin,
            PasswordHash = passwordHasher.Hash("admin123")
        });

        var jwtOptions = Options.Create(new JwtOptions
        {
            Key = "SmartQueue-Super-Secret-Jwt-Key-For-Development-Only-Replace",
            Issuer = "SmartQueueAPI",
            Audience = "SmartQueueClients",
            ExpiresMinutes = 120
        });

        var authService = new AuthService(userRepo, passwordHasher, new JwtTokenGenerator(jwtOptions));

        var response = await authService.LoginAsync("admin", "admin123");

        Assert.NotNull(response);
        Assert.False(string.IsNullOrWhiteSpace(response!.Token));
        Assert.Equal(RoleNames.Admin, response.Role);
    }

    private static QueueService CreateQueueService(AppDbContext context)
    {
        return new QueueService(
            new TicketRepository(context),
            new QueueConfigurationRepository(context),
            new NoOpQueueNotifier());
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private class NoOpQueueNotifier : IQueueNotifier
    {
        public Task BroadcastQueueUpdatedAsync()
        {
            return Task.CompletedTask;
        }
    }
}
