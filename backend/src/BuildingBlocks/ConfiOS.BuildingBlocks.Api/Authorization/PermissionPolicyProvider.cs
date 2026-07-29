using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace ConfiOS.BuildingBlocks.Api.Authorization;

/// <summary>
/// Turns a permission key used as a policy name (for example <c>catalog.products.create</c>)
/// into a policy that requires the matching <c>permission</c> claim.
/// </summary>
/// <remarks>
/// This lets every module authorise against its own permissions with
/// <c>RequireAuthorization("module.resource.action")</c> without any module registering a
/// policy per key at start-up — which also avoids coupling the host to each module's
/// permission list.
/// </remarks>
public sealed class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    /// <summary>Claim type carrying a granted permission. Matches the access-token generator.</summary>
    public const string PermissionClaim = "permission";

    private readonly DefaultAuthorizationPolicyProvider _fallback;

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallback = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        ArgumentNullException.ThrowIfNull(policyName);

        // Permission keys are module.resource.action; anything else is a named policy.
        if (policyName.Contains('.', StringComparison.Ordinal))
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim(PermissionClaim, policyName)
                .Build();
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        return _fallback.GetPolicyAsync(policyName);
    }
}
