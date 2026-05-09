using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Erp.Sales.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SalesModuleCen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyCen",
                schema: "sal",
                table: "warehouse_configurations",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MainWarehouseCen",
                schema: "sal",
                table: "warehouse_configurations",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cen",
                schema: "sal",
                table: "waiters",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyCen",
                schema: "sal",
                table: "waiters",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cen",
                schema: "sal",
                table: "teams",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyCen",
                schema: "sal",
                table: "teams",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CategoryCen",
                schema: "sal",
                table: "team_configurations",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyCen",
                schema: "sal",
                table: "team_configurations",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyCen",
                schema: "sal",
                table: "tax_configurations",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cen",
                schema: "sal",
                table: "sales",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyCen",
                schema: "sal",
                table: "sales",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductCen",
                schema: "sal",
                table: "sale_details",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cen",
                schema: "sal",
                table: "restaurant_orders",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cen",
                schema: "sal",
                table: "restaurant_order_details",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductCen",
                schema: "sal",
                table: "restaurant_order_details",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyCen",
                schema: "sal",
                table: "orders",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductCen",
                schema: "sal",
                table: "order_details",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyCen",
                schema: "sal",
                table: "customers",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_warehouse_configurations_CompanyCen",
                schema: "sal",
                table: "warehouse_configurations",
                column: "CompanyCen");

            migrationBuilder.CreateIndex(
                name: "IX_warehouse_configurations_MainWarehouseCen",
                schema: "sal",
                table: "warehouse_configurations",
                column: "MainWarehouseCen");

            migrationBuilder.CreateIndex(
                name: "IX_waiters_CompanyCen",
                schema: "sal",
                table: "waiters",
                column: "CompanyCen");

            migrationBuilder.CreateIndex(
                name: "IX_waiters_CompanyId_Cen",
                schema: "sal",
                table: "waiters",
                columns: new[] { "CompanyId", "Cen" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_teams_CompanyCen",
                schema: "sal",
                table: "teams",
                column: "CompanyCen");

            migrationBuilder.CreateIndex(
                name: "IX_teams_CompanyId_Cen",
                schema: "sal",
                table: "teams",
                columns: new[] { "CompanyId", "Cen" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_team_configurations_CategoryCen",
                schema: "sal",
                table: "team_configurations",
                column: "CategoryCen");

            migrationBuilder.CreateIndex(
                name: "IX_team_configurations_CompanyCen",
                schema: "sal",
                table: "team_configurations",
                column: "CompanyCen");

            migrationBuilder.CreateIndex(
                name: "IX_tax_configurations_CompanyCen",
                schema: "sal",
                table: "tax_configurations",
                column: "CompanyCen");

            migrationBuilder.CreateIndex(
                name: "IX_sales_Cen",
                schema: "sal",
                table: "sales",
                column: "Cen",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sales_CompanyCen",
                schema: "sal",
                table: "sales",
                column: "CompanyCen");

            migrationBuilder.CreateIndex(
                name: "IX_sale_details_ProductCen",
                schema: "sal",
                table: "sale_details",
                column: "ProductCen");

            migrationBuilder.CreateIndex(
                name: "IX_restaurant_orders_Cen",
                schema: "sal",
                table: "restaurant_orders",
                column: "Cen",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_restaurant_order_details_Cen",
                schema: "sal",
                table: "restaurant_order_details",
                column: "Cen",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_restaurant_order_details_ProductCen",
                schema: "sal",
                table: "restaurant_order_details",
                column: "ProductCen");

            migrationBuilder.CreateIndex(
                name: "IX_orders_CompanyCen",
                schema: "sal",
                table: "orders",
                column: "CompanyCen");

            migrationBuilder.CreateIndex(
                name: "IX_order_details_ProductCen",
                schema: "sal",
                table: "order_details",
                column: "ProductCen");

            migrationBuilder.CreateIndex(
                name: "IX_customers_CompanyCen",
                schema: "sal",
                table: "customers",
                column: "CompanyCen");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_warehouse_configurations_CompanyCen",
                schema: "sal",
                table: "warehouse_configurations");

            migrationBuilder.DropIndex(
                name: "IX_warehouse_configurations_MainWarehouseCen",
                schema: "sal",
                table: "warehouse_configurations");

            migrationBuilder.DropIndex(
                name: "IX_waiters_CompanyCen",
                schema: "sal",
                table: "waiters");

            migrationBuilder.DropIndex(
                name: "IX_waiters_CompanyId_Cen",
                schema: "sal",
                table: "waiters");

            migrationBuilder.DropIndex(
                name: "IX_teams_CompanyCen",
                schema: "sal",
                table: "teams");

            migrationBuilder.DropIndex(
                name: "IX_teams_CompanyId_Cen",
                schema: "sal",
                table: "teams");

            migrationBuilder.DropIndex(
                name: "IX_team_configurations_CategoryCen",
                schema: "sal",
                table: "team_configurations");

            migrationBuilder.DropIndex(
                name: "IX_team_configurations_CompanyCen",
                schema: "sal",
                table: "team_configurations");

            migrationBuilder.DropIndex(
                name: "IX_tax_configurations_CompanyCen",
                schema: "sal",
                table: "tax_configurations");

            migrationBuilder.DropIndex(
                name: "IX_sales_Cen",
                schema: "sal",
                table: "sales");

            migrationBuilder.DropIndex(
                name: "IX_sales_CompanyCen",
                schema: "sal",
                table: "sales");

            migrationBuilder.DropIndex(
                name: "IX_sale_details_ProductCen",
                schema: "sal",
                table: "sale_details");

            migrationBuilder.DropIndex(
                name: "IX_restaurant_orders_Cen",
                schema: "sal",
                table: "restaurant_orders");

            migrationBuilder.DropIndex(
                name: "IX_restaurant_order_details_Cen",
                schema: "sal",
                table: "restaurant_order_details");

            migrationBuilder.DropIndex(
                name: "IX_restaurant_order_details_ProductCen",
                schema: "sal",
                table: "restaurant_order_details");

            migrationBuilder.DropIndex(
                name: "IX_orders_CompanyCen",
                schema: "sal",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_order_details_ProductCen",
                schema: "sal",
                table: "order_details");

            migrationBuilder.DropIndex(
                name: "IX_customers_CompanyCen",
                schema: "sal",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "CompanyCen",
                schema: "sal",
                table: "warehouse_configurations");

            migrationBuilder.DropColumn(
                name: "MainWarehouseCen",
                schema: "sal",
                table: "warehouse_configurations");

            migrationBuilder.DropColumn(
                name: "Cen",
                schema: "sal",
                table: "waiters");

            migrationBuilder.DropColumn(
                name: "CompanyCen",
                schema: "sal",
                table: "waiters");

            migrationBuilder.DropColumn(
                name: "Cen",
                schema: "sal",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "CompanyCen",
                schema: "sal",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "CategoryCen",
                schema: "sal",
                table: "team_configurations");

            migrationBuilder.DropColumn(
                name: "CompanyCen",
                schema: "sal",
                table: "team_configurations");

            migrationBuilder.DropColumn(
                name: "CompanyCen",
                schema: "sal",
                table: "tax_configurations");

            migrationBuilder.DropColumn(
                name: "Cen",
                schema: "sal",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "CompanyCen",
                schema: "sal",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "ProductCen",
                schema: "sal",
                table: "sale_details");

            migrationBuilder.DropColumn(
                name: "Cen",
                schema: "sal",
                table: "restaurant_orders");

            migrationBuilder.DropColumn(
                name: "Cen",
                schema: "sal",
                table: "restaurant_order_details");

            migrationBuilder.DropColumn(
                name: "ProductCen",
                schema: "sal",
                table: "restaurant_order_details");

            migrationBuilder.DropColumn(
                name: "CompanyCen",
                schema: "sal",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "ProductCen",
                schema: "sal",
                table: "order_details");

            migrationBuilder.DropColumn(
                name: "CompanyCen",
                schema: "sal",
                table: "customers");
        }
    }
}
