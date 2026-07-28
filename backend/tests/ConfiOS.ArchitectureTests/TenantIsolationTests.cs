using System.Reflection;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using Shouldly;
using Xunit;

namespace ConfiOS.ArchitectureTests;

/// <summary>
/// Enforces MT-001: every business entity belongs to exactly one tenant.
/// </summary>
/// <remarks>
/// An aggregate that is not <see cref="ITenantScoped"/> gets no query filter, so it would
/// be readable across tenants. That is the worst failure mode in a shared database, so it
/// is checked mechanically rather than left to review.
/// Plain reflection rather than NetArchTest here: the check must follow the whole
/// inheritance chain, and <see cref="Type.IsAssignableFrom"/> is unambiguous about that.
/// </remarks>
public sealed class TenantIsolationTests
{
    /// <summary>
    /// Aggregates that legitimately sit outside tenant scope, each with a reason.
    /// Anything added here needs justifying in review.
    /// </summary>
    private static readonly HashSet<string> PlatformLevelTypes = new(StringComparer.Ordinal)
    {
        // A tenant is not owned by a tenant; it is the root of the scope itself.
        "ConfiOS.Modules.Identity.Domain.Tenants.Tenant",
    };

    [Fact]
    public void Every_aggregate_root_is_tenant_scoped()
    {
        var offenders = DomainAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(AggregateRoot).IsAssignableFrom(type) && !type.IsAbstract)
            .Where(type => !typeof(ITenantScoped).IsAssignableFrom(type))
            .Select(type => type.FullName!)
            .Where(name => !PlatformLevelTypes.Contains(name))
            .Order(StringComparer.Ordinal)
            .ToList();

        offenders.ShouldBeEmpty(
            "every business aggregate must implement ITenantScoped so the tenant query filter applies");
    }

    [Fact]
    public void The_scan_actually_finds_aggregates()
    {
        // Guards the test above: if assembly discovery silently returned nothing, the
        // isolation check would pass while examining no types at all.
        var aggregates = DomainAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Count(type => typeof(AggregateRoot).IsAssignableFrom(type) && !type.IsAbstract);

        aggregates.ShouldBeGreaterThan(0);
    }

    /// <summary>
    /// Referencing a type from each domain assembly forces it to load. Scanning AppDomain
    /// alone would skip an assembly no earlier test had touched.
    /// </summary>
    private static Assembly[] DomainAssemblies() =>
    [
        typeof(AggregateRoot).Assembly,
        typeof(Modules.Identity.Domain.IdentityErrorCodes).Assembly,
    ];
}
