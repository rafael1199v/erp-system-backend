#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Erp.Sales.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class QuantityForDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Note",
                schema: "sal",
                table: "restaurant_order_details",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
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
                name: "Quantity",
                schema: "sal",
                table: "restaurant_order_details");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                schema: "sal",
                table: "restaurant_order_details",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldNullable: true);
        }
    }
}
