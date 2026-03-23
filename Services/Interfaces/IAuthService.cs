using SmartQueueAPI.DTOs.Auth;

namespace SmartQueueAPI.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(string username, string password);
}
