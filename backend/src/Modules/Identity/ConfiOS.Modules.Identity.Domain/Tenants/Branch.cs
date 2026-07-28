using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.BuildingBlocks.Domain.ValueObjects;

namespace ConfiOS.Modules.Identity.Domain.Tenants;

/// <summary>
/// A physical location of a tenant: a shop, depot or office. Warehouses hang off a
/// branch, and a branch belongs to exactly one tenant.
/// </summary>
public sealed class Branch : TenantEntity
{
    private Branch(Guid id, TenantId tenantId, string name, string code, Address? address)
        : base(id, tenantId)
    {
        Name = name;
        Code = code;
        Address = address;
        IsActive = true;
    }

    private Branch()
    {
    }

    public string Name { get; private set; } = string.Empty;

    /// <summary>Short code unique within the tenant, for example <c>HQ</c> or <c>MUS01</c>.</summary>
    public string Code { get; private set; } = string.Empty;

    public Address? Address { get; private set; }

    public PhoneNumber? Phone { get; private set; }

    public bool IsActive { get; private set; }

    internal static Branch Create(TenantId tenantId, string name, string code, Address? address)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        return new Branch(Guid.CreateVersion7(), tenantId, name.Trim(), code, address);
    }

    public void Update(string name, Address? address, PhoneNumber? phone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name.Trim();
        Address = address;
        Phone = phone;
    }

    /// <summary>
    /// Closes the branch to new activity. Existing records stay readable, so history and
    /// reports for the branch remain intact (BR-005).
    /// </summary>
    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;
}
