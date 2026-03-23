using System.Security.Cryptography;
using System.Text;
using SmartQueueAPI.Services.Interfaces;

namespace SmartQueueAPI.Infrastructure.Security;

public class Sha256PasswordHasher : IPasswordHasher
{
    public string Hash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }

    public bool Verify(string value, string hash)
    {
        var candidate = Hash(value);
        return string.Equals(candidate, hash, StringComparison.OrdinalIgnoreCase);
    }
}
