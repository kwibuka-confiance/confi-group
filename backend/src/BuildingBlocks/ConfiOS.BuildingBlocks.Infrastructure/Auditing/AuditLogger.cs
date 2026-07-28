using System.Text.Json;
using ConfiOS.BuildingBlocks.Application.Abstractions;
using ConfiOS.BuildingBlocks.Application.Auditing;
using Microsoft.EntityFrameworkCore;

namespace ConfiOS.BuildingBlocks.Infrastructure.Auditing;

/// <summary>
/// Writes audit entries through the caller's own context, so the entry and the action it
/// describes commit or roll back together (BR-002).
/// </summary>
/// <typeparam name="TContext">Module context that owns the current transaction.</typeparam>
/// <param name="context">Module database context.</param>
/// <param name="clock">Source of the current time.</param>
public sealed class AuditLogger<TContext>(TContext context, IClock clock) : IAuditLogger
    where TContext : DbContext
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public Task RecordAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entry);

        var record = AuditRecord.Create(
            entry.TenantId.Value,
            entry.BranchId?.Value,
            entry.UserId?.Value,
            entry.Action,
            entry.EntityType,
            entry.EntityId,
            entry.Summary is null ? null : JsonSerializer.Serialize(entry.Summary, SerializerOptions),
            entry.CorrelationId,
            clock.UtcNow);

        // Added, not saved: the caller's SaveChangesAsync commits this with the change.
        context.Set<AuditRecord>().Add(record);

        return Task.CompletedTask;
    }
}
