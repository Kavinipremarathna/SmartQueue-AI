using SmartQueueAPI.Entities;

namespace SmartQueueAPI.Repositories;

public interface IQueueConfigurationRepository
{
    Task<QueueConfiguration> GetOrCreateAsync();
    Task SaveChangesAsync();
}
