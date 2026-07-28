namespace ConfiOS.BuildingBlocks.Domain.Primitives;

/// <summary>
/// Strongly typed tenant identifier. Using a distinct type stops a BranchId or UserId
/// being passed where a tenant is expected, which is the most damaging class of
/// mistake in a shared multi-tenant database (MT-001).
/// </summary>
public readonly record struct TenantId(Guid Value)
{
    public static TenantId New() => new(Guid.CreateVersion7());

    public static TenantId From(Guid value) => value == Guid.Empty
        ? throw new ArgumentException("Tenant identifier must not be empty.", nameof(value))
        : new TenantId(value);

    public override string ToString() => Value.ToString();
}

/// <summary>Strongly typed branch identifier.</summary>
public readonly record struct BranchId(Guid Value)
{
    public static BranchId New() => new(Guid.CreateVersion7());

    public static BranchId From(Guid value) => value == Guid.Empty
        ? throw new ArgumentException("Branch identifier must not be empty.", nameof(value))
        : new BranchId(value);

    public override string ToString() => Value.ToString();
}

/// <summary>Strongly typed user identifier.</summary>
public readonly record struct UserId(Guid Value)
{
    public static UserId New() => new(Guid.CreateVersion7());

    public static UserId From(Guid value) => value == Guid.Empty
        ? throw new ArgumentException("User identifier must not be empty.", nameof(value))
        : new UserId(value);

    public override string ToString() => Value.ToString();
}
