namespace ConfiOS.BuildingBlocks.Domain.Primitives;

/// <summary>
/// Marks an entity as owned by exactly one tenant (MT-001). Every business entity
/// implements this; the persistence layer applies a global query filter to each one.
/// </summary>
public interface ITenantScoped
{
    Guid TenantId { get; }
}

/// <summary>
/// Carries the standard audit columns from docs/03-architecture/04-database-design.md.
/// The mutation methods exist for the persistence layer only: application code never
/// calls them, so a caller cannot forge who created or changed a record.
/// </summary>
public interface IAuditable
{
    DateTimeOffset CreatedAt { get; }

    Guid? CreatedBy { get; }

    DateTimeOffset? UpdatedAt { get; }

    Guid? UpdatedBy { get; }

    void MarkCreated(DateTimeOffset at, Guid? by);

    void MarkUpdated(DateTimeOffset at, Guid? by);
}

/// <summary>
/// Business records are archived rather than deleted (BR-005). Ledger, audit and event
/// tables deliberately do not implement this.
/// </summary>
public interface ISoftDeletable
{
    DateTimeOffset? DeletedAt { get; }

    bool IsDeleted { get; }

    void MarkDeleted(DateTimeOffset at);

    void Restore();
}
