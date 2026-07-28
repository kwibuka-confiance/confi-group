using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.BuildingBlocks.Domain.ValueObjects;
using ConfiOS.Modules.Identity.Domain.Events;

// The aggregate exposes a TenantId property, which would shadow the type of the same name
// in expression position. The alias keeps both usable.
using TenantIdentifier = ConfiOS.BuildingBlocks.Domain.Primitives.TenantId;

namespace ConfiOS.Modules.Identity.Domain.Tenants;

/// <summary>
/// One independent business on the platform, and the root of all tenant-scoped data.
/// </summary>
/// <remarks>
/// Not a <see cref="TenantEntity"/>: a tenant is not owned by a tenant. It lives at the
/// platform level, which is also why it is never covered by the tenant query filter.
/// </remarks>
public sealed class Tenant : AggregateRoot, IAuditable, ISoftDeletable
{
    private readonly List<Branch> _branches = [];

    private Tenant(
        Guid id,
        string name,
        string slug,
        TenantSettings settings)
        : base(id)
    {
        Name = name;
        Slug = slug;
        Settings = settings;
        Status = TenantStatus.Active;
    }

    private Tenant()
    {
    }

    /// <summary>Trading name shown to users.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>URL-safe unique handle, for example <c>kwaconfi-depot</c>.</summary>
    public string Slug { get; private set; } = string.Empty;

    public TenantStatus Status { get; private set; }

    public TenantSettings Settings { get; private set; } = null!;

    public IReadOnlyCollection<Branch> Branches => _branches.AsReadOnly();

    public DateTimeOffset CreatedAt { get; private set; }

    public Guid? CreatedBy { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public Guid? UpdatedBy { get; private set; }

    public DateTimeOffset? DeletedAt { get; private set; }

    public bool IsDeleted { get; private set; }

    public static Tenant Register(string name, string slug, TenantSettings settings)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        ArgumentNullException.ThrowIfNull(settings);

        var tenant = new Tenant(Guid.CreateVersion7(), name.Trim(), slug.Trim().ToLowerInvariant(), settings);

        tenant.Raise(new TenantCreated(
            tenant.Id,
            tenant.Name,
            settings.CountryCode,
            settings.Currency.Code,
            settings.DefaultLanguage));

        return tenant;
    }

    /// <summary>This tenant's identifier in its strongly typed form.</summary>
    public TenantIdentifier TenantId => TenantIdentifier.From(Id);

    public Branch AddBranch(string name, string code, Address? address)
    {
        EnsureActive();
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        var normalisedCode = code.Trim().ToUpperInvariant();

        if (_branches.Exists(branch => string.Equals(branch.Code, normalisedCode, StringComparison.Ordinal)))
        {
            throw new DomainException(Error.Conflict(IdentityErrorCodes.BranchCodeTaken));
        }

        var newBranch = Branch.Create(TenantId, name, normalisedCode, address);
        _branches.Add(newBranch);

        Raise(new BranchCreated(Id, newBranch.Id, newBranch.Name));

        return newBranch;
    }

    public void Rename(string name)
    {
        EnsureActive();
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
    }

    public void UpdateSettings(TenantSettings settings)
    {
        EnsureActive();
        ArgumentNullException.ThrowIfNull(settings);
        Settings = settings;
    }

    public void Suspend(string reason)
    {
        if (Status == TenantStatus.Suspended)
        {
            return;
        }

        Status = TenantStatus.Suspended;
        Raise(new TenantSuspended(Id, reason));
    }

    public void Reactivate() => Status = TenantStatus.Active;

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

    /// <summary>
    /// A suspended tenant is readable so its owner can settle an invoice and export data,
    /// but nothing about it may change.
    /// </summary>
    private void EnsureActive()
    {
        if (Status != TenantStatus.Active)
        {
            throw new DomainException(Error.Forbidden(IdentityErrorCodes.TenantSuspended));
        }
    }
}

/// <summary>Lifecycle state of a tenant.</summary>
public enum TenantStatus
{
    Active = 0,
    Suspended = 1,
    Closed = 2,
}
