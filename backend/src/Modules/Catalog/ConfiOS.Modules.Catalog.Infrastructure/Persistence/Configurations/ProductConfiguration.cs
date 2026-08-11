using ConfiOS.BuildingBlocks.Domain.ValueObjects;
using ConfiOS.Modules.Catalog.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConfiOS.Modules.Catalog.Infrastructure.Persistence.Configurations;

/// <summary>Maps <see cref="Product"/>, its money-valued prices and its packagings.</summary>
public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("products");
        builder.HasKey(product => product.Id);

        builder.Property(product => product.Name).HasMaxLength(200).IsRequired();
        builder.Property(product => product.Sku).HasMaxLength(64).IsRequired();
        builder.Property(product => product.Description).HasMaxLength(2000);
        builder.Property(product => product.BaseUnitCode).HasMaxLength(16).IsRequired();
        builder.Property(product => product.TaxClass).HasConversion<string>().HasMaxLength(20);

        // SKU is unique per tenant, not globally: two businesses may use the same code.
        builder.HasIndex(product => new { product.TenantId, product.Sku }).IsUnique();

        MapMoney(builder, product => product.Price, "price");
        MapMoney(builder, product => product.CostPrice, "cost");
        MapMoney(builder, product => product.DepositPerBaseUnit, "deposit");

        // Packagings belong to the product and have no identity outside it, so they
        // are owned rather than a separate aggregate.
        builder.OwnsMany(product => product.Packagings, packaging =>
        {
            packaging.ToTable("product_packagings");
            packaging.WithOwner().HasForeignKey("product_id");
            packaging.Property<Guid>("id");
            packaging.HasKey("id");

            packaging.Property(item => item.UnitCode).HasMaxLength(16).IsRequired();
            packaging.Property(item => item.QuantityInBaseUnit).IsRequired();
            packaging.Property(item => item.Barcode).HasMaxLength(64);

            // A unit may only be defined once per product.
            packaging.HasIndex("product_id", nameof(Packaging.UnitCode)).IsUnique();

            packaging.OwnsOne(item => item.SellingPrice, money =>
            {
                money.Property(value => value.Amount)
                    .HasColumnName("selling_price_amount")
                    .HasColumnType("numeric(18,4)")
                    .IsRequired();
                money.Property(value => value.Currency)
                    .HasColumnName("selling_price_currency")
                    .HasMaxLength(3)
                    .IsRequired()
                    .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
            });

            packaging.OwnsOne(item => item.CostPrice, money =>
            {
                money.Property(value => value.Amount)
                    .HasColumnName("cost_price_amount")
                    .HasColumnType("numeric(18,4)");
                money.Property(value => value.Currency)
                    .HasColumnName("cost_price_currency")
                    .HasMaxLength(3)
                    .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
            });
        });
    }

    /// <summary>
    /// Money spans an amount and a currency, so it maps as an owned pair of columns;
    /// the currency is stored as its ISO code and rebuilt on read.
    /// </summary>
    private static void MapMoney(
        EntityTypeBuilder<Product> builder,
        System.Linq.Expressions.Expression<Func<Product, Money?>> selector,
        string prefix)
    {
        builder.OwnsOne(selector, money =>
        {
            money.Property(value => value.Amount)
                .HasColumnName($"{prefix}_amount")
                .HasColumnType("numeric(18,4)");

            money.Property(value => value.Currency)
                .HasColumnName($"{prefix}_currency")
                .HasMaxLength(3)
                .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        });
    }
}
