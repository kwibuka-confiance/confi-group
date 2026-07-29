using System.Text;
using ConfiOS.BuildingBlocks.Api.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ConfiOS.Api.Extensions;

/// <summary>Configures token validation and the permission-based authorization policies.</summary>
public static class AuthenticationExtensions
{
    /// <summary>
    /// Adds JWT bearer authentication and registers one policy per known permission, so an
    /// endpoint can require a permission by name.
    /// </summary>
    public static IServiceCollection AddConfiOsAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var jwt = configuration.GetSection("Jwt");
        var signingKey = jwt["SigningKey"];

        // Fail at startup rather than on the first request. A missing key means every
        // token would be rejected, which is far harder to diagnose from a 401.
        if (string.IsNullOrWhiteSpace(signingKey))
        {
            throw new InvalidOperationException(
                "Jwt:SigningKey is not configured. Set it through user secrets in development or the environment in production.");
        }

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt["Issuer"],
                    ValidAudience = jwt["Audience"],
                    ClockSkew = TimeSpan.FromSeconds(30),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                };

                // Tokens travel in the Authorization header only; no query-string fallback,
                // which would put credentials into access logs.
                options.MapInboundClaims = false;
            });

        // Permission policies are resolved on demand from the policy name, so no module has
        // to register a policy per permission and the host stays unaware of each module's keys.
        services.AddAuthorizationBuilder();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

        return services;
    }
}
