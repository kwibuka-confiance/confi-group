using System.Text.Json;
using ConfiOS.BuildingBlocks.Domain.Events;

namespace ConfiOS.BuildingBlocks.Infrastructure.Outbox;

/// <summary>
/// A domain event persisted alongside the change that produced it, then published
/// separately (docs/03-architecture/06-event-driven-architecture.md).
/// </summary>
/// <remarks>
/// Writing the event in the same transaction as the state change is what makes the two
/// consistent. Publishing it afterwards is at-least-once, so consumers must be idempotent.
/// </remarks>
public sealed class OutboxMessage
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private OutboxMessage()
    {
    }

    public Guid Id { get; private set; }

    /// <summary>Null for platform-level events that belong to no single tenant.</summary>
    public Guid? TenantId { get; private set; }

    /// <summary>Stable wire name, for example <c>inventory.stock-received</c>.</summary>
    public string EventType { get; private set; } = string.Empty;

    /// <summary>Assembly-qualified CLR type, used to deserialise on the way out.</summary>
    public string PayloadType { get; private set; } = string.Empty;

    public string Payload { get; private set; } = string.Empty;

    public DateTimeOffset OccurredAt { get; private set; }

    public DateTimeOffset? ProcessedAt { get; private set; }

    public int AttemptCount { get; private set; }

    /// <summary>Last failure, kept for diagnosis. Never contains payload secrets.</summary>
    public string? LastError { get; private set; }

    public static OutboxMessage From(IDomainEvent domainEvent, Guid? tenantId, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        var type = domainEvent.GetType();

        return new OutboxMessage
        {
            Id = domainEvent.EventId,
            TenantId = tenantId,
            EventType = domainEvent.EventType,
            PayloadType = type.AssemblyQualifiedName ?? type.FullName ?? type.Name,
            Payload = JsonSerializer.Serialize(domainEvent, type, SerializerOptions),
            OccurredAt = domainEvent.OccurredAt == default ? now : domainEvent.OccurredAt,
        };
    }

    public IDomainEvent? Deserialize()
    {
        var type = Type.GetType(PayloadType);
        return type is null ? null : JsonSerializer.Deserialize(Payload, type, SerializerOptions) as IDomainEvent;
    }

    public void MarkProcessed(DateTimeOffset at)
    {
        ProcessedAt = at;
        LastError = null;
        AttemptCount++;
    }

    public void MarkFailed(string error)
    {
        AttemptCount++;
        LastError = error.Length > 2000 ? error[..2000] : error;
    }
}
