#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Erp.Sales.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ResendCounter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResendCount",
                schema: "sal",
                table: "restaurant_order_details",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResendCount",
                schema: "sal",
                table: "restaurant_order_details");
        }
    }
}
