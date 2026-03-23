using SmartQueueAPI.Entities;

namespace SmartQueueAPI.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task AddAsync(User user);
    Task<bool> AnyAsync();
}
