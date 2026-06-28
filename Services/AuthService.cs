using System.Security.Cryptography;
using System.Text;

namespace AscarServiceHub.Services;

public class AuthService
{
    private readonly IConfiguration _config;

    public AuthService(IConfiguration config)
    {
        _config = config;
    }

    public bool ValidateCredentials(string username, string password)
    {
        var expectedUser = _config["AdminCredentials:Username"] ?? "admin";
        var expectedHash = _config["AdminCredentials:PasswordHash"] ?? "";
        var inputHash = ComputeSha256(password);
        return string.Equals(username, expectedUser, StringComparison.OrdinalIgnoreCase)
               && string.Equals(inputHash, expectedHash, StringComparison.OrdinalIgnoreCase);
    }

    private static string ComputeSha256(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
