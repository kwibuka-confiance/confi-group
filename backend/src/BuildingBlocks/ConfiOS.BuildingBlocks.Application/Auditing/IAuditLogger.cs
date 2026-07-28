using ConfiOS.BuildingBlocks.Domain.Primitives;

namespace ConfiOS.BuildingBlocks.Application.Auditing;

/// <summary>
/// Records sensitive actions to the append-only audit log (BR-002, BR-008).
/// </summary>
/// <remarks>
/// Entries are written in the same transaction as the action they describe, so an action
/// cannot succeed without its audit record. Nothing in the platform updates or deletes
/// them.
/// </remarks>
public interface IAuditLogger
{
    Task RecordAsync(AuditEntry entry, CancellationToken cancellationToken = default);
}

/// <summary>One audit record.</summary>
/// <param name="Action">Stable action name, for example <c>inventory.stock-adjusted</c>.</param>
/// <param name="EntityType">Type of the affected record.</param>
/// <param name="EntityId">Identifier of the affected record.</param>
/// <param name="TenantId">Owning tenant.</param>
/// <param name="BranchId">Branch the action happened in, when applicable.</param>
/// <param name="UserId">Acting user, absent for system actions.</param>
/// <param name="Summary">Structured before and after values. Never include secrets or credentials.</param>
/// <param name="CorrelationId">Trace identifier tying the entry to the originating request.</param>
public sealed record AuditEntry(
    string Action,
    string EntityType,
    Guid EntityId,
    TenantId TenantId,
    BranchId? BranchId,
    UserId? UserId,
    IReadOnlyDictionary<string, object?>? Summary = null,
    string? CorrelationId = null);
