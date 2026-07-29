using ConfiOS.BuildingBlocks.Domain.ValueObjects;
using ConfiOS.Modules.Catalog.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConfiOS.Modules.Catalog.Infrastructure.Persistence.Configurations;

/// <summary>Maps <see cref="Product"/> and its money-valued price.</summary>
public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("products");
        builder.HasKey(product => product.Id);

        builder.Property(product => product.Name).HasMaxLength(200).IsRequired();
        builder.Property(product => product.Sku).HasMaxLength(64).IsRequired();

        // SKU is unique per tenant, not globally: two businesses may use the same code.
        builder.HasIndex(product => new { product.TenantId, product.Sku }).IsUnique();

        // Money spans an amount and a currency, so it maps as an owned pair of columns; the
        // currency is stored as its ISO code and rebuilt on read.
        builder.OwnsOne(product => product.Price, price =>
        {
            price.Property(money => money.Amount)
                .HasColumnName("price_amount")
                .HasColumnType("numeric(18,4)")
                .IsRequired();

            price.Property(money => money.Currency)
                .HasColumnName("price_currency")
                .HasMaxLength(3)
                .IsRequired()
                .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        });
    }
}
