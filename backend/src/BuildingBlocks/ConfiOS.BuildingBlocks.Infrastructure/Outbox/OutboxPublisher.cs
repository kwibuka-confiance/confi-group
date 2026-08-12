using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConfiOS.BuildingBlocks.Infrastructure.Outbox;

/// <summary>How often the outbox is drained, and how much of it at a time.</summary>
public sealed class OutboxOptions
{
    /// <summary>Pause between passes. Short enough that an event feels immediate.</summary>
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>Messages published per module per pass.</summary>
    public int BatchSize { get; set; } = 50;
}

/// <summary>
/// Drains every module's outbox on a schedule.
/// </summary>
/// <remarks>
/// Publishing is at-least-once: a handler that succeeds but whose message fails to be
/// marked processed will run again, so handlers must be idempotent.
/// <para>
/// A failing pass is logged and retried on the next tick rather than stopping the service.
/// If this stopped, events would accumulate silently and the modules that depend on them
/// would simply never hear anything — which is exactly the failure this replaced.
/// </para>
/// </remarks>
/// <param name="scopeFactory">Opens a scope per pass to resolve the module drains.</param>
/// <param name="options">Schedule and batch size.</param>
/// <param name="logger">Reports counts and failures.</param>
public sealed partial class OutboxPublisher(
    IServiceScopeFactory scopeFactory,
    IOptions<OutboxOptions> options,
    ILogger<OutboxPublisher> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = options.Value;
        using var timer = new PeriodicTimer(settings.PollInterval);

        Started(logger, settings.PollInterval);

        do
        {
            try
            {
                await PublishAsync(settings.BatchSize, stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
#pragma warning disable CA1031 // A bad pass must not take the publisher down.
            catch (Exception exception)
#pragma warning restore CA1031
            {
                PassFailed(logger, exception);
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false));
    }

    private async Task PublishAsync(int batchSize, CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();

        foreach (var drain in scope.ServiceProvider.GetServices<IOutboxDrain>())
        {
            var published = await drain.DrainAsync(batchSize, cancellationToken).ConfigureAwait(false);

            if (published > 0)
            {
                Published(logger, published, drain.Module);
            }
        }
    }

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Outbox publisher started, polling every {Interval}.")]
    private static partial void Started(ILogger logger, TimeSpan interval);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Information,
        Message = "Published {Count} event(s) from the {Module} outbox.")]
    private static partial void Published(ILogger logger, int count, string module);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Error,
        Message = "Outbox pass failed. Retrying on the next tick.")]
    private static partial void PassFailed(ILogger logger, Exception exception);
}
