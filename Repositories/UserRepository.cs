using Microsoft.EntityFrameworkCore;
using SmartQueueAPI.Data;
using SmartQueueAPI.Entities;

namespace SmartQueueAPI.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        return _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public Task<bool> AnyAsync()
    {
        return _context.Users.AnyAsync();
    }
}
