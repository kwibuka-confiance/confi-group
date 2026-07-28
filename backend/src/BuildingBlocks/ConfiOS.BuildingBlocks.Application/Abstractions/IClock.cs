namespace ConfiOS.BuildingBlocks.Application.Abstractions;

/// <summary>
/// Supplies the current instant. Injected rather than calling <c>DateTimeOffset.UtcNow</c>
/// directly so time-dependent rules such as business-day close can be tested.
/// </summary>
public interface IClock
{
    /// <summary>Current instant in UTC. All stored timestamps are UTC.</summary>
    DateTimeOffset UtcNow { get; }
}
