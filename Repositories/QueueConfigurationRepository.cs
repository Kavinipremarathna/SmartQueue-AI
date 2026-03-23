using Microsoft.EntityFrameworkCore;
using SmartQueueAPI.Data;
using SmartQueueAPI.Entities;

namespace SmartQueueAPI.Repositories;

public class QueueConfigurationRepository : IQueueConfigurationRepository
{
    private readonly AppDbContext _context;

    public QueueConfigurationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<QueueConfiguration> GetOrCreateAsync()
    {
        var config = await _context.QueueConfigurations.FirstOrDefaultAsync();
        if (config is not null)
        {
            return config;
        }

        config = new QueueConfiguration();
        _context.QueueConfigurations.Add(config);
        await _context.SaveChangesAsync();
        return config;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
