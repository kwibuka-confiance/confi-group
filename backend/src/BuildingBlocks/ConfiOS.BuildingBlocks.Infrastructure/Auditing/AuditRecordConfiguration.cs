using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConfiOS.BuildingBlocks.Infrastructure.Auditing;

/// <summary>Maps the audit log into the shared <c>audit</c> schema.</summary>
public sealed class AuditRecordConfiguration : IEntityTypeConfiguration<AuditRecord>
{
    /// <summary>Schema holding audit data for every module.</summary>
    public const string AuditSchema = "audit";

    public void Configure(EntityTypeBuilder<AuditRecord> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("audit_records", AuditSchema);
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
