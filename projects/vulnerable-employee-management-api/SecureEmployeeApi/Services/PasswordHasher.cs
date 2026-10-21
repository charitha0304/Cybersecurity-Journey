using System.Security.Cryptography;

namespace SecureEmployeeApi.Services;

// FIX (SQLi login bypass + excessive exposure): passwords are stored as salted
// PBKDF2-HMAC-SHA256 hashes and verified in constant time. No plain-text password
// is ever stored or compared. Self-contained — no extra NuGet package.
public static class PasswordHasher
{
    private const int SaltSize = 16;        // 128-bit salt
    private const int KeySize = 32;         // 256-bit derived key
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algo = HashAlgorithmName.SHA256;

    // Format: {iterations}.{base64 salt}.{base64 key}
    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algo, KeySize);
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    public static bool Verify(string password, string stored)
    {
        var parts = stored.Split('.', 3);
        if (parts.Length != 3) return false;
        if (!int.TryParse(parts[0], out var iterations)) return false;

        var salt = Convert.FromBase64String(parts[1]);
        var key = Convert.FromBase64String(parts[2]);
        var attempt = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, Algo, key.Length);
        return CryptographicOperations.FixedTimeEquals(attempt, key);
    }
}
