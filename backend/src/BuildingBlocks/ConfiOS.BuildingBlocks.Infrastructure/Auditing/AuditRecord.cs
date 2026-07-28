namespace ConfiOS.BuildingBlocks.Infrastructure.Auditing;

/// <summary>
/// A row in the append-only audit log. Has no update or delete path by design: the type
/// exposes no mutators and the table grants no UPDATE or DELETE to the application role.
/// </summary>
public sealed class AuditRecord
{
    private AuditRecord()
    {
    }

    public Guid Id { get; private set; }

    public Guid TenantId { get; private set; }

    public Guid? BranchId { get; private set; }

    public Guid? UserId { get; private set; }

    /// <summary>Stable action name, for example <c>sales.sale-completed</c>.</summary>
    public string Action { get; private set; } = string.Empty;

    public string EntityType { get; private set; } = string.Empty;

    public Guid EntityId { get; private set; }

    /// <summary>Structured before and after values, stored as jsonb.</summary>
    public string? Summary { get; private set; }

    public string? CorrelationId { get; private set; }

    public DateTimeOffset RecordedAt { get; private set; }

    public static AuditRecord Create(
        Guid tenantId,
        Guid? branchId,
        Guid? userId,
        string action,
        string entityType,
        Guid entityId,
        string? summary,
        string? correlationId,
        DateTimeOffset recordedAt) => new()
        {
            Id = Guid.CreateVersion7(),
            TenantId = tenantId,
            BranchId = branchId,
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Summary = summary,
            CorrelationId = correlationId,
            RecordedAt = recordedAt,
        };
}
