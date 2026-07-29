using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConfiOS.BuildingBlocks.Infrastructure.Auditing;

/// <summary>
/// Maps the append-only audit log into the module's own schema.
/// </summary>
/// <remarks>
/// Each module keeps its own <c>audit_records</c> table in its own schema, mirroring the
/// outbox: a module owns its schema exclusively, and this keeps per-module migrations
/// independent instead of racing to create one shared table.
/// </remarks>
/// <param name="schema">The owning module's schema, for example <c>identity</c>.</param>
public sealed class AuditRecordConfiguration(string schema) : IEntityTypeConfiguration<AuditRecord>
{
    public void Configure(EntityTypeBuilder<AuditRecord> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("audit_records", schema);
        builder.HasKey(record => record.Id);

        builder.Property(record => record.Action).HasMaxLength(200).IsRequired();
        builder.Property(record => record.EntityType).HasMaxLength(200).IsRequired();
        builder.Property(record => record.Summary).HasColumnType("jsonb");
        builder.Property(record => record.CorrelationId).HasMaxLength(100);

        builder.HasIndex(record => new { record.TenantId, record.RecordedAt });
        builder.HasIndex(record => new { record.TenantId, record.EntityType, record.EntityId });

        // Deliberately not ISoftDeletable and not ITenantScoped-filtered by convention:
        // audit rows are never removed, and platform-level review reads across tenants.
    }
}
