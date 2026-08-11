using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConfiOS.Modules.Catalog.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ProductPackagingAndPricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "base_unit_code",
                schema: "catalog",
                table: "products",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "cost_amount",
                schema: "catalog",
                table: "products",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cost_currency",
                schema: "catalog",
                table: "products",
                type: "character varying(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "deposit_amount",
                schema: "catalog",
                table: "products",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "deposit_currency",
                schema: "catalog",
                table: "products",
                type: "character varying(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "catalog",
                table: "products",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_returnable",
                schema: "catalog",
                table: "products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "tax_class",
                schema: "catalog",
                table: "products",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "product_packagings",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    unit_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    quantity_in_base_unit = table.Column<int>(type: "integer", nullable: false),
                    selling_price_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    selling_price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    cost_price_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    cost_price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    barcode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_packagings", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_packagings_products_product_id",
                        column: x => x.product_id,
                        principalSchema: "catalog",
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_product_packagings_product_id_unit_code",
                schema: "catalog",
                table: "product_packagings",
                columns: new[] { "product_id", "unit_code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_packagings",
                schema: "catalog");

            migrationBuilder.DropColumn(
                name: "base_unit_code",
                schema: "catalog",
                table: "products");

            migrationBuilder.DropColumn(
                name: "cost_amount",
                schema: "catalog",
                table: "products");

            migrationBuilder.DropColumn(
                name: "cost_currency",
                schema: "catalog",
                table: "products");

            migrationBuilder.DropColumn(
                name: "deposit_amount",
                schema: "catalog",
                table: "products");

            migrationBuilder.DropColumn(
                name: "deposit_currency",
                schema: "catalog",
                table: "products");

            migrationBuilder.DropColumn(
                name: "description",
                schema: "catalog",
                table: "products");

            migrationBuilder.DropColumn(
                name: "is_returnable",
                schema: "catalog",
                table: "products");

            migrationBuilder.DropColumn(
                name: "tax_class",
                schema: "catalog",
                table: "products");
        }
    }
}
