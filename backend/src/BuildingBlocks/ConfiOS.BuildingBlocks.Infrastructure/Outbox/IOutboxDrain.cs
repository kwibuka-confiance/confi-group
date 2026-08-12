namespace ConfiOS.BuildingBlocks.Infrastructure.Outbox;

/// <summary>
/// One module's outbox, drained by the host without it knowing the module's context type.
/// </summary>
/// <remarks>
/// Each module owns its own outbox table, so there is one of these per module. Registering
/// them under a shared interface is what lets the host publish everything on one schedule
/// while still keeping each module's persistence private to it.
/// </remarks>
public interface IOutboxDrain
{
    /// <summary>Module name, used only for logging which outbox produced what.</summary>
    string Module { get; }

    /// <summary>Publishes up to <paramref name="batchSize"/> pending messages.</summary>
    /// <returns>Number of messages successfully published.</returns>
    Task<int> DrainAsync(int batchSize, CancellationToken cancellationToken = default);
}
