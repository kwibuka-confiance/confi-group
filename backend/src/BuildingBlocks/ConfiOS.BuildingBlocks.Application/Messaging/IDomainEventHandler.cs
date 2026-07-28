using ConfiOS.BuildingBlocks.Domain.Events;

namespace ConfiOS.BuildingBlocks.Application.Messaging;

/// <summary>
/// Reacts to an event raised by another module. Handlers run after the originating
/// transaction commits, so they must be idempotent: the outbox guarantees at-least-once
/// delivery, not exactly-once.
/// </summary>
/// <typeparam name="TEvent">Event handled.</typeparam>
public interface IDomainEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken);
}
