using SmartQueueAPI.DTOs.Auth;
using SmartQueueAPI.Entities;

namespace SmartQueueAPI.Infrastructure.Security;

public interface IJwtTokenGenerator
{
    LoginResponseDto Generate(User user);
}
