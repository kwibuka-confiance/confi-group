using System.Globalization;
using System.Security.Claims;
using System.Text;
using ConfiOS.Modules.Identity.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ConfiOS.Modules.Identity.Infrastructure.Security;

/// <summary>
/// Signs and validates the short-lived token that carries a completed password
/// check into the business-selection step.
/// </summary>
/// <remarks>
/// It uses the same signing key as access tokens but carries a distinct
/// <c>purpose</c> claim and a separate audience, and it is validated explicitly
/// here rather than by the bearer middleware. That keeps it unusable as an access
/// token even though the two share a key.
/// </remarks>
/// <param name="configuration">Provides the <c>Jwt</c> section.</param>
public sealed class BusinessSelectionTokens(IConfiguration configuration) : IBusinessSelectionTokens
{
    private const string Purpose = "tenant-selection";
    private const string PurposeClaim = "purpose";
    private const string EmailClaim = "email";
    private const string TenantClaim = "selectable_tenant";
    private const string Audience = "confios-business-selection";
    private const int DefaultLifetimeMinutes = 5;

    public BusinessSelectionToken Issue(string email, IReadOnlyCollection<Guid> tenantIds)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentNullException.ThrowIfNull(tenantIds);

        var jwt = configuration.GetSection("Jwt");
        var lifetime = int.TryParse(
            jwt["BusinessSelectionMinutes"],
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var minutes)
            ? minutes
            : DefaultLifetimeMinutes;

        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(lifetime);

        var claims = new List<Claim>
        {
            new(PurposeClaim, Purpose),
            new(EmailClaim, email),
        };
        claims.AddRange(tenantIds.Select(id =>
            new Claim(TenantClaim, id.ToString())));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = jwt["Issuer"],
            Audience = Audience,
            IssuedAt = DateTime.UtcNow,
            Expires = expiresAt.UtcDateTime,
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = new SigningCredentials(SigningKey(jwt), SecurityAlgorithms.HmacSha256),
        };

        return new BusinessSelectionToken(new JsonWebTokenHandler().CreateToken(descriptor), expiresAt);
    }

    public BusinessSelectionPayload? Validate(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var jwt = configuration.GetSection("Jwt");
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],
            ValidateAudience = true,
            ValidAudience = Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SigningKey(jwt),
            ClockSkew = TimeSpan.FromSeconds(30),
        };

        var result = new JsonWebTokenHandler().ValidateTokenAsync(token, parameters).GetAwaiter().GetResult();
        if (!result.IsValid)
        {
            return null;
        }

        var identity = result.ClaimsIdentity;
        if (identity.FindFirst(PurposeClaim)?.Value != Purpose)
        {
            return null;
        }

        var email = identity.FindFirst(EmailClaim)?.Value;
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        var tenantIds = identity.FindAll(TenantClaim)
            .Select(claim => Guid.TryParse(claim.Value, CultureInfo.InvariantCulture, out var id) ? id : Guid.Empty)
            .Where(id => id != Guid.Empty)
            .ToList();

        return tenantIds.Count == 0 ? null : new BusinessSelectionPayload(email, tenantIds);
    }

    private static SymmetricSecurityKey SigningKey(IConfigurationSection jwt)
    {
        var key = jwt["SigningKey"];
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException("Jwt:SigningKey is not configured.");
        }

        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    }
}
