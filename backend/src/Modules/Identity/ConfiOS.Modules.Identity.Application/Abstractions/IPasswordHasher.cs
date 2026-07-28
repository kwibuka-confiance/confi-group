namespace ConfiOS.Modules.Identity.Application.Abstractions;

/// <summary>
/// Hashes and verifies passwords.
/// </summary>
/// <remarks>
/// Abstracted so the algorithm and its work factor can be raised over time without
/// touching the domain. Implementations must use a salted, deliberately slow KDF, and
/// verification must be constant-time.
/// </remarks>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string hash);
}
