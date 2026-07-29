namespace ConfiOS.BuildingBlocks.Application.Authorization;

/// <summary>
/// A module's contribution to the platform's permission set. Each module registers one so
/// its permissions are granted to owners without the Identity module having to know them.
/// </summary>
/// <param name="Keys">The permission keys the module defines.</param>
public sealed record ModulePermissions(IReadOnlyList<string> Keys);

/// <summary>Every permission the platform recognises, aggregated across all loaded modules.</summary>
public interface IPermissionRegistry
{
    IReadOnlyList<string> All { get; }
}

/// <summary>
/// Aggregates the <see cref="ModulePermissions"/> every module registered, so provisioning
/// can grant an owner the full set — including permissions defined by modules loaded later.
/// </summary>
/// <param name="contributions">One entry per module.</param>
public sealed class PermissionRegistry(IEnumerable<ModulePermissions> contributions) : IPermissionRegistry
{
    public IReadOnlyList<string> All { get; } = contributions
        .SelectMany(contribution => contribution.Keys)
        .Distinct(StringComparer.Ordinal)
        .OrderBy(key => key, StringComparer.Ordinal)
        .ToList();
}
