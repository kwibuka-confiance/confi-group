using ConfiOS.BuildingBlocks.Application.Abstractions;
using ConfiOS.BuildingBlocks.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace ConfiOS.BuildingBlocks.Infrastructure.Outbox;

/// <summary>
/// Publishes pending outbox messages to in-process handlers.
/// </summary>
/// <remarks>
/// While ConfiOS is a modular monolith this is all the transport that is needed. When a
/// module is extracted, this is the seam that starts writing to a broker instead, without
/// any change to the modules that raised the events.
/// <para>
/// Each message is handled in its own dependency scope with the tenant it belongs to
/// resolved, because a handler writes through a tenant-scoped context just as a request
/// does. Without that the handler would read nothing and write rows belonging to no one.
/// </para>
/// </remarks>
/// <typeparam name="TContext">Module context holding the outbox table.</typeparam>
/// <param name="context">Module database context.</param>
/// <param name="dispatcher">Runs each event's handlers in their own tenant-resolved scope.</param>
/// <param name="clock">Source of the current time.</param>
public sealed class OutboxProcessor<TContext>(
    TContext context,
    OutboxDispatcher dispatcher,
    IClock clock)
    where TContext : DbContext
{
    /// <summary>Publishes up to <paramref name="batchSize"/> pending messages.</summary>
    /// <returns>Number of messages successfully published.</returns>
    public async Task<int> ProcessAsync(int batchSize = 50, CancellationToken cancellationToken = default)
    {
        var pending = await context
            .Set<OutboxMessage>()
            .Where(message => message.ProcessedAt == null && message.AttemptCount < 10)
            .OrderBy(message => message.OccurredAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var published = 0;

        foreach (var message in pending)
        {
            try
            {
                var domainEvent = message.Deserialize();

                if (domainEvent is null)
                {
                    // The event type no longer exists in this build. Park it rather than
                    // retrying forever; it needs a human decision.
                    message.MarkFailed($"Unknown payload type '{message.PayloadType}'.");
                    continue;
                }

                await dispatcher
                    .DispatchAsync(domainEvent, message.TenantId, cancellationToken)
                    .ConfigureAwait(false);

                message.MarkProcessed(clock.UtcNow);
                published++;
            }
#pragma warning disable CA1031 // One poisoned message must not stop the batch.
            catch (Exception exception)
#pragma warning restore CA1031
            {
                message.MarkFailed(exception.Message);
            }
        }

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return published;
    }
}
