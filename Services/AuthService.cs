using SmartQueueAPI.Infrastructure.Security;
using SmartQueueAPI.Repositories;
using SmartQueueAPI.Services.Interfaces;
using SmartQueueAPI.DTOs.Auth;

namespace SmartQueueAPI.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<LoginResponseDto?> LoginAsync(string username, string password)
    {
        var normalized = username.Trim();
        var user = await _userRepository.GetByUsernameAsync(normalized);
        if (user is null)
        {
            return null;
        }

        var validPassword = _passwordHasher.Verify(password, user.PasswordHash);
        if (!validPassword)
        {
            return null;
        }

        return _tokenGenerator.Generate(user);
    }
}
