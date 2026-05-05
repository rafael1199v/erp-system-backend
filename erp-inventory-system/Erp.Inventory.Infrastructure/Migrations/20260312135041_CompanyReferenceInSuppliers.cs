#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Erp.Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CompanyReferenceInSuppliers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "company_id",
                schema: "inv",
                table: "suppliers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_suppliers_company_id",
                schema: "inv",
                table: "suppliers",
                column: "company_id");

            migrationBuilder.AddForeignKey(
                name: "FK_suppliers_companies_company_id",
                schema: "inv",
                table: "suppliers",
                column: "company_id",
                principalSchema: "inv",
                principalTable: "companies",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_suppliers_companies_company_id",
                schema: "inv",
                table: "suppliers");

            migrationBuilder.DropIndex(
                name: "IX_suppliers_company_id",
                schema: "inv",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "company_id",
                schema: "inv",
                table: "suppliers");
        }
    }
}
