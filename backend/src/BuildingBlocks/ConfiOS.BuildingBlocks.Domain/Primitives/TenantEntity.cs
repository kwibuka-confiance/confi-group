namespace ConfiOS.BuildingBlocks.Domain.Primitives;

/// <summary>
/// Base class for tenant-owned aggregate roots. Supplies the common columns every
/// business table carries so individual modules do not re-declare them.
/// </summary>
public abstract class TenantEntity : AggregateRoot, ITenantScoped, IAuditable, ISoftDeletable
{
    protected TenantEntity(Guid id, TenantId tenantId)
        : base(id)
        => TenantId = tenantId.Value;

    protected TenantEntity()
    {
    }

    public Guid TenantId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public Guid? CreatedBy { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public Guid? UpdatedBy { get; private set; }

    public DateTimeOffset? DeletedAt { get; private set; }

    public bool IsDeleted { get; private set; }

    public void MarkCreated(DateTimeOffset at, Guid? by)
    {
        CreatedAt = at;
        CreatedBy = by;
    }

    public void MarkUpdated(DateTimeOffset at, Guid? by)
    {
        UpdatedAt = at;
        UpdatedBy = by;
    }

    public void MarkDeleted(DateTimeOffset at)
    {
        IsDeleted = true;
        DeletedAt = at;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
    }
}
