namespace SmartQueueAPI.DTOs.Auth;

public record LoginResponseDto(
    string Token,
    string Username,
    string Role,
    DateTime ExpiresAtUtc);
