#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Erp.Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SellPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "sell_price",
                schema: "inv",
                table: "products",
                type: "numeric(12,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sell_price",
                schema: "inv",
                table: "products");
        }
    }
}
