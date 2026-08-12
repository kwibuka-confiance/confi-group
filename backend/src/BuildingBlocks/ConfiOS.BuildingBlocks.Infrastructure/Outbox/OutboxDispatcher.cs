using System.Reflection;
using System.Runtime.ExceptionServices;
using ConfiOS.BuildingBlocks.Application.Context;
using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.BuildingBlocks.Domain.Events;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace ConfiOS.BuildingBlocks.Infrastructure.Outbox;

/// <summary>
/// Runs one event's handlers in their own dependency scope.
/// </summary>
/// <remarks>
/// Separate from the processor because this is where the subtlety is: a handler writes
/// through a tenant-scoped context exactly as a request does, so the tenant the event
/// belongs to has to be resolved before the handler runs. Without that a handler reads
/// nothing and writes rows belonging to no one, and it fails silently rather than loudly.
/// </remarks>
/// <param name="scopeFactory">Opens the scope each event is handled in.</param>
public sealed class OutboxDispatcher(IServiceScopeFactory scopeFactory)
{
    /// <summary>Invokes every handler registered for this event's type.</summary>
    /// <param name="domainEvent">The event to publish.</param>
    /// <param name="tenantId">Owning tenant, or null for platform-level events.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>How many handlers ran.</returns>
    public async Task<int> DispatchAsync(
        IDomainEvent domainEvent,
        Guid? tenantId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        using var scope = scopeFactory.CreateScope();

        // Platform events belong to no tenant and are handled without one; anything
        // tenant-owned is handled as that tenant, so the usual isolation still applies.
        if (tenantId is { } owner && owner != Guid.Empty)
        {
            scope.ServiceProvider
                .GetRequiredService<AmbientContext>()
                .Resolve(TenantId.From(owner), userId: null, branchId: null, "en", "RWF", "Africa/Kigali");
        }

        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        var method = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync))!;

        var invoked = 0;

        foreach (var handler in scope.ServiceProvider.GetServices(handlerType))
        {
            if (handler is null)
            {
                continue;
            }

            // Invoking through reflection wraps a handler that throws before its first
            // await in a TargetInvocationException, whose message says nothing about what
            // actually went wrong. The outbox records that message against the poisoned
            // event, so the real exception is what has to reach it.
            try
            {
                await ((Task)method.Invoke(handler, [domainEvent, cancellationToken])!).ConfigureAwait(false);
            }
            catch (TargetInvocationException wrapped) when (wrapped.InnerException is not null)
            {
                ExceptionDispatchInfo.Capture(wrapped.InnerException).Throw();
            }

            invoked++;
        }

        return invoked;
    }
}
