using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConfiOS.BuildingBlocks.Infrastructure.Outbox;

/// <summary>Maps the outbox table into each module's schema.</summary>
/// <param name="schema">Schema of the owning module.</param>
public sealed class OutboxMessageConfiguration(string schema) : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("outbox_messages", schema);
        builder.HasKey(message => message.Id);

        builder.Property(message => message.EventType).HasMaxLength(200).IsRequired();
        builder.Property(message => message.PayloadType).HasMaxLength(500).IsRequired();
        builder.Property(message => message.Payload).HasColumnType("jsonb").IsRequired();
        builder.Property(message => message.LastError).HasMaxLength(2000);

        // Drives the dispatcher's "oldest unprocessed first" scan.
        builder
            .HasIndex(message => new { message.ProcessedAt, message.OccurredAt })
            .HasDatabaseName("ix_outbox_messages_unprocessed");

        builder.HasIndex(message => message.TenantId);
    }
}
