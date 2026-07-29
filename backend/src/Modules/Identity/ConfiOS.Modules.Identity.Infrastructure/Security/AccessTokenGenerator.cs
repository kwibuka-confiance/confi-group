using System.Globalization;
using System.Security.Claims;
using System.Text;
using ConfiOS.Modules.Identity.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ConfiOS.Modules.Identity.Infrastructure.Security;

/// <summary>
/// Signs access tokens with the symmetric key from configuration, matching the validation
/// parameters the API host sets up.
/// </summary>
/// <remarks>
/// Claim names are kept in step with the tenant-resolution middleware and the permission
/// policies (<c>tenant_id</c>, <c>sub</c>, <c>currency</c>, <c>time_zone</c>,
/// <c>permission</c>). Inbound claim mapping is disabled on the host, so these names survive
/// validation unchanged.
/// </remarks>
/// <param name="configuration">Provides the <c>Jwt</c> section.</param>
public sealed class AccessTokenGenerator(IConfiguration configuration) : IAccessTokenGenerator
{
    private const string TenantClaim = "tenant_id";
    private const string SubjectClaim = "sub";
    private const string EmailClaim = "email";
    private const string CurrencyClaim = "currency";
    private const string TimeZoneClaim = "time_zone";
    private const string PermissionClaim = "permission";
    private const int DefaultLifetimeMinutes = 120;

    public AccessToken Generate(AccessTokenClaims claims)
    {
        ArgumentNullException.ThrowIfNull(claims);

        var jwt = configuration.GetSection("Jwt");
        var signingKey = jwt["SigningKey"];
        if (string.IsNullOrWhiteSpace(signingKey))
        {
            throw new InvalidOperationException("Jwt:SigningKey is not configured.");
        }

        var lifetime = int.TryParse(
            jwt["AccessTokenMinutes"],
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var minutes)
            ? minutes
            : DefaultLifetimeMinutes;

        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(lifetime);

        var identityClaims = new List<Claim>
        {
            new(SubjectClaim, claims.UserId.Value.ToString()),
            new(TenantClaim, claims.TenantId.Value.ToString()),
            new(EmailClaim, claims.Email),
            new(CurrencyClaim, claims.CurrencyCode),
            new(TimeZoneClaim, claims.TimeZoneId),
        };
        identityClaims.AddRange(
            claims.Permissions.Select(permission => new Claim(PermissionClaim, permission)));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = jwt["Issuer"],
            Audience = jwt["Audience"],
            IssuedAt = DateTime.UtcNow,
            Expires = expiresAt.UtcDateTime,
            Subject = new ClaimsIdentity(identityClaims),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                SecurityAlgorithms.HmacSha256),
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);
        return new AccessToken(token, expiresAt);
    }
}
