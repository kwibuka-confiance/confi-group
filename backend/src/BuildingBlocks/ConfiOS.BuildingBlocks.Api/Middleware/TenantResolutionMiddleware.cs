using System.Globalization;
using System.Security.Claims;
using ConfiOS.BuildingBlocks.Application.Context;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ConfiOS.BuildingBlocks.Api.Middleware;

/// <summary>
/// Establishes tenant context for the request from validated identity claims (MT-002).
/// </summary>
/// <remarks>
/// The tenant is taken from the authenticated principal, never from a header or body
/// field, so a caller cannot address another tenant by editing a request. Membership was
/// proven when the token was issued; a client switching tenants must obtain a new token.
/// </remarks>
/// <param name="next">Next middleware in the pipeline.</param>
/// <param name="logger">Logger for resolution failures.</param>
public sealed class TenantResolutionMiddleware(
    RequestDelegate next,
    ILogger<TenantResolutionMiddleware> logger)
{
    /// <summary>Claim carrying the tenant the token was issued for.</summary>
    public const string TenantClaim = "tenant_id";

    /// <summary>Claim carrying the branch the session is scoped to, when there is one.</summary>
    public const string BranchClaim = "branch_id";

    /// <summary>Claim carrying the tenant's default ISO 4217 currency.</summary>
    public const string CurrencyClaim = "currency";

    /// <summary>Claim carrying the tenant's IANA time zone.</summary>
    public const string TimeZoneClaim = "time_zone";

    /// <summary>
    /// Subject claim. Read directly rather than through <see cref="ClaimTypes.NameIdentifier"/>
    /// because the host disables inbound claim mapping, so claims keep their original
    /// JWT names.
    /// </summary>
    public const string SubjectClaim = "sub";

    public async Task InvokeAsync(HttpContext context, AmbientContext ambientContext)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(ambientContext);

        var user = context.User;

        if (user?.Identity?.IsAuthenticated == true)
        {
            if (TryReadGuid(user, TenantClaim, out var tenantId))
            {
                var branchId = TryReadGuid(user, BranchClaim, out var branch)
                    ? BranchId.From(branch)
                    : (BranchId?)null;

                var userId = TryReadUserId(user);

                ambientContext.Resolve(
                    TenantId.From(tenantId),
                    userId,
                    branchId,
                    CultureInfo.CurrentUICulture.Name,
                    user.FindFirstValue(CurrencyClaim) ?? "RWF",
                    user.FindFirstValue(TimeZoneClaim) ?? "Africa/Kigali");
            }
            else
            {
                // Authenticated but with no usable tenant claim. Left unresolved: endpoints
                // that need a tenant fail closed rather than defaulting to one.
                logger.LogWarning(
                    "Authenticated request carried no valid tenant claim. Trace {TraceId}.",
                    context.TraceIdentifier);
            }
        }

        await next(context).ConfigureAwait(false);
    }

    private static bool TryReadGuid(ClaimsPrincipal principal, string claimType, out Guid value)
    {
        var raw = principal.FindFirstValue(claimType);
        return Guid.TryParse(raw, CultureInfo.InvariantCulture, out value) && value != Guid.Empty;
    }

    /// <summary>
    /// Reads the subject under either name, so the middleware keeps working whether or not
    /// a given host maps inbound claims to the legacy WS-Federation URIs.
    /// </summary>
    private static UserId? TryReadUserId(ClaimsPrincipal principal)
    {
        if (TryReadGuid(principal, SubjectClaim, out var subject)
            || TryReadGuid(principal, ClaimTypes.NameIdentifier, out subject))
        {
            return UserId.From(subject);
        }

        return null;
    }
}
