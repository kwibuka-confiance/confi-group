using ConfiOS.BuildingBlocks.Domain.ValueObjects;
using ConfiOS.Modules.Identity.Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConfiOS.Modules.Identity.Infrastructure.Persistence.Configurations;

/// <summary>Maps <see cref="Branch"/> and its owned address.</summary>
public sealed class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("branches");
        builder.HasKey(branch => branch.Id);

        builder.Property(branch => branch.Name).HasMaxLength(200).IsRequired();
        builder.Property(branch => branch.Code).HasMaxLength(20).IsRequired();

        // Unique per tenant, not globally: two businesses may both have an "HQ".
        builder.HasIndex(branch => new { branch.TenantId, branch.Code }).IsUnique();

        // An address spans several columns, so it stays an owned type.
        builder.OwnsOne(branch => branch.Address, address =>
        {
            address.Property(value => value.Country).HasColumnName("country").HasMaxLength(2);
            address.Property(value => value.Province).HasColumnName("province").HasMaxLength(100);
            address.Property(value => value.District).HasColumnName("district").HasMaxLength(100);
            address.Property(value => value.Sector).HasColumnName("sector").HasMaxLength(100);
            address.Property(value => value.Cell).HasColumnName("cell").HasMaxLength(100);
            address.Property(value => value.Village).HasColumnName("village").HasMaxLength(100);
            address.Property(value => value.Street).HasColumnName("street").HasMaxLength(200);
        });

        builder.Property(branch => branch.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20)
            .HasConversion(
                phone => phone!.Value,
                value => PhoneNumber.Create(value));
    }
}
