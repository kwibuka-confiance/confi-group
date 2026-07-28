namespace ConfiOS.BuildingBlocks.Domain.Events;

/// <summary>
/// Something that has happened in the domain, named in the past tense.
/// Modules react to each other's events rather than reading each other's tables.
/// </summary>
public interface IDomainEvent
{
    /// <summary>Identifies this occurrence, used for outbox idempotency.</summary>
    Guid EventId { get; }

    DateTimeOffset OccurredAt { get; }

    /// <summary>
    /// Stable wire name such as <c>inventory.stock-received</c>. It must not change once
    /// released: consumers and stored outbox rows depend on it.
    /// </summary>
    string EventType { get; }
}

/// <summary>Base record supplying the plumbing every domain event needs.</summary>
/// <param name="EventType">Stable wire name of the event.</param>
public abstract record DomainEvent(string EventType) : IDomainEvent
{
    public Guid EventId { get; } = Guid.CreateVersion7();

    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
