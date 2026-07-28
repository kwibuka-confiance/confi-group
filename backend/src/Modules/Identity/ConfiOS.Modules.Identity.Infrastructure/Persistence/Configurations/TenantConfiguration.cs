using ConfiOS.BuildingBlocks.Domain.ValueObjects;
using ConfiOS.Modules.Identity.Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConfiOS.Modules.Identity.Infrastructure.Persistence.Configurations;

/// <summary>Maps <see cref="Tenant"/> and its owned settings.</summary>
public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("tenants");
        builder.HasKey(tenant => tenant.Id);

        builder.Property(tenant => tenant.Name).HasMaxLength(200).IsRequired();
        builder.Property(tenant => tenant.Slug).HasMaxLength(100).IsRequired();
        builder.Property(tenant => tenant.Status).HasConversion<string>().HasMaxLength(20);

        // Platform-wide uniqueness: the slug appears in URLs, so it cannot repeat.
        builder.HasIndex(tenant => tenant.Slug).IsUnique();

        builder.OwnsOne(tenant => tenant.Settings, settings =>
        {
            settings.Property(value => value.CountryCode).HasColumnName("country_code").HasMaxLength(2).IsRequired();
            settings.Property(value => value.DefaultLanguage).HasColumnName("default_language").HasMaxLength(10).IsRequired();
            settings.Property(value => value.TimeZoneId).HasColumnName("time_zone_id").HasMaxLength(60).IsRequired();
            settings.Property(value => value.FiscalYearStartMonth).HasColumnName("fiscal_year_start_month");

            settings.Property(value => value.Currency)
                .HasColumnName("currency_code")
                .HasMaxLength(3)
                .IsRequired()
                .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        });

        builder.Navigation(tenant => tenant.Settings).IsRequired();

        builder
            .HasMany(tenant => tenant.Branches)
            .WithOne()
            .HasForeignKey(branch => branch.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Metadata
            .FindNavigation(nameof(Tenant.Branches))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(tenant => tenant.TenantId);
    }
}
