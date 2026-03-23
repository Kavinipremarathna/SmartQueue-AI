namespace SmartQueueAPI.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = RoleNames.Customer;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
