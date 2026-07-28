using System.Globalization;
using System.Security.Cryptography;
using ConfiOS.Modules.Identity.Application.Abstractions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ConfiOS.Modules.Identity.Infrastructure.Security;

/// <summary>
/// PBKDF2-HMAC-SHA512 password hashing.
/// </summary>
/// <remarks>
/// The stored format is <c>v1.iterations.salt.hash</c>, all base64. Embedding the version
/// and iteration count means the work factor can be raised later and old hashes still
/// verify, so nobody is locked out by a security improvement.
/// Argon2id would be preferable, but it needs a native dependency; PBKDF2 with a high
/// iteration count is the reasonable in-framework choice.
/// </remarks>
public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int DefaultIterations = 210_000;
    private const string Version = "v1";

    public string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Derive(password, salt, DefaultIterations);

        return string.Join(
            '.',
            Version,
            DefaultIterations.ToString(CultureInfo.InvariantCulture),
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash));
    }

    public bool Verify(string password, string hash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
        {
            return false;
        }

        var parts = hash.Split('.');

        if (parts.Length != 4
            || !string.Equals(parts[0], Version, StringComparison.Ordinal)
            || !int.TryParse(parts[1], CultureInfo.InvariantCulture, out var iterations))
        {
            return false;
        }

        try
        {
            var salt = Convert.FromBase64String(parts[2]);
            var expected = Convert.FromBase64String(parts[3]);
            var actual = Derive(password, salt, iterations);

            // Constant-time: a length-independent comparison avoids leaking how much of the
            // hash matched.
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static byte[] Derive(string password, byte[] salt, int iterations) =>
        KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA512, iterations, HashSize);
}
