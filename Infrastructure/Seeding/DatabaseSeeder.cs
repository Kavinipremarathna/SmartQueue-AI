using SmartQueueAPI.Entities;
using SmartQueueAPI.Repositories;
using SmartQueueAPI.Services.Interfaces;

namespace SmartQueueAPI.Infrastructure.Seeding;

public class DatabaseSeeder
{
    private readonly IUserRepository _userRepository;
    private readonly IQueueConfigurationRepository _configurationRepository;
    private readonly IPasswordHasher _passwordHasher;

    public DatabaseSeeder(
        IUserRepository userRepository,
        IQueueConfigurationRepository configurationRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _configurationRepository = configurationRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync()
    {
        await _configurationRepository.GetOrCreateAsync();

        if (await _userRepository.AnyAsync())
        {
            return;
        }

        var users = new[]
        {
            new User { Username = "admin", Role = RoleNames.Admin, PasswordHash = _passwordHasher.Hash("admin123") },
            new User { Username = "staff", Role = RoleNames.Staff, PasswordHash = _passwordHasher.Hash("staff123") },
            new User { Username = "customer", Role = RoleNames.Customer, PasswordHash = _passwordHasher.Hash("customer123") }
        };

        foreach (var user in users)
        {
            await _userRepository.AddAsync(user);
        }
    }
}
