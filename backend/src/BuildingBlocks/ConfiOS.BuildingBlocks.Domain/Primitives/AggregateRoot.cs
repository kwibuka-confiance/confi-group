using ConfiOS.BuildingBlocks.Domain.Events;

namespace ConfiOS.BuildingBlocks.Domain.Primitives;

/// <summary>
/// Consistency boundary. Only aggregate roots may be loaded and saved directly;
/// everything inside an aggregate is reached through its root.
/// </summary>
public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot(Guid id)
        : base(id)
    {
    }

    protected AggregateRoot()
    {
    }

    /// <summary>
    /// Events raised but not yet dispatched. The infrastructure layer drains these
    /// into the outbox inside the same transaction as the state change.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();

    protected void Raise(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _domainEvents.Add(domainEvent);
    }
}
