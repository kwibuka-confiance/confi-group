using ConfiOS.BuildingBlocks.Application.Abstractions;

namespace ConfiOS.BuildingBlocks.Infrastructure.Time;

/// <summary>Reads the machine clock. Replaced by a fake in tests.</summary>
public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
