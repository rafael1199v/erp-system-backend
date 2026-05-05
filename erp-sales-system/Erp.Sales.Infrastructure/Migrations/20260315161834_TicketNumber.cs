#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Erp.Sales.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TicketNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                schema: "sal",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "Title",
                schema: "sal",
                table: "orders");

            migrationBuilder.AddColumn<int>(
                name: "DailyNumber",
                schema: "sal",
                table: "orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyNumber",
                schema: "sal",
                table: "orders");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "sal",
                table: "sales",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "sal",
                table: "orders",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
