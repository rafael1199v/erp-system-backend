using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Erp.Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InventoryContractCens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_warehouses_company_id",
                schema: "inv",
                table: "warehouses");

            migrationBuilder.DropIndex(
                name: "IX_units_company_id",
                schema: "inv",
                table: "units");

            migrationBuilder.DropIndex(
                name: "IX_products_company_id",
                schema: "inv",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_categories_company_id",
                schema: "inv",
                table: "categories");

            migrationBuilder.AddColumn<string>(
                name: "cen",
                schema: "inv",
                table: "warehouses",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "cen",
                schema: "inv",
                table: "units",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "cen",
                schema: "inv",
                table: "transactions",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "cen",
                schema: "inv",
                table: "products",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "sku",
                schema: "inv",
                table: "products",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cen",
                schema: "inv",
                table: "inventory_movements",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "external_reference",
                schema: "inv",
                table: "inventory_movements",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cen",
                schema: "inv",
                table: "companies",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "cen",
                schema: "inv",
                table: "categories",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE inv.warehouses
                SET cen = substr(md5(random()::text || clock_timestamp()::text), 1, 8) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 4) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 4) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 4) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 12)
                WHERE cen = '';

                UPDATE inv.units
                SET cen = substr(md5(random()::text || clock_timestamp()::text), 1, 8) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 4) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 4) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 4) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 12)
                WHERE cen = '';

                UPDATE inv.transactions
                SET cen = substr(md5(random()::text || clock_timestamp()::text), 1, 8) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 4) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 4) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 4) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 12)
                WHERE cen = '';

                UPDATE inv.products
                SET cen = substr(md5(random()::text || clock_timestamp()::text), 1, 8) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 4) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 4) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 4) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 12)
                WHERE cen = '';

                UPDATE inv.inventory_movements
                SET cen = substr(md5(random()::text || clock_timestamp()::text), 1, 8) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 4) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 4) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 4) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 12)
                WHERE cen = '';

                UPDATE inv.companies
                SET cen = substr(md5(random()::text || clock_timestamp()::text), 1, 8) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 4) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 4) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 4) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 12)
                WHERE cen = '';

                UPDATE inv.categories
                SET cen = substr(md5(random()::text || clock_timestamp()::text), 1, 8) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 4) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 4) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 4) || '-' ||
                          substr(md5(random()::text || clock_timestamp()::text), 1, 12)
                WHERE cen = '';

                ALTER TABLE inv.warehouses ALTER COLUMN cen DROP DEFAULT;
                ALTER TABLE inv.units ALTER COLUMN cen DROP DEFAULT;
                ALTER TABLE inv.transactions ALTER COLUMN cen DROP DEFAULT;
                ALTER TABLE inv.products ALTER COLUMN cen DROP DEFAULT;
                ALTER TABLE inv.inventory_movements ALTER COLUMN cen DROP DEFAULT;
                ALTER TABLE inv.companies ALTER COLUMN cen DROP DEFAULT;
                ALTER TABLE inv.categories ALTER COLUMN cen DROP DEFAULT;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_warehouses_company_id_cen",
                schema: "inv",
                table: "warehouses",
                columns: new[] { "company_id", "cen" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_units_company_id_cen",
                schema: "inv",
                table: "units",
                columns: new[] { "company_id", "cen" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transactions_cen",
                schema: "inv",
                table: "transactions",
                column: "cen",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_products_company_id_cen",
                schema: "inv",
                table: "products",
                columns: new[] { "company_id", "cen" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_products_company_id_sku",
                schema: "inv",
                table: "products",
                columns: new[] { "company_id", "sku" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_inventory_movements_cen",
                schema: "inv",
                table: "inventory_movements",
                column: "cen",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_companies_cen",
                schema: "inv",
                table: "companies",
                column: "cen",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_categories_company_id_cen",
                schema: "inv",
                table: "categories",
                columns: new[] { "company_id", "cen" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_warehouses_company_id_cen",
                schema: "inv",
                table: "warehouses");

            migrationBuilder.DropIndex(
                name: "IX_units_company_id_cen",
                schema: "inv",
                table: "units");

            migrationBuilder.DropIndex(
                name: "IX_transactions_cen",
                schema: "inv",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_products_company_id_cen",
                schema: "inv",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_products_company_id_sku",
                schema: "inv",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_inventory_movements_cen",
                schema: "inv",
                table: "inventory_movements");

            migrationBuilder.DropIndex(
                name: "IX_companies_cen",
                schema: "inv",
                table: "companies");

            migrationBuilder.DropIndex(
                name: "IX_categories_company_id_cen",
                schema: "inv",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "cen",
                schema: "inv",
                table: "warehouses");

            migrationBuilder.DropColumn(
                name: "cen",
                schema: "inv",
                table: "units");

            migrationBuilder.DropColumn(
                name: "cen",
                schema: "inv",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "cen",
                schema: "inv",
                table: "products");

            migrationBuilder.DropColumn(
                name: "sku",
                schema: "inv",
                table: "products");

            migrationBuilder.DropColumn(
                name: "cen",
                schema: "inv",
                table: "inventory_movements");

            migrationBuilder.DropColumn(
                name: "external_reference",
                schema: "inv",
                table: "inventory_movements");

            migrationBuilder.DropColumn(
                name: "cen",
                schema: "inv",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "cen",
                schema: "inv",
                table: "categories");

            migrationBuilder.CreateIndex(
                name: "IX_warehouses_company_id",
                schema: "inv",
                table: "warehouses",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_units_company_id",
                schema: "inv",
                table: "units",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_company_id",
                schema: "inv",
                table: "products",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_categories_company_id",
                schema: "inv",
                table: "categories",
                column: "company_id");
        }
    }
}
