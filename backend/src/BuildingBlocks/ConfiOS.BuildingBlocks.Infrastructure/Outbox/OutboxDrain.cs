using ConfiOS.BuildingBlocks.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ConfiOS.BuildingBlocks.Infrastructure.Outbox;

/// <summary>
/// Binds the outbox processor to one module's context so the host can drain it without
/// naming that context.
/// </summary>
/// <typeparam name="TContext">Module context holding the outbox table.</typeparam>
/// <param name="module">Module name, for logging.</param>
/// <param name="context">Module database context.</param>
/// <param name="dispatcher">Runs each event's handlers in their own tenant-resolved scope.</param>
/// <param name="clock">Source of the current time.</param>
public sealed class OutboxDrain<TContext>(
    string module,
    TContext context,
    OutboxDispatcher dispatcher,
    IClock clock) : IOutboxDrain
    where TContext : DbContext
{
    private readonly OutboxProcessor<TContext> _processor = new(context, dispatcher, clock);

    public string Module { get; } = module;

    public Task<int> DrainAsync(int batchSize, CancellationToken cancellationToken = default) =>
        _processor.ProcessAsync(batchSize, cancellationToken);
}
