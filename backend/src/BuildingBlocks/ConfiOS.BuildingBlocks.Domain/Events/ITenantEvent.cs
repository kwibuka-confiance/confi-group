namespace ConfiOS.BuildingBlocks.Domain.Events;

/// <summary>
/// An event that belongs to one tenant, stated by the event rather than inferred.
/// </summary>
/// <remarks>
/// Most events are raised while handling a request, where the tenant is already resolved.
/// Provisioning is the exception: a business is created before there is any tenant context
/// to read, so its events would otherwise be filed under no tenant at all and every
/// handler would run unscoped. Those are precisely the events other modules seed
/// themselves from, so the tenant has to travel with the event.
/// </remarks>
public interface ITenantEvent
{
    /// <summary>The tenant this event belongs to.</summary>
    Guid TenantId { get; }
}
